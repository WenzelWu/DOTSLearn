﻿using Unity.Entities;
using UnityEngine;

namespace KickBall
{
    // The Config component will be used as a singleton (meaning only one entity will have this component).
    // It stores a grab bag of game parameters plus the entity prefabs that we'll instantiate at runtime.
    public class ConfigAuthoring : MonoBehaviour
    {
        public int ObstaclesNumRows;
        public int ObstaclesNumColumns;
        public float ObstacleGridCellSize;
        public float ObstacleRadius;
        public float ObstacleOffset;
        public float PlayerOffset;
        public float PlayerSpeed;
        public float BallStartVelocity;
        public float BallVelocityDecay;
        public float BallKickingRange;
        public float BallKickForce;
        public GameObject ObstaclePrefab;
        public GameObject PlayerPrefab;
        public GameObject BallPrefab;

        class Baker : Baker<ConfigAuthoring>
        {
            public override void Bake(ConfigAuthoring authoring)
            {
                // GetEntity() bakes current GameObject into an entity
                var entity = GetEntity(TransformUsageFlags.None);
                
                AddComponent(entity, new Config
                {
                    NumRows = authoring.ObstaclesNumRows,
                    NumColumns = authoring.ObstaclesNumColumns,
                    ObstacleGridCellSize = authoring.ObstacleGridCellSize,
                    ObstacleRadius = authoring.ObstacleRadius,
                    ObstacleOffset = authoring.ObstacleOffset,
                    PlayerOffset = authoring.PlayerOffset,
                    PlayerSpeed = authoring.PlayerSpeed,
                    BallStartVelocity = authoring.BallStartVelocity,
                    BallVelocityDecay = authoring.BallVelocityDecay,
                    BallKickingRangeSQ = authoring.BallKickingRange * authoring.BallKickingRange,
                    BallKickForce = authoring.BallKickForce,
                    // GetEntity() bakes a GameObject prefab into its entity equivalent.
                    ObstaclePrefab = GetEntity(authoring.ObstaclePrefab, TransformUsageFlags.Dynamic),
                    PlayerPrefab = GetEntity(authoring.PlayerPrefab, TransformUsageFlags.Dynamic),
                    BallPrefab = GetEntity(authoring.BallPrefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }

    public struct Config : IComponentData
    {
        public int NumRows;
        public int NumColumns;
        public float ObstacleGridCellSize;
        public float ObstacleRadius;
        public float ObstacleOffset;
        public float PlayerOffset;
        public float PlayerSpeed;
        public float BallStartVelocity;
        public float BallVelocityDecay;
        public float BallKickingRangeSQ;
        public float BallKickForce;
        public Entity ObstaclePrefab;
        public Entity PlayerPrefab;
        public Entity BallPrefab;
    }
}