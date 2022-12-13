using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using ComponentsAndTags;
using AuthoringAndMono;

namespace Systems
{
    
    public partial class SunBurstSystem : SystemBase
    {
        // This script handles the sun burst (once per minute) and also handles the damage that was added to the sun.
        
        
        private float m_timeToSunBurst = 55;
        private float m_timer = 0;
        private float m_burstRadius = 5;
        
        private EndSimulationEntityCommandBufferSystem m_EndSimEcb;
        
        protected override void OnCreate()
        {
            m_EndSimEcb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            var sunBurst = false;
            var burstRadius = 5f;

            m_timer += deltaTime;
            
            if (m_timer >= m_timeToSunBurst)
            {
                sunBurst = true;
                //Debug.Log("SunBurst");
            }

            if (sunBurst)
            {
                m_burstRadius += deltaTime * 50;
                burstRadius = m_burstRadius;
                
                if (m_timer >= m_timeToSunBurst + 5)
                {
                    m_timer = 0;
                    m_burstRadius = 5;
                }
            }

            GameValuesMono.timerSingleton = m_timer;
            
            //if (!sunBurst) { return; }

            var commandBuffer = m_EndSimEcb.CreateCommandBuffer().AsParallelWriter();
            var sun = GetSingletonEntity<SunHealthComponent>();

            GameValuesMono.sunHealth = GetComponent<SunHealthComponent>(sun).Value;
            
            //DynamicBuffer<SunDamageBufferElement> buffer = EntityManager.GetBuffer<SunDamageBufferElement>(sun);

            Entities.WithAll<AsteroidTag>().ForEach((Entity entity, int entityInQueryIndex, in Translation position) =>
            {
                if (math.distance(position.Value, float3.zero) < burstRadius)
                {
                    if (sunBurst)
                    {
                        commandBuffer.AddComponent(entityInQueryIndex, entity, new DestroyTag());
                    }
                    else
                    {
                        commandBuffer.AddComponent(entityInQueryIndex, entity, new DestroyTag());
                        
                        var damage = new SunDamageBufferElement { Value = 1 };
                        //buffer.Add(damage);
                        commandBuffer.AppendToBuffer(entityInQueryIndex, sun, damage);
                    }
                }

            }).ScheduleParallel();
            
            m_EndSimEcb.AddJobHandleForProducer(Dependency);

        }
    }
}