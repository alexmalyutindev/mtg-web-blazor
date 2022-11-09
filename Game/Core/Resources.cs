using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MtgWeb.Core;

public class Resources
{
    private static HttpClient _httpClient;

    public static void Init(HttpClient client)
    {
        _httpClient = client;
    }
    
    public static async Task<Scene> LoadScene(String name)
    {
        var json = await _httpClient.GetStringAsync($"Resources/{name}.json");
        var scene = JsonConvert.DeserializeObject<Scene>(json, new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
        return scene;
    }
    
    public static async Task<(string, string)> LoadShader(String name)
    {
        var vertex = await _httpClient.GetStringAsync($"Resources/Shaders/{name}.vert");
        var fragment = await _httpClient.GetStringAsync($"Resources/Shaders/{name}.frag");

        return (vertex, fragment);
    }
}