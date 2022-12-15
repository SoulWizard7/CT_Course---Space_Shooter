using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using System.Diagnostics;
using ComponentsAndTags;
using UnityEngine;

public partial class AsteroidSpawnSystem : SystemBase
{
    private EntityQuery m_AsteroidQuery;
    private BeginSimulationEntityCommandBufferSystem m_BeginSimECB;
    private EntityQuery m_GameSettingsQuery;

    private Entity m_AsteroidPrefab;
    private bool spawn;

    protected override void OnCreate()
    {
        m_AsteroidQuery = GetEntityQuery(ComponentType.ReadWrite<AsteroidTag>());
        m_GameSettingsQuery = GetEntityQuery(ComponentType.ReadWrite<GameSettingsComponent>());

        m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        spawn = true;

        RequireForUpdate(m_GameSettingsQuery);
    }
    
    protected override void OnUpdate()
    {
        if (m_AsteroidPrefab == Entity.Null)
        {
            m_AsteroidPrefab = GetSingleton<AsteroidAuthoringComponent>().Prefab;
            return;
        }
        
        if (!spawn) return;
        spawn = false;
        
        var settings = GetSingleton<GameSettingsComponent>();
        var commandBuffer = m_BeginSimECB.CreateCommandBuffer();

        //This provides the current amount of Asteroids in the EntityQuery
        var count = m_AsteroidQuery.CalculateEntityCountWithoutFiltering();
       
        var asteroidPrefab = m_AsteroidPrefab;
        var random = new Unity.Mathematics.Random((uint)Stopwatch.GetTimestamp());

        Job
        .WithCode(() => {
            for (int i = count; i < settings.numAsteroids; ++i)
            {
                // ReSharper disable PossibleLossOfFraction
                Translation position;
                
                do
                {
                    var xPos = random.NextFloat(-1f*(settings.levelWidth/2), settings.levelWidth/2);
                    var zPos = random.NextFloat(-1f*(settings.levelDepth/2), settings.levelDepth/2);
                
                    position = new Translation{Value = new float3(xPos, 0, zPos)};
                } while (math.distance(float3.zero, position.Value) <= 200); 

                var entity = commandBuffer.Instantiate(asteroidPrefab);
                commandBuffer.SetComponent(entity, position);

                var velocity = new Vector3(0,0,0) - new Vector3(position.Value.x, 0 , position.Value.z);
                velocity.Normalize();
                velocity *= random.NextFloat(1f, settings.asteroidVelocity);
                
                var vel = new VelocityComponent{Value = velocity};
                
                commandBuffer.SetComponent(entity, vel);
            }
        }).Schedule();

        m_BeginSimECB.AddJobHandleForProducer(Dependency);
    }
}
