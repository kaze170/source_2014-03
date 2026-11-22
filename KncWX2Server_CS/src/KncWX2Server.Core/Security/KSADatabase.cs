using System.Collections.Concurrent;

namespace KncWX2Server.Core.Security;

/// <summary>
/// Port of KncSecurity::KSADatabase (KNCSDK/Include/KncSecurity/SADatabase.{h,cpp}).
/// Always contains default SPI=0 SA for initial connection handshake.
/// </summary>
public sealed class KSADatabase
{
    private readonly ConcurrentDictionary<ushort, KSecurityAssociation> _saMap = new();

    public KSADatabase()
    {
        _saMap[0] = new KSecurityAssociation();
    }

    public KSecurityAssociation? GetSA(ushort spi)
        => _saMap.TryGetValue(spi, out var sa) ? sa : null;

    public bool Insert(ushort spi, KSecurityAssociation sa)
        => _saMap.TryAdd(spi, sa);

    public ushort CreateNewSA(out KSecurityAssociation sa)
    {
        sa = KSecurityAssociation.CreateRandom();
        ushort spi;
        do
        {
            spi = (ushort)Random.Shared.Next(1, ushort.MaxValue + 1);
        } while (!_saMap.TryAdd(spi, sa));
        return spi;
    }

    public bool Delete(ushort spi)
    {
        if (spi == 0) return false;
        return _saMap.TryRemove(spi, out _);
    }
}
