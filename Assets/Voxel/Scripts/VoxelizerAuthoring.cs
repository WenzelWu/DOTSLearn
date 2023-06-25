using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace VoxelDancer
{
    public class VoxelizerAuthoring : MonoBehaviour
    {
        [SerializeField] private float voxelSize = 0.05f;
        [SerializeField] private float voxelLife = 0.3f;
        [SerializeField] private float colorFrequency = 0.5f;
        [SerializeField] private float colorSpeed = 0.5f;
        [SerializeField] private float gravity = 0.2f;

        class Baker : Baker<VoxelizerAuthoring>
        {
            public override void Bake(VoxelizerAuthoring authoring)
            {
                var data = new Voxelizer()
                {
                    voxelSize = authoring.voxelSize,
                    voxelLife = authoring.voxelLife,
                    colorFrequency = authoring.colorFrequency,
                    colorSpeed = authoring.colorSpeed,
                    gravity = authoring.gravity,
                };
                // 同样的，直接GetEntity的是不是就是创建一个Singleton Entity
                // 但为什么这个是None
                // 还是说GetEntity(None)才是创建Singleton Entity
                // 这里的Dynamic是和LocalTransform关联，和单例没有关系
                AddComponent(GetEntity(TransformUsageFlags.None), data);
            }
        }
    }

    public struct Voxelizer : IComponentData
    {
        public float voxelSize;
        public float voxelLife;
        public float colorFrequency;
        public float colorSpeed;
        public float gravity;
    }
}