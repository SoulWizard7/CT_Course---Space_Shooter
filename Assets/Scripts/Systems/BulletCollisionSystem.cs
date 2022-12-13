
using ComponentsAndTags;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    
    public partial class BulletCollisionSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem m_EndSimEcb;
        private EntityQuery query;
        
        protected override void OnCreate()
        {
            m_EndSimEcb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            RequireSingletonForUpdate<GameSettingsComponent>();
            RequireSingletonForUpdate<PlayerTag>();
        }

        protected override void OnUpdate()
        {
            NativeList<Entity> bulletEntity = new NativeList<Entity>(Allocator.Temp);

            var commandBuffer = m_EndSimEcb.CreateCommandBuffer();
            var player = GetSingletonEntity<PlayerTag>();
            var playerPos = GetComponent<Translation>(player);
            
            Entities.ForEach((Entity Entity, ref BulletTag bulletTag, ref Translation position) =>
            {
                bulletEntity.Add(Entity);

            }).Run();

            NativeArray<Entity> bulletEntityArray = bulletEntity.ToArray(Allocator.Temp);
            
            Entities.ForEach((Entity entity, int entityInQueryIndex, ref AsteroidTag Asteroids,
                in Translation position) =>
            {
                if (math.distancesq(playerPos.Value, position.Value) < 30000) // this line +~40fps w/ 60000 asteroids
                {
                    for (int i = 0; i < bulletEntityArray.Length; i++)
                    {
                        float3 bp = GetComponent<Translation>(bulletEntityArray[i]).Value;
                        
                        if (math.distancesq(position.Value, bp) < 8)
                        {
                            commandBuffer.AddComponent(entity, new DestroyTag());
                            commandBuffer.AddComponent(bulletEntityArray[i], new DestroyTag());
                        }
                    }
                }
            }).Run();
            
            m_EndSimEcb.AddJobHandleForProducer(Dependency);
        }
    }
}