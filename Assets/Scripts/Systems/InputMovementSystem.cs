using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using ComponentsAndTags;

namespace Systems
{
    public partial class InputMovementSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<GameSettingsComponent>();
        }

        protected override void OnUpdate()
        {
            var gameSettings = GetSingleton<GameSettingsComponent>();
            var deltaTime = Time.DeltaTime;
    
            //we will control thrust with WASD"
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

            Entities.ForEach((Entity entity, ref PlayerTag playerTag, ref Rotation rotation, ref VelocityComponent velocity) =>
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
                
                
                
                if (mouseX != 0)
                {
                    Quaternion currentQuaternion = rotation.Value; 
                    float yaw = currentQuaternion.eulerAngles.y;
    
                    //MOVING WITH MOUSE
                    yaw += gameSettings.mouseSpeed * mouseX;
                    Quaternion newQuaternion = Quaternion.identity;
                    newQuaternion.eulerAngles = new Vector3(0,yaw, 0);
                    rotation.Value = newQuaternion;
                }
            }).Schedule();
        }
    }
}