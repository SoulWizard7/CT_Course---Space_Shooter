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
        private EndSimulationEntityCommandBufferSystem m_EndSimEcb;    

        protected override void OnCreate()
        {
            m_EndSimEcb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = m_EndSimEcb.CreateCommandBuffer().AsParallelWriter();

            Entities
                .WithAll<DestroyTag>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    commandBuffer.DestroyEntity(entityInQueryIndex, entity);

                }).ScheduleParallel();

            //We add the dependencies of these jobs to the EndSimulationEntityCommandBufferSystem
            //that will be playing back the structural changes recorded in this system
            m_EndSimEcb.AddJobHandleForProducer(Dependency);
        }
    }
}