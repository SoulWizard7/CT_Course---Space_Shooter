using Unity.Entities;
namespace ComponentsAndTags
{
    [GenerateAuthoringComponent]
    public struct CameraAuthoringComponent : IComponentData
    {
        public Entity Prefab;
    }
}