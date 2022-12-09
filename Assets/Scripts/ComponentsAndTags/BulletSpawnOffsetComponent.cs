using Unity.Entities;
using Unity.Mathematics;

namespace ComponentsAndTags
{
    public struct BulletSpawnOffsetComponent : IComponentData
    {
        public float3 Value;
        public float3 Value2;
    }
}