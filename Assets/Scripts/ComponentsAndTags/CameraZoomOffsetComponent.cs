using Unity.Entities;
using Unity.Mathematics;


namespace ComponentsAndTags
{
    public struct CameraZoomOffsetComponent : IComponentData
    {
        public float3 Value;
        public quaternion Rotation;
    }
}