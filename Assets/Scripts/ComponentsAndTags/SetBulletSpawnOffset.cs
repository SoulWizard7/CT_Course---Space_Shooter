using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ComponentsAndTags
{
    public class SetBulletSpawnOffset : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
    {
        public GameObject bulletSpawn;
        public GameObject bulletSpawn2;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var bulletOffset = default(BulletSpawnOffsetComponent);

            var offsetVector = bulletSpawn.transform.position;
            var offsetVector2 = bulletSpawn2.transform.position;
            bulletOffset.Value = new float3(offsetVector.x, offsetVector.y, offsetVector.z);
            bulletOffset.Value2 = new float3(offsetVector2.x, offsetVector2.y, offsetVector2.z);

            dstManager.AddComponentData(entity, bulletOffset);
        }
    }
}