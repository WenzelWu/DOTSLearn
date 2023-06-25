using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Profiling;
using MeshCollider = Unity.Physics.MeshCollider;

namespace VoxelDancer
{
    // monobehaviour也能被BurstCompile？
    [BurstCompile(CompileSynchronously = true)]
    public class CollisionGenerator : MonoBehaviour
    {
        # region Editable attributes
        
        [SerializeField] private SkinnedMeshRenderer source = null;

        #endregion

        #region Private Fields

        private Mesh mesh;

        private Entity entity;

        #endregion

        #region DOTS Interop

        [BurstCompile]
        static void CreateCollider(in EntityManager manager, in Entity entity, in NativeArray<float3> vtx,
            in NativeArray<int3> idx, int layer)
        {
            var filter = CollisionFilter.Default;
            filter.CollidesWith = (uint)layer;

            var collider = MeshCollider.Create(vtx, idx, filter);
            manager.SetComponentData(entity, new PhysicsCollider{Value = collider});
        }

        #endregion

        #region MonoBehaviour Implementation

        void Start()
        {
            mesh = new Mesh();

            var manager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var componentTypes = new ComponentType[]
            {
                typeof(LocalTransform),
                typeof(PhysicsCollider),
                typeof(PhysicsWorldIndex), // shared component, which physic world the entity belongs to
            };

            entity = manager.CreateEntity(componentTypes);

            var world = new PhysicsWorldIndex { Value = 0 };
            
            //Adds a shared component to an entity.为什么要添加两遍，上面的CreateEntity不就是已经存在PhysicsWorldIndex组件了吗
            //上面是指定Compoennt类型，这里是设置Component的值
            manager.AddSharedComponentManaged(entity, world); 
            
        }

        void Update()
        {
            Profiler.BeginSample("BakeMesh");
            
            source.BakeMesh(mesh);
            
            Profiler.EndSample();

            using var vtx = new NativeArray<Vector3>(mesh.vertices, Allocator.Temp);
            using var idx = new NativeArray<int>(mesh.triangles, Allocator.Temp);
            var vtx_re = vtx.Reinterpret<float3>();
            var idx_re = idx.Reinterpret<int3>(sizeof(int));
            
            var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var localTransform = new LocalTransform
            {
                Position = source.transform.position,
                Rotation = source.transform.rotation,
                Scale = source.transform.localScale.x
            };
            manager.SetComponentData(entity, localTransform);
            
            Profiler.BeginSample("MeshCollider Update");
            CreateCollider(manager, entity, vtx_re, idx_re, gameObject.layer);
            Profiler.EndSample();
        }

        #endregion
    }
}