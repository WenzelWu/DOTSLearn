using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace KickBall
{
    public partial struct PlayerMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerMovement>();
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<Config>();
            
            // Most of the UnityEngine.Input are Burst compatible, but not all are.
            // If the OnUpdate, OnCreate, or OnDestroy methods needs to access managed objects or call methods
            // that aren't Burst-compatible, the [BurstCompile] attribute can be omitted.
            var horizontal = UnityEngine.Input.GetAxis("Horizontal");
            var vertical = UnityEngine.Input.GetAxis("Vertical");
            var input = new float3(horizontal, 0, vertical) * SystemAPI.Time.DeltaTime * config.PlayerSpeed;

            if (input.Equals(float3.zero)) return;
            
            var minDist = config.ObstacleRadius + 0.5f;
            var minDistSQ = minDist * minDist;
            
            // For every entity having a LocalTransform and Player component, a read-write reference to
            // the LocalTransform is assigned to 'playerTransform'. Player Component is a tag component, help to filter the entity
            foreach (var playerTrasform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<Player>())
            {
                var newPos = playerTrasform.ValueRO.Position + input;

                foreach (var obstacleTransform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Obstacle>())
                {
                    if (math.distancesq(newPos, obstacleTransform.ValueRO.Position) < minDistSQ)
                    {
                        newPos = playerTrasform.ValueRO.Position;
                        break;
                    }
                }
                
                playerTrasform.ValueRW.Position = newPos;
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}