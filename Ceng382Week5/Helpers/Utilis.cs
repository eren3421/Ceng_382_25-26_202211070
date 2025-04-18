using System.Text.Json;

namespace Ceng382Week5.Helpers
{
    public sealed class Utils
    {
        private static readonly Lazy<Utils> _instance = new(() => new Utils());

        public static Utils Instance => _instance.Value;

        private Utils() { }

        public string ExportToJson<T>(IEnumerable<T> data, List<string> selectedProperties = null)
        {
            if (selectedProperties == null || !selectedProperties.Any())
                return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

            var filteredData = data.Select(item =>
            {
                var obj = new Dictionary<string, object>();
                foreach (var prop in item.GetType().GetProperties())
                {
                    if (selectedProperties.Contains(prop.Name))
                    {
                        obj[prop.Name] = prop.GetValue(item);
                    }
                }
                return obj;
            });

            return JsonSerializer.Serialize(filteredData, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
// I created this page by using ChatGPT with prompt "Create a Utilis class for my .net core project.