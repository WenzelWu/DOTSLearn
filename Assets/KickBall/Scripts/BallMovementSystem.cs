using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace KickBall
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct BallMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Config>();
            state.RequireForUpdate<BallMovement>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<Config>();
            
            var obstacleQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform, Obstacle>().Build(); // SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Obstacle>();
            var minDist = config.ObstacleRadius + 0.5f;

            var job = new BallMovementJob
            {
                ObstacleTransforms = obstacleQuery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator),
                DecayFactor = config.BallVelocityDecay * SystemAPI.Time.DeltaTime,
                DeltaTime = SystemAPI.Time.DeltaTime,
                MinDistToObstacleSQ = minDist * minDist
            };
            job.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
        
        // 继承自IJobEntity的Job会对所有Entity执行，通过WithAll和WithDisabled可以过滤掉不需要的Entity？
        [WithAll(typeof(Ball))]
        [WithDisabled(typeof(Carry))]
        [BurstCompile]
        public partial struct BallMovementJob : IJobEntity
        {
            [ReadOnly] public NativeArray<LocalTransform> ObstacleTransforms;
            public float DecayFactor;
            public float DeltaTime;
            public float MinDistToObstacleSQ;
        
            // Execute方法会对通过Filter的Entity执行？
            // 输入可以(任意/全部)通过Filter的Entity上的相同组件？
            public void Execute(ref LocalTransform transform, ref Velocity velocity)
            {
                if (velocity.Value.Equals(float2.zero))
                {
                    return;
                }

                var magnitude = math.length(velocity.Value);
                var newPosition = transform.Position +
                                  new float3(velocity.Value.x, 0, velocity.Value.y) * DeltaTime;
                foreach (var obstacleTransform in ObstacleTransforms)
                {
                    if (math.distancesq(newPosition, obstacleTransform.Position) <= MinDistToObstacleSQ)
                    {
                        newPosition = DeflectBall(transform.Position, obstacleTransform.Position, ref velocity, magnitude, DeltaTime);
                        break;
                    }
                }

                transform.Position = newPosition;

                var newMagnitude = math.max(magnitude - DecayFactor, 0);
                velocity.Value = math.normalizesafe(velocity.Value) * newMagnitude;
            }
            
            private float3 DeflectBall(float3 ballPos, float3 obstaclePos, ref Velocity velocity, float magnitude, float dt)
            {
                var obstacleToBallVector = math.normalize((ballPos - obstaclePos).xz);
                velocity.Value = math.reflect(math.normalize(velocity.Value), obstacleToBallVector) * magnitude;
                return ballPos + new float3(velocity.Value.x, 0, velocity.Value.y) * dt;
            }
        }
    }
}