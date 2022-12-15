using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using ComponentsAndTags;

namespace Systems
{
    public partial class InputMovementSystem : SystemBase
    {
        private float m_zoom = 1000f;
        
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<GameSettingsComponent>();
        }

        protected override void OnUpdate()
        {
            var gameSettings = GetSingleton<GameSettingsComponent>();
            var deltaTime = Time.DeltaTime;
    
            // WASD controls
            byte right, left, thrust, reverseThrust;
            right = left = thrust = reverseThrust = 0;

            float mouseX = Input.GetAxis("Mouse X");
    
            if (Input.GetKey("d"))
            {
                right = 1;
            }
            if (Input.GetKey("a"))
            {
                left = 1;
            }
            if (Input.GetKey("w"))
            {
                thrust = 1;
            }
            if (Input.GetKey("s"))
            {
                reverseThrust = 1;
            }
            
            if (Input.GetMouseButton(1))
            {
                m_zoom += deltaTime * 70;
                if (m_zoom > 1000) m_zoom = 1000;
            }
            else
            {
                if (m_zoom > 1) 
                {
                    m_zoom -= deltaTime * 200;
                }
                else
                {
                    m_zoom = 0.99f;
                }
            }

            float zoomOffset = m_zoom;
            float3 newCameraPos = float3.zero;
            float3 playerPos = float3.zero;
            quaternion cameraRot = quaternion.identity;

            Entities.ForEach((Entity entity, ref PlayerTag playerTag, ref Rotation rotation, ref VelocityComponent velocity, ref Translation position, ref CameraZoomOffsetComponent cameraZoomOffsetComponent) =>
            {
                float3 dir = float3.zero;
                
                if (right == 1)
                {   
                    //velocity.Value += (math.mul(rotation.Value, new float3(1,0,0)).xyz) * gameSettings.playerForce * deltaTime;
                    dir += new float3(1, 0, 0);
                }
                if (left == 1)
                {   
                    //velocity.Value += (math.mul(rotation.Value, new float3(-1,0,0)).xyz) * gameSettings.playerForce * deltaTime;
                    dir += new float3(-1, 0, 0);
                }
                if (thrust == 1)
                {   
                    //velocity.Value += (math.mul(rotation.Value, new float3(0,0,1)).xyz) * gameSettings.playerForce * deltaTime;
                    dir += new float3(0, 0, 1);
                }
                if (reverseThrust == 1)
                {   
                    //velocity.Value += (math.mul(rotation.Value, new float3(0,0,-1)).xyz) * gameSettings.playerForce * deltaTime;
                    dir += new float3(0, 0, -1);
                }

                math.normalize(dir);
                math.clamp(dir, new float3(-1, -1, -1), new float3(1, 1, 1));
                velocity.Value += math.mul(rotation.Value, dir).xyz * gameSettings.playerForce * deltaTime;

                //MOUSE LOOK
                
                Quaternion currentQuaternion = rotation.Value; 
                float yaw = currentQuaternion.eulerAngles.y;
    
                yaw += gameSettings.mouseSpeed * mouseX;
                Quaternion newQuaternion = Quaternion.identity;
                newQuaternion.eulerAngles = new Vector3(0,yaw, 0);
                rotation.Value = newQuaternion;

                newQuaternion.eulerAngles = new Vector3(60, yaw, 0);
                cameraRot = newQuaternion;

                Translation n = new Translation
                {
                    Value = position.Value + math.mul(rotation.Value, cameraZoomOffsetComponent.Value).xyz
                };

                newCameraPos = n.Value;
                playerPos = position.Value;

            }).Run();
            
            //Camera pos, rot and zoom
            Entities.WithAll<CameraTag>().ForEach((Entity entity, ref Translation position, ref Rotation rotation) =>
            {
                rotation.Value = cameraRot;
                
                float3 dir = math.normalize(newCameraPos - playerPos);
                newCameraPos += dir * zoomOffset;
                position.Value = newCameraPos;
                
            }).Run();
            
        }
    }
}