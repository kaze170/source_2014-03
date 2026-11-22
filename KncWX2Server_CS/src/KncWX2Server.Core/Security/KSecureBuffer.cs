using System.Buffers.Binary;
using System.Security.Cryptography;

namespace KncWX2Server.Core.Security;

/// <summary>
/// Wire-exact port of KncSecurity::KSecureBuffer (KNCSDK/Include/KncSecurity/SecureBuffer.{h,cpp}).
/// Wire format:
///   [SPI: 2B LE] [SeqNum: 4B LE] [IV: 8B] [DES-CBC Ciphertext (Payload + RFC2406 Pad + PadLen)] [ICV: 10B HMAC-MD5]
/// </summary>
public sealed class KSecureBuffer
{
    private readonly KSADatabase _saDb;
    private readonly ushort? _expectedSpi;
    private ushort _spi;
    private byte[] _rawBuffer = [];

    public KSecureBuffer(KSADatabase saDb, ushort spi = 0)
    {
        _saDb = saDb;
        _spi = spi;
    }

    public KSecureBuffer(KSADatabase saDb, ReadOnlySpan<byte> wireBytes)
    {
        _saDb = saDb;
        _rawBuffer = wireBytes.ToArray();
        if (_rawBuffer.Length >= 2)
            _spi = BinaryPrimitives.ReadUInt16LittleEndian(_rawBuffer.AsSpan(0, 2));
    }

    /// <summary>
    /// Receive-side constructor matching C++ KSecureBuffer(SpiType nSPI, const void* pRecvData, unsigned long nBytes).
    /// Looks up the SA using <paramref name="expectedSpi"/> (the server session's active SPI) rather than the
    /// header SPI field, because the C++ client replaces m_nSPIndex with a client-local random SPI in SADatabase::Insert.
    /// </summary>
    public KSecureBuffer(KSADatabase saDb, ushort expectedSpi, ReadOnlySpan<byte> wireBytes)
    {
        _saDb = saDb;
        _expectedSpi = expectedSpi;
        _spi = expectedSpi;
        _rawBuffer = wireBytes.ToArray();
    }

    public ushort Spi => _spi;
    public ReadOnlySpan<byte> WireBytes => _rawBuffer;

    /// <summary>
    /// Creates an encrypted & authenticated packet buffer matching KSecureBuffer::Create_notRWM.
    /// </summary>
    public bool Create(ReadOnlySpan<byte> plainPayload)
    {
        var sa = _saDb.GetSA(_spi);
        if (sa == null) return false;

        uint seqNum = sa.GetSequenceNum();
        byte[] iv = sa.GenerateIV();

        // RFC 2406 padding matching SecureBuffer.cpp GeneratePadding: 1..BlockSize pad bytes + 1 byte padLen
        int padBytes = KSecurityAssociation.BlockSize - ((plainPayload.Length + 1) % KSecurityAssociation.BlockSize);

        byte[] padded = new byte[plainPayload.Length + padBytes + 1];
        plainPayload.CopyTo(padded);
        for (int i = 0; i < padBytes; i++)
            padded[plainPayload.Length + i] = (byte)(i + 1);
        padded[^1] = (byte)padBytes;

        byte[] cipher = sa.Encrypt(iv, padded);

        // Assemble SPI (2) + SeqNum (4) + IV (8) + Ciphertext + ICV (10)
        int authHeaderAndCipherLen = 2 + 4 + KSecurityAssociation.IvSize + cipher.Length;
        byte[] output = new byte[authHeaderAndCipherLen + KSecurityAssociation.IcvSize];

        BinaryPrimitives.WriteUInt16LittleEndian(output.AsSpan(0, 2), _spi);
        BinaryPrimitives.WriteUInt32LittleEndian(output.AsSpan(2, 4), seqNum);
        iv.CopyTo(output.AsSpan(6, KSecurityAssociation.IvSize));
        cipher.CopyTo(output.AsSpan(14, cipher.Length));

        byte[] icv = sa.GenerateICV(output.AsSpan(0, authHeaderAndCipherLen));
        icv.CopyTo(output.AsSpan(authHeaderAndCipherLen, KSecurityAssociation.IcvSize));

        if (_spi != 0)
            sa.IncrSequenceNumNotRwm();

        _rawBuffer = output;
        return true;
    }

    /// <summary>
    /// Verifies ICV and sequence number matching KSecureBuffer::IsAuthentic_notRWM.
    /// </summary>
    public bool IsAuthentic(bool checkSequenceNum = true)
        => IsAuthentic(out _, out _, checkSequenceNum);

    public bool IsAuthentic(out bool sequenceMismatch, bool checkSequenceNum = true)
        => IsAuthentic(out sequenceMismatch, out _, checkSequenceNum);

