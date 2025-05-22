using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Librarian.Common.Models.Yesod.BuiltinActions
{
    public partial class KeywordFilter
    {
        [JsonPropertyName("or_list")]
        [Required]
        public ICollection<OrList> OrList { get; set; } = new List<OrList>();
    }

    public partial class OrList
    {
        [JsonPropertyName("and_list")]
        [Required]
        public ICollection<AndList> AndList { get; set; } = new List<AndList>();
    }

    public partial class AndList
    {
        [JsonPropertyName("field")]
        [Required(AllowEmptyStrings = true)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AndListField Field { get; set; }

        [JsonPropertyName("equation")]
        [Required(AllowEmptyStrings = true)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AndListEquation Equation { get; set; }

        [JsonPropertyName("value")]
        [Required(AllowEmptyStrings = true)]
        public string Value { get; set; } = string.Empty;
    }

    public enum AndListField
    {
        [EnumMember(Value = "author")]
        Author = 0,

        [EnumMember(Value = "title")]
        Title = 1,

        [EnumMember(Value = "description")]
        Description = 2,

        [EnumMember(Value = "content")]
        Content = 3,
    }

    public enum AndListEquation
    {
        [EnumMember(Value = "equal")]
        Equal = 0,

        [EnumMember(Value = "not_equal")]
        NotEqual = 1,

        [EnumMember(Value = "contain")]
        Contain = 2,

        [EnumMember(Value = "not_contain")]
        NotContain = 3,

        [EnumMember(Value = "start_with")]
        StartWith = 4,

        [EnumMember(Value = "not_start_with")]
        NotStartWith = 5,

        [EnumMember(Value = "end_with")]
        EndWith = 6,

        [EnumMember(Value = "not_end_with")]
        NotEndWith = 7,
    }
}
