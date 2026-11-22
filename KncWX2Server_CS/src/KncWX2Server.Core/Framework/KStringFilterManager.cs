namespace KncWX2Server.Core.Framework;

/// <summary>
/// Faithful C# port of Common/StringFilterManager.{h,cpp} (SERV_STRING_FILTER_USING_DB):
/// nickname, post-word, and chat/note profanity filtering with ReleaseTick tracking.
/// </summary>
public sealed class KStringFilterManager
{
    public enum FilterType
    {
        FtWord = 0,
        FtNickName = 1,
        FtNickNameWord = 2,
        FtPostWord = 3,
    }

    private readonly List<string> _banNickNames = ["gm", "admin", "operator", "elsword"];
    private readonly List<string> _banNickNameWords = ["gm", "admin"];
    private readonly List<string> _banWords = ["fuck", "shit", "hacktool"];
    private readonly List<string> _banPostWords = ["gold_sell", "phishing"];
    private readonly object _sync = new();

    public int ReleaseTick { get; set; }

    public void SetStringFilter(IEnumerable<(FilterType type, string word)> entries)
    {
        lock (_sync)
        {
            _banNickNames.Clear();
            _banNickNameWords.Clear();
            _banWords.Clear();
            _banPostWords.Clear();
            foreach (var (t, word) in entries)
            {
                if (string.IsNullOrWhiteSpace(word)) continue;
                string upper = word.Trim().ToUpperInvariant();
                switch (t)
                {
                    case FilterType.FtNickName: _banNickNames.Add(upper); break;
                    case FilterType.FtNickNameWord: _banNickNameWords.Add(upper); break;
                    case FilterType.FtPostWord: _banPostWords.Add(upper); break;
                    default: _banWords.Add(upper); break;
                }
            }
        }
    }

    public bool CheckIsValidString(FilterType filterType, string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        string upper = input.ToUpperInvariant();
        lock (_sync)
        {
            if (filterType == FilterType.FtNickName)
            {
                if (_banNickNames.Any(b => string.Equals(upper, b, StringComparison.Ordinal)))
                    return false;
                if (_banNickNameWords.Any(b => upper.Contains(b, StringComparison.Ordinal)))
                    return false;
                if (_banWords.Any(b => upper.Contains(b, StringComparison.Ordinal)))
                    return false;
                return true;
            }
            if (filterType == FilterType.FtPostWord)
                return !_banPostWords.Any(b => upper.Contains(b, StringComparison.Ordinal));

            return !_banWords.Any(b => upper.Contains(b, StringComparison.Ordinal));
        }
    }

    public string FilteringChatString(string input, char replacement = '*')
        => FilterWithList(input, _banWords, replacement);

    public string FilteringNoteString(string input, char replacement = '*')
    {
        string step1 = FilterWithList(input, _banWords, replacement);
        return FilterWithList(step1, _banPostWords, replacement);
    }

    private static readonly string[] InvalidMarks =
    [
        "'", "\"", ";", "--", " ", "\u3000", "\t", ",", "`", "^", "&", "*", "+", "=", "|", "{", "}", ":", "?", "<", ">", "\u266A"
    ];

    /// <summary>
    /// Exact C++ KODBC::IsInvalidMarkIn guard (ERR_ODBC_00).
    /// </summary>
    public static bool IsInvalidMarkIn(string? data)
    {
        if (string.IsNullOrEmpty(data)) return false;
        foreach (string mark in InvalidMarks)
            if (data.Contains(mark, StringComparison.Ordinal))
                return true;
        return false;
    }

    /// <summary>
    /// Exact C++ KODBC::IsInvalidMarkInForLetter guard (forbids single quote in letters/wedding messages).
    /// </summary>
    public static bool IsInvalidMarkInForLetter(string? data)
        => !string.IsNullOrEmpty(data) && data.Contains('\'', StringComparison.Ordinal);

    private string FilterWithList(string input, List<string> list, char replacement)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        char[] chars = input.ToCharArray();
        string upper = input.ToUpperInvariant();
        lock (_sync)
        {
            foreach (string bad in list)
            {
                if (string.IsNullOrEmpty(bad)) continue;
                int idx = 0;
                while ((idx = upper.IndexOf(bad, idx, StringComparison.Ordinal)) >= 0)
                {
                    for (int i = 0; i < bad.Length; i++)
                        chars[idx + i] = replacement;
                    idx += bad.Length;
                }
            }
        }
        return new string(chars);
    }
}
