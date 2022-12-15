using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using ComponentsAndTags;

namespace Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class AsteroidsDestructionSystem : SystemBase
    {
        // THIS SCRIPT IS CALLED ASTEROIDS DESTRUCTION SYSTEM, BUT DESTORYS EVERYTHING WITH THE <DestroyTag> component (bullets & asteroids) 

        private EndSimulationEntityCommandBufferSystem m_EndSimEcb;    

        protected override void OnCreate()
        {
            m_EndSimEcb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = m_EndSimEcb.CreateCommandBuffer().AsParallelWriter();

            Entities.WithAll<DestroyTag>().ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    commandBuffer.DestroyEntity(entityInQueryIndex, entity);

                }).ScheduleParallel();

            m_EndSimEcb.AddJobHandleForProducer(Dependency);
        }
    }
}