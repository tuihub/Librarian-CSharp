using System.Text.Json.Serialization;

namespace Librarian.Common.Models.Yesod.BuiltinActions;

public class SimpleKeywordFilter
{
    [JsonPropertyName("title_include")] public ICollection<string> TitleInclude { get; } = new List<string>();

    [JsonPropertyName("title_exclude")] public ICollection<string> TitleExclude { get; } = new List<string>();

    [JsonPropertyName("content_include")] public ICollection<string> ContentInclude { get; } = new List<string>();

    [JsonPropertyName("content_exclude")] public ICollection<string> ContentExclude { get; } = new List<string>();
}