using Unity.Entities;
using UnityEngine;

namespace KickBall
{
    public class ObstacleAuthoring : MonoBehaviour
    {
        class Baker : Baker<ObstacleAuthoring>
        {
            public override void Bake(ObstacleAuthoring authoring)
            {
                // GetEntity() bakes current GameObject into an entity
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                // ObstacleAuthoring's GameObject is baked into an entity in build time-GetEntity()
                // Add the Obstacle component to the entity.
                AddComponent<Obstacle>(entity);
            }
        }
    }
    
    public struct Obstacle : IComponentData
    {
        // empty struct - tag component, used to mark entities as obstacles, so we can query for all obstacles
    }
}