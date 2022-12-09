using Unity.Entities;
namespace ComponentsAndTags
{
    [GenerateAuthoringComponent]
    public struct PlayerAuthoringComponent : IComponentData
    {
        public Entity Prefab;
    }
}