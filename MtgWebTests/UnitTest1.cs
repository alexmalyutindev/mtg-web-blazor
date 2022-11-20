using MtgWeb.Core;
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

    [Test]
    public async Task PrefabSerialization()
    {
        var prefabJson = @"
{
    'Name': 'Player',
    'Transform': {
        'Position': {
            'X': 0.0,
            'Y': 0.0,
            'Z': 5.0
        }
    },
    'Components': [
        {
            '$type': 'MtgWeb.Core.PlayerController, MtgWeb'
        },
        {
            '$type': 'MtgWeb.Core.Camera, MtgWeb'
        }
    ]
}";
        var sceneJson = @"{'Root': [
{'PrefabName' : 'Player'},
{'Name': 'Other'}
]}";

        Resources.Init(new HttpClient());
        var scene = JsonConvert.DeserializeObject<Scene>(sceneJson);
        for (var index = 0; index < scene.Root.Length; index++)
        {
            var entity = scene.Root[index];
            if (entity is PrefabEntity prefab)
            {
                Console.WriteLine("Loading prefab: " + prefab.PrefabName);
                scene.Root[index] = await Resources.LoadPrefab(prefab.PrefabName);
            }
        }

        Console.WriteLine(JsonConvert.SerializeObject(scene, Formatting.Indented));
    }
}