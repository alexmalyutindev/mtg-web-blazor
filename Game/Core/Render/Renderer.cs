using Newtonsoft.Json;

namespace MtgWeb.Core.Render;

public class Renderer : Component
{
    public MeshType MeshType;

    [JsonConverter(typeof(ShaderConverter))]
    public Shader Shader;

    public override async void Start()
    {
        await Shader.Load();
    }
}

public class ShaderConverter : JsonConverter<Shader>
{
    public override bool CanRead => true;
    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, Shader? value, JsonSerializer serializer) { }

    public override Shader? ReadJson(
        JsonReader reader,
        Type objectType,
        Shader? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer
    )
    {
        var name = serializer.Deserialize<string>(reader);
        var shader = Shader.Create(name);

        return shader;
    }
}