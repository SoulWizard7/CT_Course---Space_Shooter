
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
            NativeList<float3> bullets = new NativeList<float3>(Allocator.Temp);
            //NativeList<Entity> bulletEntity = new NativeList<Entity>(Allocator.Temp);

            var commandBuffer = m_EndSimEcb.CreateCommandBuffer();
            var player = GetSingletonEntity<PlayerTag>();
            var playerPos = GetComponent<Translation>(player);
            var playerRot = GetComponent<Rotation>(player);
            
            Entities.ForEach((Entity Entity, ref BulletTag bulletTag, ref Translation position) =>
            {
                //A.Add(Entity);
                bullets.Add(position.Value);

            }).Run();

            NativeArray<float3> b = bullets.ToArray(Allocator.Temp);
            
            Entities.ForEach((Entity entity, int entityInQueryIndex, ref AsteroidTag Asteroids,
                ref Translation position) =>
            {
                if (math.distancesq(playerPos.Value, position.Value) < 30000) // this line +~40fps w/ 60000 asteroids
                {
                    for (int i = 0; i < b.Length; i++)
                    {
                        if (math.distancesq(position.Value, b[i]) < 2)
                        {
                            //Debug.Log("hit");
                            commandBuffer.AddComponent(entity, new DestroyTag());
                        }
                    }
                }
            }).Run();
            
            m_EndSimEcb.AddJobHandleForProducer(Dependency);
        }
    }
}