using Unity.Entities;
namespace ComponentsAndTags
{
    [GenerateAuthoringComponent]
    public struct BulletAuthoringComponent : IComponentData
    {
        public Entity Prefab;
    }
}