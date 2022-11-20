using MtgWeb.Core.Serialization;
using Newtonsoft.Json;

namespace MtgWeb.Core;

public class Scene
{
    [JsonProperty(ItemConverterType = typeof(EntityConverter))]
    public Entity[] Root;

    public async Task PostLoad()
    {
        for (var index = 0; index < Root.Length; index++)
        {
            var entity = Root[index];
            if (entity is PrefabEntity prefab)
            {
                Console.WriteLine("Loading prefab: " + prefab.PrefabName);
                Root[index] = await Resources.LoadPrefab(prefab.PrefabName);
            }
        }
    }
}