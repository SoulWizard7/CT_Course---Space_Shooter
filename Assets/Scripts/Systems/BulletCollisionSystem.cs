
using ComponentsAndTags;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateBefore(typeof(AsteroidsDestructionSystem))]  
    public partial class BulletCollisionSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem m_EndSimEcb;
        private EntityQuery m_bulletQuery;
        
        protected override void OnCreate()
        {
            m_EndSimEcb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

            m_bulletQuery = GetEntityQuery(ComponentType.ReadWrite<BulletTag>());
            
            RequireSingletonForUpdate<GameSettingsComponent>();
            RequireSingletonForUpdate<PlayerTag>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = m_EndSimEcb.CreateCommandBuffer();
            var player = GetSingletonEntity<PlayerTag>();
            var playerPos = GetComponent<Translation>(player);
            
            NativeArray<Entity> bulletEntityArray = m_bulletQuery.ToEntityArray(Allocator.TempJob);
            
            // How can I make this multi-thread?

            Entities.WithNone<DestroyTag>().ForEach((Entity asteroid, int entityInQueryIndex, in AsteroidTag asteroids,
                in Translation position) =>
            {
                if (math.distancesq(playerPos.Value, position.Value) < 30000) // this line +~40fps w/ 60000 asteroids
                {
                    for (int i = 0; i < bulletEntityArray.Length; i++)
                    {
                        float3 bp = GetComponent<Translation>(bulletEntityArray[i]).Value;
                        
                        if (math.distancesq(position.Value, bp) < 8)
                        {
                            //commandBuffer.AddComponent(entityInQueryIndex, asteroid, new DestroyTag());
                            commandBuffer.AddComponent(asteroid, new DestroyTag());
                            commandBuffer.AddComponent(bulletEntityArray[i], new DestroyTag());
                        }
                    }
                }
            }).Schedule();

            bulletEntityArray.Dispose(Dependency);
            
            m_EndSimEcb.AddJobHandleForProducer(Dependency);
        }
    }
}