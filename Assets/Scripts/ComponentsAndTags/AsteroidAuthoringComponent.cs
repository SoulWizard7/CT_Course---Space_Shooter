using Unity.Entities;

namespace ComponentsAndTags
{
    [GenerateAuthoringComponent]
    public struct AsteroidAuthoringComponent : IComponentData
    {
        public Entity Prefab;
    }
}
