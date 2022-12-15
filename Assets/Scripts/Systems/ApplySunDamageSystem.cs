using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using ComponentsAndTags;
using AuthoringAndMono;

namespace Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateAfter(typeof(AsteroidsDestructionSystem))]  
    public partial class ApplySunDamageSystem : SystemBase
    {
        //private uiUpdate UIUpdate;

        protected override void OnCreate()
        {
            //UIUpdate = GameObject.Find("Canvas").GetComponent<uiUpdate>();
            
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref SunHealthComponent health, ref DynamicBuffer<SunDamageBufferElement>buffer) =>
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    health.Value += buffer[i].Value;
                }
                buffer.Clear();
            }).Run();
            
            GameValuesMono.sunHealth = GetSingleton<SunHealthComponent>().Value;
            //UIUpdate.UpdateUI(GetSingleton<SunHealthComponent>().Value);
        }
    }
}