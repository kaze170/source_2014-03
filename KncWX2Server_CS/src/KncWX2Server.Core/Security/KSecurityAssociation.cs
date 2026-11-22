using System.Security.Cryptography;
using KncWX2Server.Core.Region;
using KncWX2Server.Core.Serialization;

namespace KncWX2Server.Core.Security;

/// <summary>
/// Wire-exact port of KncSecurity::KSecurityAssociation (KNCSDK/Include/KncSecurity/SecurityAssociation.{h,cpp}).
/// Uses 8-byte DES-CBC with custom RFC 2406 padding ([1..padLen, (byte)padLen]) and 10-byte truncated HMAC-MD5 ICV.
/// </summary>
public sealed class KSecurityAssociation : IKSerializable
{
    public const int KeySize = 8;
    public const int BlockSize = 8;
    public const int IvSize = 8;
    public const int IcvSize = 10;
    public const uint MaxExtraPadBlocks = 1;

    private static readonly byte[] DefaultAuthKey = [0xC0, 0xD3, 0xBD, 0xC3, 0xB7, 0xCE, 0xB8, 0xB8];
    private static readonly byte[] DefaultCryptoKey = [0xC7, 0xD8, 0xC4, 0xBF, 0xB5, 0xE9, 0xC0, 0xFD];

    private readonly object _seqLock = new();
    private uint _sequenceNum = 1;
    private uint _lastSequenceNum = 0;
    private uint _replayWindowMask = 0;

    public byte[] AuthKey { get; private set; }
    public byte[] CryptoKey { get; private set; }

    public KSecurityAssociation()
    {
        AuthKey = (byte[])DefaultAuthKey.Clone();
        CryptoKey = (byte[])DefaultCryptoKey.Clone();
    }

    public KSecurityAssociation(ReadOnlySpan<byte> authKey, ReadOnlySpan<byte> cryptoKey)
    {
        if (authKey.Length != KeySize) throw new ArgumentException("AuthKey must be 8 bytes.", nameof(authKey));
        if (cryptoKey.Length != KeySize) throw new ArgumentException("CryptoKey must be 8 bytes.", nameof(cryptoKey));
        AuthKey = authKey.ToArray();
        CryptoKey = cryptoKey.ToArray();
    }

    public static KSecurityAssociation CreateRandom()
    {
        // Note: C++ ResetRandomizeKey draws bytes in [1..255]
        byte[] auth = new byte[KeySize];
        byte[] crypto = new byte[KeySize];
        for (int i = 0; i < KeySize; i++)
        {
            auth[i] = (byte)RandomNumberGenerator.GetInt32(1, 256);
            crypto[i] = (byte)RandomNumberGenerator.GetInt32(1, 256);
        }
        return new KSecurityAssociation(auth, crypto);
    }

    public KSecurityAssociation Clone()
    {
        lock (_seqLock)
        {
            return new KSecurityAssociation(AuthKey, CryptoKey)
            {
                _sequenceNum = _sequenceNum,
                _lastSequenceNum = _lastSequenceNum,
                _replayWindowMask = _replayWindowMask,
            };
        }
    }

    public uint GetSequenceNum()
    {
        lock (_seqLock)
            return _sequenceNum;
    }

    public void IncrSequenceNum()
    {
        lock (_seqLock)
            _sequenceNum++;
    }

    public void IncrSequenceNumNotRwm()
    {
        lock (_seqLock)
        {
            _sequenceNum++;
            if (_sequenceNum == 0)
                _sequenceNum = 1;
        }
    }

    public bool IsValidSequenceNumNotRwm(uint seqNum)
    {
        lock (_seqLock)
        {
            if (seqNum == 0)
                return false;

            if (seqNum > _lastSequenceNum)
            {
                _lastSequenceNum = seqNum;
                return true;
            }

            if (seqNum == _lastSequenceNum)
                return false;

            if (_lastSequenceNum != uint.MaxValue)
                return false;

            _lastSequenceNum = seqNum;
            return true;
        }
    }

    public byte[] GenerateICV(ReadOnlySpan<byte> data)
    {
        byte[] fullMac = HMACMD5.HashData(AuthKey, data);
        return fullMac[..IcvSize];
    }

    public byte[] GenerateIV()
    {
        byte[] iv = new byte[IvSize];
        RandomNumberGenerator.Fill(iv);
        return iv;
    }

    public byte[] Encrypt(ReadOnlySpan<byte> iv, ReadOnlySpan<byte> plainWithPadding)
    {
        if (iv.Length != IvSize) throw new ArgumentException("IV must be 8 bytes.", nameof(iv));
        if (plainWithPadding.Length % BlockSize != 0) throw new ArgumentException("Input must be 8-byte aligned.");

#pragma warning disable SYSLIB0021
        using var des = DES.Create();
#pragma warning restore SYSLIB0021
        des.Mode = CipherMode.CBC;
        des.Padding = PaddingMode.None;
        des.Key = CryptoKey;
        des.IV = iv.ToArray();

        using var encryptor = des.CreateEncryptor();
        byte[] input = plainWithPadding.ToArray();
        return encryptor.TransformFinalBlock(input, 0, input.Length);
    }

    public byte[] Decrypt(ReadOnlySpan<byte> iv, ReadOnlySpan<byte> cipherText)
    {
        if (iv.Length != IvSize) throw new ArgumentException("IV must be 8 bytes.", nameof(iv));
        if (cipherText.Length % BlockSize != 0) throw new ArgumentException("Ciphertext must be 8-byte aligned.");

#pragma warning disable SYSLIB0021
        using var des = DES.Create();
#pragma warning restore SYSLIB0021
        des.Mode = CipherMode.CBC;
        des.Padding = PaddingMode.None;
        des.Key = CryptoKey;
        des.IV = iv.ToArray();

        using var decryptor = des.CreateDecryptor();
        byte[] input = cipherText.ToArray();
        return decryptor.TransformFinalBlock(input, 0, input.Length);
    }

    /// <summary>
    /// Wire-exact serialization used in E_ACCEPT_CONNECTION_NOT handshake:
    /// KByteStream m_bsAuthKey, KByteStream m_bsCryptoKey, m_nSequenceNum, m_nLastSequenceNum, m_nReplayWindowMask.
    /// </summary>
    public bool Put(KSerializer ks, RegionProfile region)
    {
        uint seq, lastSeq, mask;
        lock (_seqLock)
        {
            seq = _sequenceNum;
            lastSeq = _lastSequenceNum;
            mask = _replayWindowMask;
        }

        return ks.Put((uint)AuthKey.Length)
            && ks.PutRawBytes(AuthKey)
            && ks.Put((uint)CryptoKey.Length)
            && ks.PutRawBytes(CryptoKey)
            && ks.Put(seq)
            && ks.Put(lastSeq)
            && ks.Put(mask);
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint authLen) || authLen > 1024) return false;
        byte[] auth = new byte[authLen];
        if (authLen > 0 && !ks.GetRawBytes(auth)) return false;

        if (!ks.Get(out uint cryptoLen) || cryptoLen > 1024) return false;
        byte[] crypto = new byte[cryptoLen];
        if (cryptoLen > 0 && !ks.GetRawBytes(crypto)) return false;

        if (!ks.Get(out uint seq)) return false;
        if (!ks.Get(out uint lastSeq)) return false;
        if (!ks.Get(out uint mask)) return false;

        lock (_seqLock)
        {
            AuthKey = auth;
            CryptoKey = crypto;
            _sequenceNum = seq;
            _lastSequenceNum = lastSeq;
            _replayWindowMask = mask;
        }
        return true;
    }
}
