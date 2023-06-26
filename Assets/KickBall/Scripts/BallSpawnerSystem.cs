using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = Unity.Mathematics.Random;

namespace KickBall
{
    // Ensure the balls get rendered in the correct position for the frame in which they're spawned.
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct BallSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Config>();
            state.RequireForUpdate<BallSpawner>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<Config>();
            
            if(!Input.GetKeyDown(KeyCode.Return)) return;
            
            Debug.Log("Spawned Ball");
            var rand = new Random(123);

            foreach (var playerTransform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Player>())
            {
                var ball = state.EntityManager.Instantiate(config.BallPrefab);
                state.EntityManager.SetComponentData(ball, new LocalTransform
                {
                    Position = playerTransform.ValueRO.Position,
                    Rotation = quaternion.identity,
                    Scale = 1
                });
                state.EntityManager.SetComponentData(ball, new Velocity
                {
                    Value = rand.NextFloat2Direction() * config.BallStartVelocity
                });
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}