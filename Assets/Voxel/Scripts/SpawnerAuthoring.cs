using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace VoxelDancer
{
    public class SpawnerAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject prefab = null;
        [SerializeField] private float3 extent = 1;
        [SerializeField] private LayerMask mask = 0;
        [SerializeField] private float frequency = 100;
        [SerializeField] private uint seed = 0x12345678;

        class Baker : Baker<SpawnerAuthoring>
        {
            public override void Bake(SpawnerAuthoring authoring)
            {
                var data = new Spawner()
                {
                    prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic), // 这里GetEntity是Dynamic的，也就是普通的Entity，这个Prefab是什么东西
                    extent = authoring.extent,
                    mask = authoring.mask.value,
                    frequency = authoring.frequency,
                    random = new Random(authoring.seed)
                };
                //那么这个Dynamic就是创建普通Entity(存在多个，在Runtime不断地被创建销毁的)
                // 这里的Dynamic是和LocalTransform关联，和单例没有关系
                AddComponent(GetEntity(TransformUsageFlags.Dynamic), data); // 创建一种Entity，添加上Spawner Component
            }
        }
    }
    
    public struct Spawner : IComponentData
    {
        public Entity prefab; // 在Component里面包含Entity?
        public float3 extent;
        public int mask;
        public float frequency;
        public Random random;
        public float timer;
    }
}