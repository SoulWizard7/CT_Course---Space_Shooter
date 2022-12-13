using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using ComponentsAndTags;
using AuthoringAndMono;

namespace Systems
{
    //[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    //[UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
    
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateAfter(typeof(AsteroidsDestructionSystem))]  
    public partial class ApplySunDamageSystem : SystemBase
    {
        protected override void OnUpdate()
        {


            Entities.ForEach((Entity entity, ref SunHealthComponent health, ref DynamicBuffer<SunDamageBufferElement>buffer) =>
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    
                    health.Value += buffer[i].Value;
                    buffer.ElementAt(i).Value = 0;
                }
                
                //Debug.Log(buffer.Length);
                
                buffer.Clear();
                
            }).Run();
            
            
            
        }
    }
}