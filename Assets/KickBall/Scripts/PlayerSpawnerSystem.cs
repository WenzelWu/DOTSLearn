using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace KickBall
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    [UpdateAfter(typeof(ObstacleSpawnerSystem))]
    public partial struct PlayerSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Config>();
            state.RequireForUpdate<PlayerSpawner>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Update Once
            state.Enabled = false;

            var config = SystemAPI.GetSingleton<Config>();

            foreach (var obstacleTransform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Obstacle>())
            {
                var player = state.EntityManager.Instantiate(config.PlayerPrefab);
                
                state.EntityManager.SetComponentData(player, new LocalTransform
                {
                    Position = new float3
                    {
                        x = obstacleTransform.ValueRO.Position.x + config.PlayerOffset,
                        y = 1,
                        z = obstacleTransform.ValueRO.Position.z + config.PlayerOffset
                    },
                    Scale = 1,  // If we didn't set Scale and Rotation, they would default to zero (which is bad!)
                    Rotation = quaternion.identity
                });
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}