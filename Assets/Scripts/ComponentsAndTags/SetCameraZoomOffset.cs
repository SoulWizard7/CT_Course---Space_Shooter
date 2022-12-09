
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ComponentsAndTags
{
    public class SetCameraZoomOffset : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
    {
        public GameObject CameraObject;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var cameraOffset = default(CameraZoomOffsetComponent);

            var offsetVector = CameraObject.transform.position;
            cameraOffset.Value = new float3(offsetVector.x, offsetVector.y, offsetVector.z);

            quaternion rotation = CameraObject.transform.localRotation;
            cameraOffset.Rotation = rotation;

            dstManager.AddComponentData(entity, cameraOffset);
        }
    }
}