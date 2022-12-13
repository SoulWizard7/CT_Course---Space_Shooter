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
    //This will be our query for Asteroids
    private EntityQuery m_AsteroidQuery;

    //We will use the BeginSimulationEntityCommandBufferSystem for our structural changes
    private BeginSimulationEntityCommandBufferSystem m_BeginSimECB;

    //This will be our query to find GameSettingsComponent data to know how many and where to spawn Asteroids
    private EntityQuery m_GameSettingsQuery;

    //This will save our Asteroid prefab to be used to spawn Asteroids
    private Entity m_Prefab;

    protected override void OnCreate()
    {
        //This is an EntityQuery for our Asteroids, they must have an AsteroidTag
        m_AsteroidQuery = GetEntityQuery(ComponentType.ReadWrite<AsteroidTag>());

        //This will grab the BeginSimulationEntityCommandBuffer system to be used in OnUpdate
        m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

        //This is an EntityQuery for the GameSettingsComponent which will drive how many Asteroids we spawn
        m_GameSettingsQuery = GetEntityQuery(ComponentType.ReadWrite<GameSettingsComponent>());

        //This says "do not go to the OnUpdate method until an entity exists that meets this query"
        //We are using GameObjectConversion to create our GameSettingsComponent so we need to make sure 
        //The conversion process is complete before continuing
        RequireForUpdate(m_GameSettingsQuery);
    }
    
    protected override void OnUpdate()
    {
        if (m_Prefab == Entity.Null)
        {
            m_Prefab = GetSingleton<AsteroidAuthoringComponent>().Prefab;
            return;
        }
        var settings = GetSingleton<GameSettingsComponent>();
        var commandBuffer = m_BeginSimECB.CreateCommandBuffer();

        #region StopSpawner 
        // funky way to stop spawning
           /*
        if (!settings.isSpawning)
        {
            return;
                
        }
        Entity v = GetSingletonEntity<GameSettingsComponent>();
        commandBuffer.SetComponent(v, new GameSettingsComponent()
        {
            asteroidVelocity = settings.asteroidVelocity,
            playerForce = settings.playerForce,
            bulletVelocity = settings.bulletVelocity,
            numAsteroids = settings.numAsteroids,
            levelWidth = settings.levelWidth,
            levelHeight = settings.levelHeight,
            levelDepth = settings.levelDepth,
            mouseSpeed = settings.mouseSpeed,  
            isSpawning = false
        });*/
        #endregion

        //This provides the current amount of Asteroids in the EntityQuery
        var count = m_AsteroidQuery.CalculateEntityCountWithoutFiltering();
       
        var asteroidPrefab = m_Prefab;
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

                //var velocity = new Vector3(random.NextFloat(-1f, 1f), 0, random.NextFloat(-1f, 1f));
                var velocity = new Vector3(0,0,0) - new Vector3(position.Value.x, 0 , position.Value.z);
                velocity.Normalize();
                velocity *= random.NextFloat(0.01f, settings.asteroidVelocity);
                
                var vel = new VelocityComponent{Value = velocity};
                
                commandBuffer.SetComponent(entity, vel);
            }
        }).Schedule();

        //This will add our dependency to be played back on the BeginSimulationEntityCommandBuffer
        m_BeginSimECB.AddJobHandleForProducer(Dependency);
        
        
        
        
    }

    protected override void OnDestroy()
    {
        
    }
}
