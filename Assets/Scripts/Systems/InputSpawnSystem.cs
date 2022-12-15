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
        private EntityQuery m_PlayerQuery;

        private BeginSimulationEntityCommandBufferSystem m_BeginSimECB;

        private Entity PlayerPrefab;
        private Entity BulletPrefab;
        private Entity CameraPrefab;

        private float m_PerSecond = 10f;
        private float m_NextTime = 0;

        protected override void OnCreate()
        {
            m_PlayerQuery = GetEntityQuery(ComponentType.ReadWrite<PlayerTag>());
            m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            
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

            var commandBuffer = m_BeginSimECB.CreateCommandBuffer();
            if (playerCount < 1)
            {
                // yes Im sorry I spawn the player and Camera here.
                
                var player = EntityManager.Instantiate(PlayerPrefab);
                Translation playerStart = new Translation {Value = new float3(0, 0, -40)};
                commandBuffer.SetComponent(player, playerStart);
                
                EntityManager.Instantiate(CameraPrefab);

                var sun = EntityManager.CreateEntity();
                commandBuffer.AddComponent<SunHealthComponent>(sun);
                commandBuffer.AddBuffer<SunDamageBufferElement>(sun);
                return;
            }

            var gameSettings = GetSingleton<GameSettingsComponent>();
            var bulletPrefab = BulletPrefab;
            m_PerSecond = gameSettings.bulletsPerSecond;

            var canShoot = false;
            if (UnityEngine.Time.time >= m_NextTime)
            {
                canShoot = true;
                m_NextTime += (1 / m_PerSecond);
            }

            Entities.ForEach((Entity entity, in PipeSwitchComponent pipeSwitchComponent, in Translation position,
                in Rotation rotation, in VelocityComponent velocity, in BulletSpawnOffsetComponent bulletOffset) =>
            {
                if (shoot != 1 || !canShoot)
                {
                    return;
                }

                var bulletEntity = commandBuffer.Instantiate(bulletPrefab);

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