    public bool IsAuthentic(out bool sequenceMismatch, out string failureReason, bool checkSequenceNum = true)
    {
        sequenceMismatch = false;
        failureReason = string.Empty;
        const int minSize = 2 + 4 + KSecurityAssociation.IvSize + KSecurityAssociation.BlockSize + KSecurityAssociation.IcvSize;
        if (_rawBuffer.Length < minSize)
        {
            failureReason = $"TooShort(len={_rawBuffer.Length})";
            return false;
        }

        ushort packetSpi = BinaryPrimitives.ReadUInt16LittleEndian(_rawBuffer.AsSpan(0, 2));
        uint seqNum = BinaryPrimitives.ReadUInt32LittleEndian(_rawBuffer.AsSpan(2, 4));
        _spi = _expectedSpi ?? packetSpi;

        var sa = _saDb.GetSA(_spi);
        if (sa == null)
        {
            failureReason = $"UnknownSpi(pktSpi={packetSpi},lookupSpi={_spi})";
            return false;
        }

        int authDataLen = _rawBuffer.Length - KSecurityAssociation.IcvSize;
        ReadOnlySpan<byte> icvActual = _rawBuffer.AsSpan(authDataLen, KSecurityAssociation.IcvSize);
        byte[] icvExpected = sa.GenerateICV(_rawBuffer.AsSpan(0, authDataLen));

        if (!CryptographicOperations.FixedTimeEquals(icvActual, icvExpected))
        {
            failureReason = $"IcvMismatch(pktSpi={packetSpi},lookupSpi={_spi},seq={seqNum},len={_rawBuffer.Length})";
            return false;
        }

        if (checkSequenceNum && _spi != 0)
        {
            if (!sa.IsValidSequenceNumNotRwm(seqNum))
            {
                sequenceMismatch = true;
                failureReason = $"SequenceMismatch(seq={seqNum},spi={_spi})";
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Decrypts ciphertext and strips RFC 2406 padding, matching KSecureBuffer::GetPayload_notRWM.
    /// </summary>
    public bool TryGetPayload(out byte[] payload, bool authentic = true, bool checkSequenceNum = true)
        => TryGetPayload(out payload, out _, out _, authentic, checkSequenceNum);

    public bool TryGetPayload(out byte[] payload, out bool sequenceMismatch, bool authentic = true, bool checkSequenceNum = true)
        => TryGetPayload(out payload, out sequenceMismatch, out _, authentic, checkSequenceNum);

    public bool TryGetPayload(out byte[] payload, out bool sequenceMismatch, out string failureReason, bool authentic = true, bool checkSequenceNum = true)
    {
        payload = [];
        sequenceMismatch = false;
        failureReason = string.Empty;
        if (authentic && !IsAuthentic(out sequenceMismatch, out failureReason, checkSequenceNum))
            return false;

        const int headerLen = 2 + 4 + KSecurityAssociation.IvSize;
        if (_rawBuffer.Length < headerLen + KSecurityAssociation.BlockSize + KSecurityAssociation.IcvSize)
        {
            failureReason = $"TooShortForDecrypt(len={_rawBuffer.Length})";
            return false;
        }

        var sa = _saDb.GetSA(_spi);
        if (sa == null)
        {
            failureReason = $"UnknownSpiDecrypt(lookupSpi={_spi})";
            return false;
        }

        ReadOnlySpan<byte> iv = _rawBuffer.AsSpan(6, KSecurityAssociation.IvSize);
        int cipherLen = _rawBuffer.Length - headerLen - KSecurityAssociation.IcvSize;
        if (cipherLen <= 0 || cipherLen % KSecurityAssociation.BlockSize != 0)
        {
            failureReason = $"UnalignedCipher(cipherLen={cipherLen})";
            return false;
        }

        ReadOnlySpan<byte> cipher = _rawBuffer.AsSpan(headerLen, cipherLen);
        byte[] plain = sa.Decrypt(iv, cipher);
        if (plain.Length == 0)
        {
            failureReason = "EmptyPlaintext";
            return false;
        }

        byte padLen = plain[^1];
        if (padLen >= plain.Length)
        {
            failureReason = $"InvalidPadLen(padLen={padLen},plainLen={plain.Length})";
            return false;
        }

        // Validate RFC 2406 padding sequence [1..padLen]
        int payloadLen = plain.Length - 1 - padLen;
        for (int i = 0; i < padLen; i++)
        {
            if (plain[payloadLen + i] != (byte)(i + 1))
            {
                failureReason = $"InvalidPadByte(padLen={padLen},idx={i})";
                return false;
            }
        }

        payload = plain[..payloadLen];
        return true;
    }
}
