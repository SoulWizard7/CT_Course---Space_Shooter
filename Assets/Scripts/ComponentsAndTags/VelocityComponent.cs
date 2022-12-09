using Unity.Entities;
using Unity.Mathematics;

namespace ComponentsAndTags
{
    [GenerateAuthoringComponent]
    public struct VelocityComponent : IComponentData
    {
        public float3 Value;
    }
}