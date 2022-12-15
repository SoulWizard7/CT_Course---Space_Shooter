using Unity.Entities;
using ComponentsAndTags;
namespace Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateBefore(typeof(AsteroidsDestructionSystem))]  
    public partial class BulletAgeSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem m_EndSimEcb;

        protected override void OnCreate()
        {
            m_EndSimEcb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = m_EndSimEcb.CreateCommandBuffer().AsParallelWriter();
            var deltaTime = Time.DeltaTime;

            Entities.ForEach((Entity entity, int entityInQueryIndex, ref BulletAgeComponent age) =>
            {
                age.age += deltaTime;
                if (age.age > age.maxAge)
                {
                    commandBuffer.AddComponent(entityInQueryIndex,entity, new DestroyTag());
                    //commandBuffer.DestroyEntity(entityInQueryIndex, entity);
                }

            }).ScheduleParallel();
            
            m_EndSimEcb.AddJobHandleForProducer(Dependency);
        }
    }
}