using Unity.Entities;
using UnityEngine;

namespace KickBall
{
    public class PlayerAuthoring : MonoBehaviour
    {
        class Baker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<Player>(entity);
                AddComponent<Carry>(entity);
                SetComponentEnabled<Carry>(entity, false);
            }
        }
    }

    public struct Player : IComponentData
    {
        
    }
    
    // IEnableableComponent is used to enable/disable components at runtime
    public struct Carry : IComponentData, IEnableableComponent
    {
        // if this Component is added to a player, Target refers to the ball being carried
        // if this Component is added to a ball, Target refers to the player carrying the ball
        public Entity Target;
    }
}