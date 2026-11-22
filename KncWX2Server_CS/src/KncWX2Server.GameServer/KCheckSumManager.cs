using System.Xml.Linq;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of Common/X2Data/XSLCheckSumManager.{h,cpp}:
/// loads XML PatchInfo file checksums (normalized uppercase filenames without path)
/// and validates client-reported file CRCs against the server manifest.
/// </summary>
public sealed class KCheckSumManager
{
    private readonly Dictionary<string, string> _checkSums = new(StringComparer.OrdinalIgnoreCase);

    public bool IsLoaded { get; private set; }
    public int EntryCount => _checkSums.Count;

    public bool LoadFromXmlContent(string xmlContent)
    {
        if (string.IsNullOrWhiteSpace(xmlContent)) return false;
        var doc = XDocument.Parse(xmlContent);
        var fileElem = doc.Element("PatchInfo")?.Element("File");
        if (fileElem == null) return false;

        _checkSums.Clear();
        foreach (var item in fileElem.Elements("Item"))
        {
            string? fullName = item.Attribute("Name")?.Value;
            string? checkSum = item.Attribute("CheckSum")?.Value;
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(checkSum))
                continue;

            int slashIdx = Math.Max(fullName.LastIndexOf('\\'), fullName.LastIndexOf('/'));
            string fileName = (slashIdx < 0 ? fullName : fullName[(slashIdx + 1)..]).ToUpperInvariant();
            _checkSums.TryAdd(fileName, checkSum);
        }

        IsLoaded = true;
        return true;
    }

    public bool AddCheckSum(string fileName, string checkSum)
        => _checkSums.TryAdd(fileName.ToUpperInvariant(), checkSum);

    public bool GetIsValidCheckSum(IReadOnlyDictionary<string, string> clientCheckSums)
    {
        if (!IsLoaded) return true;
        foreach (var kv in clientCheckSums)
        {
            if (_checkSums.TryGetValue(kv.Key.ToUpperInvariant(), out string? expected))
            {
                if (!string.Equals(kv.Value, expected, StringComparison.OrdinalIgnoreCase))
                    return false;
            }
        }
        return true;
    }
}
