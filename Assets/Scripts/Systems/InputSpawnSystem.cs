using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Burst;
using ComponentsAndTags;

namespace Systems
{
    public partial class InputSpawnSystem : SystemBase
    {
        //This will be our query for Players
        private EntityQuery m_PlayerQuery;

        //We will use the BeginSimulationEntityCommandBufferSystem for our structural changes
        private BeginSimulationEntityCommandBufferSystem m_BeginSimECB;

        //This will save our Player prefab to be used to spawn Players
        private Entity PlayerPrefab;
        private Entity BulletPrefab;
        private Entity CameraPrefab;

        //We are going to use this to rate limit bullets per second
        //We could have included this in the game settings, no "ECS reason" not to
        private float m_PerSecond = 10f;
        private float m_NextTime = 0;

        protected override void OnCreate()
        {
            //This is an EntityQuery for our Players, they must have an PlayerTag
            m_PlayerQuery = GetEntityQuery(ComponentType.ReadWrite<PlayerTag>());

            //This will grab the BeginSimulationEntityCommandBuffer system to be used in OnUpdate
            m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

            //We need the GameSettingsComponent to grab the bullet velocity
            //When there is only 1 instance of a component (like GamesSettingsComponent) we can use "RequireSingletonForUpdate"
            RequireSingletonForUpdate<GameSettingsComponent>();
        }

        protected override void OnUpdate()
        {
            if (PlayerPrefab == Entity.Null || BulletPrefab == Entity.Null || CameraPrefab == Entity.Null)
            {
                PlayerPrefab = GetSingleton<PlayerAuthoringComponent>().Prefab;
                BulletPrefab = GetSingleton<BulletAuthoringComponent>().Prefab;
                CameraPrefab = GetSingleton<CameraAuthoringComponent>().Prefab;
                return;
            }

            byte shoot;
            shoot = 0;
            var playerCount = m_PlayerQuery.CalculateEntityCountWithoutFiltering();

            if (Input.GetKey("space"))
            {
                shoot = 1;
            }

            if (playerCount < 1)
            {
                EntityManager.Instantiate(PlayerPrefab);
                EntityManager.Instantiate(CameraPrefab);
                return;
            }

            var commandBuffer = m_BeginSimECB.CreateCommandBuffer();
            
            //We must declare our local variables before the .ForEach()
            var gameSettings = GetSingleton<GameSettingsComponent>();
            var bulletPrefab = BulletPrefab;

            //we are going to implement rate limiting for shooting
            var canShoot = false;
            if (UnityEngine.Time.time >= m_NextTime)
            {
                canShoot = true;
                m_NextTime += (1 / m_PerSecond);
            }

            Entities.ForEach((Entity entity, in PipeSwitchComponent pipeSwitchComponent, in Translation position,
                in Rotation rotation, in VelocityComponent velocity, in BulletSpawnOffsetComponent bulletOffset) =>
            {
                //If we don't have space bar pressed we don't have anything to do
                if (shoot != 1 || !canShoot)
                {
                    return;
                }

                // We create the bullet here
                var bulletEntity = commandBuffer.Instantiate(bulletPrefab);

                //we set the bullets position as the player's position + the bullet spawn offset
                //math.mul(rotation.Value,bulletOffset.Value) finds the position of the bullet offset in the given rotation
                //think of it as finding the LocalToParent of the bullet offset (because the offset needs to be rotated in the players direction)
                Translation newPosition;

                if (pipeSwitchComponent.Value)
                {
                    PipeSwitchComponent p = new PipeSwitchComponent {Value = false};
                    commandBuffer.SetComponent(entity, p);
                    newPosition = new Translation
                        {Value = position.Value + math.mul(rotation.Value, bulletOffset.Value).xyz};
                }
                else
                {
                    PipeSwitchComponent p = new PipeSwitchComponent {Value = true};
                    commandBuffer.SetComponent(entity, p);
                    newPosition = new Translation
                        {Value = position.Value + math.mul(rotation.Value, bulletOffset.Value2).xyz};
                }

                commandBuffer.SetComponent(bulletEntity, newPosition);

                // bulletVelocity * math.mul(rotation.Value, new float3(0,0,1)).xyz) takes linear direction of where facing and multiplies by velocity
                // adding to the players physics Velocity makes sure that it takes into account the already existing player velocity (so if shoot backwards while moving forwards it stays in place)
                var vel = new VelocityComponent
                {
                    Value = (gameSettings.bulletVelocity * math.mul(rotation.Value, new float3(0, 0, 1)).xyz) +
                            velocity.Value
                };

                commandBuffer.SetComponent(bulletEntity, vel);
            }).Run();

            m_BeginSimECB.AddJobHandleForProducer(Dependency);
        }
    }
}