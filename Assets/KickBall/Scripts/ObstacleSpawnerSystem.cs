using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace KickBall
{
    // Systems in TransformSystemGroup compute the rendering matrix from entity's LocalTransform component
    // UpdateBefore attribute ensures that this system runs before TransformSystemGroup, thus the obstacles we spawn
    // will have their rendering matrix computed in the same frame rather than the next frame
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct ObstacleSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            // System updates are skipped as long as no instances of component T exist in the world
            
            // Normally a system will start updating before the main scene is loaded. By using RequireForUpdate,
            // we can make a system skip updating until certain components are loaded from the scene.
            
            // This system needs to access the singleton component Config, which won't exist until the scene has loaded
            state.RequireForUpdate<Config>();
            
            // The ExecuteAuthoring is used to control which systems run in which scenes.
            // By adding the ExecuteAuthoring component to a GameObject in the sub scene and checking the ObstacleSpawner
            // checkbox, an instance of ObstacleSpawner component will be created in the scene, and so this system will start updating when the scene loads
            state.RequireForUpdate<ObstacleSpawner>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // We only want to spawn obstacles one time. Disabling the system stops subsequent updates
            state.Enabled = false;
            
            // GetSingleton and SetSingleton are conveniences for accessing a "singleton" component (a component type that only one entity has)
            // If 0 entities or 2 or more entities have the Config component, this GetSingleton() call will throw an exception
            var config = SystemAPI.GetSingleton<Config>(); 
            // Singleton Component并没有一种指定的创建方式，只能通过保证只有一个实体拥有该组件来实现
            // 在这个例子中，我们在SubScene中创建一个Config GameObject，添加ConfigAuthoring和ExecuteAuthoring组件
            // 保证World中只存在一个Config Entity，从而保证Config Component和ObstacleSpawner Component等Component是Singleton Component
            // 与之相反的，ObstacleAuthoring挂载在Obstacle Prefab上，Obstacle Prefab的Entity会创建多个，因此Obstacle Component也会创建多个

            var rand = new Random(123);
            var scale = config.ObstacleRadius * 2;

            for (int column = 0; column < config.NumColumns; column++)
            {
                for (int row = 0; row < config.NumRows; row++)
                {
                    // Instantiate: create an entity with all the same component types and component values as the ObstaclePrefab entity
                    var obstacle = state.EntityManager.Instantiate(config.ObstaclePrefab);
                    
                    // SetComponentData: set the value of a component type on an entity
                    // In ObstacleAuthoring, we set the Obstacle Entity's TransformUsageFlags to TransformUsageFlags.Dynamic
                    // Thus the Obstacle Entity has a LocalTransform Component
                    state.EntityManager.SetComponentData(obstacle, new LocalTransform
                    {
                        Position = new float3
                        {
                            x = (column * config.ObstacleGridCellSize) + rand.NextFloat(config.ObstacleOffset),
                            y = 0,
                            z = (row * config.ObstacleGridCellSize) + rand.NextFloat(config.ObstacleOffset)
                        },
                        Scale = scale,
                        Rotation = quaternion.identity
                    });
                }
            }
        }
    }
}