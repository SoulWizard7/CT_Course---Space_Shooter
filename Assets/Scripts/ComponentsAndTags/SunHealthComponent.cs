using Unity.Entities;

namespace ComponentsAndTags
{
    [GenerateAuthoringComponent]
    public struct SunHealthComponent : IComponentData
    {
        public int Value;
    }
}