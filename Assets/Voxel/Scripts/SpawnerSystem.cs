using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Physics;

namespace VoxelDancer
{
    [BurstCompile(CompileSynchronously = true)]
    public partial struct SpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var dt = SystemAPI.Time.DeltaTime;
            
            // 这一段能改为Job吗
            
            // 查询带有Spawner和LocalTransform组件的Entity，并获取它们的值，其中Spawner是可读写的，LocalTransform是只读的
            // Spawner是DynamicEntity，所以场景中存在多个，这里通过这种方式获取，而不是GetSingleton 不对不对，DynamicComponent表示该Entity有LocalTransofmr组件
            // localTransform是每个DynamicEntity必备的Component
            foreach (var(spawner, localTransform) in SystemAPI.Query<RefRW<Spawner>, RefRO<LocalTransform>>())
            {
                var world = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

                var nt = spawner.ValueRO.timer + spawner.ValueRO.frequency * dt; 
                var count = (int)nt; // dt时间下生成的数量
                spawner.ValueRW.timer = nt - count; // 小数部分

                var p0 = localTransform.ValueRO.Position;
                var ext = spawner.ValueRO.extent;
                
                var filter = CollisionFilter.Default;
                filter.CollidesWith = (uint)spawner.ValueRO.mask;

                for (var i = 0; i < count; i++)
                {
                    var disp = spawner.ValueRW.random.NextFloat2(-ext.xy, ext.xy); // 生成随机位置[-1, 1]
                    var vox = SystemAPI.GetSingleton<Voxelizer>();
                    disp = math.floor(disp / vox.voxelSize) * vox.voxelSize; // voxelSize=0.05，体素大小，这一步是离散化，确定生成的位置在呢一个体素里

                    var ray = new RaycastInput()
                    {
                        Start = p0 + math.float3(disp, -ext.z), // 在XY平面上生成，朝Z轴正负方向做射线检测
                        End = p0 + math.float3(disp, +ext.z),
                        Filter = filter
                    };
                    
                    if(!world.CastRay(ray, out RaycastHit hit)) continue;
                    
                    var spawned = state.EntityManager.Instantiate(spawner.ValueRO.prefab); // 在击中位置生成Entity，以Prefab生成Entity？不过这里的Prefab也确实是Entity
                    var entityTransform = SystemAPI.GetComponentRW<LocalTransform>(spawned); 
                    entityTransform.ValueRW.Position = hit.Position; // 获取生成的Entity的LocalTransform组件，设置位置
                }
            }
        }
    }
}