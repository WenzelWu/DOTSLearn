using Unity.Entities;
using UnityEngine;

namespace KickBall
{
    public class ExecuteAuthoring : MonoBehaviour
    {
        public bool ObstacleSpawner;
        public bool PlayerSpawner;
        public bool PlayerMovement;
        public bool BallSpawner;
        public bool BallMovement;
        public bool BallCarry;
        public bool BallKicking;

        class Baker : Baker<ExecuteAuthoring>
        {
            public override void Bake(ExecuteAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                if (authoring.ObstacleSpawner) AddComponent<ObstacleSpawner>(entity);
                if (authoring.PlayerMovement) AddComponent<PlayerMovement>(entity);
                if (authoring.PlayerSpawner) AddComponent<PlayerSpawner>(entity);
                if (authoring.BallSpawner) AddComponent<BallSpawner>(entity);
                if (authoring.BallMovement) AddComponent<BallMovement>(entity);
                if (authoring.BallCarry) AddComponent<BallCarry>(entity);
                if (authoring.BallKicking) AddComponent<BallKicking>(entity);
            }
        }
    }
    
    // Tag Component
    public struct ObstacleSpawner : IComponentData
    {
    }

    public struct PlayerMovement : IComponentData
    {
    }

    public struct BallMovement : IComponentData
    {
    }

    public struct PlayerSpawner : IComponentData
    {
    }

    public struct BallSpawner : IComponentData
    {
    }

    public struct BallCarry : IComponentData
    {
    }

    public struct BallKicking : IComponentData
    {
    }
}