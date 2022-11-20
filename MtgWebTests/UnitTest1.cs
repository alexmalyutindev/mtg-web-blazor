using MtgWeb.Core.Render;
using MtgWeb.Core.Serialization;
using Newtonsoft.Json;

namespace MtgWebTests;

public class Tests
{
    [SetUp]
    public void Setup() { }

    [Test]
    public void MaterialSerialization()
    {
        var json = @"
{ 
    'Uniforms' : {
        'u_TestFloat' : 0.5,
        'u_TestInt' : 5
    },
    'Shader' : { 'Name' : 'Lit' }
}";
        var materialProperty = JsonConvert.DeserializeObject<Material>(json);
        Console.WriteLine(string.Join("\n", materialProperty.Uniforms));
        Console.WriteLine(JsonConvert.SerializeObject(materialProperty, Formatting.Indented));
    }
}