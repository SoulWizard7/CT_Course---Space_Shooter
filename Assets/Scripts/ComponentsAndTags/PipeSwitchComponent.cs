using Unity.Entities;

namespace ComponentsAndTags
{
    [GenerateAuthoringComponent]
    public struct PipeSwitchComponent : IComponentData
    {
        public bool Value;
    }
}