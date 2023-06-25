using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace VoxelDancer
{
    [BurstCompile(CompileSynchronously = true)]
    public partial struct BoxUpdateSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if(!SystemAPI.HasSingleton<Voxelizer>()) return;

            var writer = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(); // 这个创建的方法怎么这么麻烦

            var job = new BoxUpdateJob()
            {
                commands = writer,
                voxelizer = SystemAPI.GetSingleton<Voxelizer>(), // GetEntity(None)创建Singleton Entity，这边可以用GetSingleton获取
                time = (float)SystemAPI.Time.ElapsedTime,
                deltaTime = SystemAPI.Time.DeltaTime
            };

            job.ScheduleParallel();
        }
    }
    
    [BurstCompile(CompileSynchronously = true)]
    public partial struct BoxUpdateJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter commands;
        public Voxelizer voxelizer;
        public float time;
        public float deltaTime;

        void Execute([ChunkIndexInQuery] int index,
            Entity entity,
            ref LocalTransform localTransform,
            ref Box box,
            ref URPMaterialPropertyBaseColor color)
        {
            box.time += deltaTime;

            if (box.time > voxelizer.voxelLife) // box的存在时间超过了设置的生命周期 voxelizer是单例全局唯一
            {
                // 在这里写入commandbuffer，并不立即执行，等到CommandBuffer被调用到的时候统一处理所有指令
                // 由于在Job中冰鞋雪茹，所以是ParallelWriter
                commands.DestroyEntity(index, entity); //不过销毁为什么需要index和Entity，有任意一个不就可以确定了吗，还是说这里的Index不是Entity在Chunk中的Index，而是Chunk的Index？
                return;
            }
            
            // 速度、位置、大小、颜色都跟随时间更新
            box.velocity -= voxelizer.gravity * deltaTime;
            localTransform.Position += box.velocity * deltaTime;

            if (localTransform.Position.y < 0)
            {
                box.velocity *= -1;
                localTransform.Position.y = -localTransform.Position.y;
            }
            
            var p01 = box.time / voxelizer.voxelLife;
            var p01_2 = p01 * p01;
            localTransform.Scale = voxelizer.voxelSize * (1 - p01_2 * p01_2);

            var hue = localTransform.Position.z * voxelizer.colorFrequency;
            hue = math.frac(hue + time * voxelizer.colorSpeed);
            color.Value = (Vector4)Color.HSVToRGB(hue, 1, 1);
        }
    }
}