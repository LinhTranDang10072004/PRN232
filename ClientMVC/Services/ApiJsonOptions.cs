using System.Text.Json;
using System.Text.Json.Serialization;
using ClientMVC.Models.Personal;

namespace ClientMVC.Services
{
    public static class ApiJsonOptions
    {
        public static readonly JsonSerializerOptions Default = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    public class ODataList<T>
    {
        [JsonPropertyName("value")]
        public List<T> Value { get; set; } = new();
    }
}
