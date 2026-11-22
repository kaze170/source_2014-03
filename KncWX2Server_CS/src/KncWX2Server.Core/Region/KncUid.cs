using System.Security.Cryptography;

namespace KncWX2Server.Core.Region;

/// <summary>
/// 64-bit UidType bit-packing matching KncUidType.h / KncUidType.cpp.
/// Supports runtime switching between standard 40-bit PureUID (TW/HK/JP)
/// and EXTEND_SERVER_GROUP_MASK 36-bit PureUID / 10-bit ServerGroupID (CN).
/// </summary>
public static class KncUid
{
    public static uint Die32()
    {
        Span<byte> buf = stackalloc byte[4];
        RandomNumberGenerator.Fill(buf);
        uint val = BitConverter.ToUInt32(buf);
        return val == 0 ? 1u : val;
    }

    public static long MakeUid(RegionProfile profile, int serverGroupId, long pureUid)
    {
        long uid = 0;
        SetServerGroupID(ref uid, serverGroupId, profile);
        SetPureUID(ref uid, pureUid, profile);
        return uid;
    }

    public static long GetTempUID(RegionProfile? profile = null)
    {
        bool extend = (profile ?? RegionProfile.Current).ExtendServerGroupMask;
        long maxPure = extend ? 0x0000_000F_FFFF_FFFFL : 0x0000_00FF_FFFF_FFFFL;
        long randomVal = Random.Shared.NextInt64(1, maxPure + 1);
        return randomVal | (1L << 62);
    }

    public static long ExtractPureUID(long uid, RegionProfile? profile = null)
    {
        bool extend = (profile ?? RegionProfile.Current).ExtendServerGroupMask;
        return extend
            ? uid & 0x0000_000F_FFFF_FFFFL
            : uid & 0x0000_00FF_FFFF_FFFFL;
    }

    public static long ExtractServerGroupID(long uid, RegionProfile? profile = null)
    {
        bool extend = (profile ?? RegionProfile.Current).ExtendServerGroupMask;
        return extend
            ? (uid >> 52) & 0x0000_0000_0000_03FFL
            : (uid >> 56) & 0x0000_0000_0000_003FL;
    }

    public static long ExtractServerID(long uid, RegionProfile? profile = null)
    {
        bool extend = (profile ?? RegionProfile.Current).ExtendServerGroupMask;
        return extend
            ? (uid >> 44) & 0x0000_0000_0000_00FFL
            : (uid >> 48) & 0x0000_0000_0000_00FFL;
    }

    public static long ExtractReservedID(long uid, RegionProfile? profile = null)
    {
        bool extend = (profile ?? RegionProfile.Current).ExtendServerGroupMask;
        return extend
            ? (uid >> 36) & 0x0000_0000_0000_00FFL
            : (uid >> 40) & 0x0000_0000_0000_00FFL;
    }

    public static void SetTempUID(ref long destUid)
    {
        unchecked
        {
            destUid = (destUid & (long)0xBFFF_FFFF_FFFF_FFFFUL) | ((1L << 62) & 0x4000_0000_0000_0000L);
        }
    }

    public static void SetPureUID(ref long destUid, long srcUid, RegionProfile? profile = null)
    {
        bool extend = (profile ?? RegionProfile.Current).ExtendServerGroupMask;
        unchecked
        {
            if (extend)
                destUid = (destUid & (long)0xFFFF_FFF0_0000_0000UL) | (srcUid & 0x0000_000F_FFFF_FFFFL);
            else
                destUid = (destUid & (long)0xFFFF_FF00_0000_0000UL) | (srcUid & 0x0000_00FF_FFFF_FFFFL);
        }
    }

    public static void SetServerGroupID(ref long destUid, long srcUid, RegionProfile? profile = null)
    {
        bool extend = (profile ?? RegionProfile.Current).ExtendServerGroupMask;
        unchecked
        {
            if (extend)
                destUid = (destUid & (long)0xC00F_FFFF_FFFF_FFFFUL) | (((srcUid & 0x03FFL) << 52) & 0x3FF0_0000_0000_0000L);
            else
                destUid = (destUid & (long)0xC0FF_FFFF_FFFF_FFFFUL) | (((srcUid & 0x003FL) << 56) & 0x3F00_0000_0000_0000L);
        }
    }

    public static void SetServerID(ref long destUid, long srcUid, RegionProfile? profile = null)
    {
        bool extend = (profile ?? RegionProfile.Current).ExtendServerGroupMask;
        unchecked
        {
            if (extend)
                destUid = (destUid & (long)0xFFF0_0FFF_FFFF_FFFFUL) | (((srcUid & 0x00FFL) << 44) & 0x000F_F000_0000_0000L);
            else
                destUid = (destUid & (long)0xFF00_FFFF_FFFF_FFFFUL) | (((srcUid & 0x00FFL) << 48) & 0x00FF_0000_0000_0000L);
        }
    }

    public static void SetReservedID(ref long destUid, long srcUid, RegionProfile? profile = null)
    {
        bool extend = (profile ?? RegionProfile.Current).ExtendServerGroupMask;
        unchecked
        {
            if (extend)
                destUid = (destUid & (long)0xFFFF_F00F_FFFF_FFFFUL) | (((srcUid & 0x00FFL) << 36) & 0x0000_0FF0_0000_0000L);
            else
                destUid = (destUid & (long)0xFFFF_00FF_FFFF_FFFFUL) | (((srcUid & 0x00FFL) << 40) & 0x0000_FF00_0000_0000L);
        }
    }

    public static long ExtractCodeID(long uid, RegionProfile? profile = null)
    {
        bool extend = (profile ?? RegionProfile.Current).ExtendServerGroupMask;
        return extend
            ? (uid >> 36) & 0xFFFFL
            : (uid >> 40) & 0xFFFFL;
    }

    public static void SetCodeID(ref long destUid, long srcUid, RegionProfile? profile = null)
    {
        bool extend = (profile ?? RegionProfile.Current).ExtendServerGroupMask;
        unchecked
        {
            if (extend)
                destUid = (destUid & (long)0xFFF0_000F_FFFF_FFFFUL) | (((srcUid & 0xFFFFL) << 36) & 0x000F_FFF0_0000_0000L);
            else
                destUid = (destUid & (long)0xFF00_00FF_FFFF_FFFFUL) | (((srcUid & 0xFFFFL) << 40) & 0x00FF_FF00_0000_0000L);
        }
    }
}
