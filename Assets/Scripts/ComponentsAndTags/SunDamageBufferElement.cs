using Unity.Entities;

namespace ComponentsAndTags
{
    [GenerateAuthoringComponent]
    public struct SunDamageBufferElement : IBufferElementData
    {
        public int Value;
    }
}