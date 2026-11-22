namespace KncWX2Server.Core.Framework;

/// <summary>
/// Faithful C# port of GameServer/SectorID.{h,cpp} (32-bit packed 3D spatial sector index:
/// bits 16..23 = X, bits 8..15 = Y, bits 0..7 = Z, InvalidSector = -1).
/// </summary>
public static class KSectorId
{
    public const int InvalidSector = -1;
    public const int MaxSector = 255;

    public static int ExtractX(int sectorId) => (sectorId >> 16) & 0x000000FF;
    public static int ExtractY(int sectorId) => (sectorId >> 8) & 0x000000FF;
    public static int ExtractZ(int sectorId) => sectorId & 0x000000FF;

    public static int SetX(int destId, int x)
    {
        x = Math.Clamp(x, 0, MaxSector);
        return (destId & 0x0000FFFF) | ((x & 0xFF) << 16);
    }

    public static int SetY(int destId, int y)
    {
        y = Math.Clamp(y, 0, MaxSector);
        return (destId & unchecked((int)0xFFFF00FF)) | ((y & 0xFF) << 8);
    }

    public static int SetZ(int destId, int z)
    {
        z = Math.Clamp(z, 0, MaxSector);
        return (destId & unchecked((int)0xFFFFFF00)) | (z & 0xFF);
    }

    public static int SetIndex(int x, int y, int z)
    {
        x = Math.Clamp(x, 0, MaxSector);
        y = Math.Clamp(y, 0, MaxSector);
        z = Math.Clamp(z, 0, MaxSector);
        return ((x & 0xFF) << 16) | ((y & 0xFF) << 8) | (z & 0xFF);
    }

    /// <summary>
    /// Computes the sector index for world position (posX, posY, posZ) given grid bounds & sector size.
    /// </summary>
    public static int FromPosition(float posX, float posY, float posZ, float sectorSize = 1500f, int maxX = 16, int maxY = 16, int maxZ = 16)
    {
        int ix = Math.Clamp((int)MathF.Floor(posX / MathF.Max(1f, sectorSize)) + (maxX / 2), 0, Math.Max(0, maxX - 1));
        int iy = Math.Clamp((int)MathF.Floor(posY / MathF.Max(1f, sectorSize)) + (maxY / 2), 0, Math.Max(0, maxY - 1));
        int iz = Math.Clamp((int)MathF.Floor(posZ / MathF.Max(1f, sectorSize)) + (maxZ / 2), 0, Math.Max(0, maxZ - 1));
        return SetIndex(ix, iy, iz);
    }

    /// <summary>
    /// Exact port of KSectorID::ExtractAdjacentSector (sorted lexicographically by X, Y, Z).
    /// </summary>
    public static List<int> ExtractAdjacentSector(int nowSector, int maxX, int maxY, int maxZ)
    {
        var adjacent = new List<int>(27);
        if (nowSector == InvalidSector)
            return adjacent;

        int ix = ExtractX(nowSector);
        int iy = ExtractY(nowSector);
        int iz = ExtractZ(nowSector);

        for (int x = ix - 1; x <= ix + 1; x++)
        {
            if (x < 0) continue;
            if (x >= maxX) break;

            for (int y = iy - 1; y <= iy + 1; y++)
            {
                if (y < 0) continue;
                if (y >= maxY) break;

                for (int z = iz - 1; z <= iz + 1; z++)
                {
                    if (z < 0) continue;
                    if (z >= maxZ) break;

                    adjacent.Add(SetIndex(x, y, z));
                }
            }
        }

        return adjacent;
    }

    /// <summary>
    /// Exact port of KSectorID::FindRemainder on two sorted sector lists:
    /// srcRemainder = src \ dest (sectors the user left),
    /// destRemainder = dest \ src (sectors the user newly entered).
    /// </summary>
    public static void FindRemainder(
        IReadOnlyList<int> srcSectorGroup,
        IReadOnlyList<int> destSectorGroup,
        out List<int> srcRemainder,
        out List<int> destRemainder)
    {
        srcRemainder = new List<int>();
        destRemainder = new List<int>();

        int i = 0;
        int j = 0;
        while (i < srcSectorGroup.Count || j < destSectorGroup.Count)
        {
            if (i >= srcSectorGroup.Count)
            {
                while (j < destSectorGroup.Count) destRemainder.Add(destSectorGroup[j++]);
                break;
            }
            if (j >= destSectorGroup.Count)
            {
                while (i < srcSectorGroup.Count) srcRemainder.Add(srcSectorGroup[i++]);
                break;
            }

            int s = srcSectorGroup[i];
            int d = destSectorGroup[j];
            if (s < d)
            {
                srcRemainder.Add(s);
                i++;
            }
            else if (s > d)
            {
                destRemainder.Add(d);
                j++;
            }
            else
            {
                i++;
                j++;
            }
        }
    }
}
