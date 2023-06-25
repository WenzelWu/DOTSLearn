using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

/*
 * 在Unity.Entities命名空间中，TransformUsageFlags枚举有六种，具体含义如下：

 * None：表示实体不需要转换组件。除非其他事物请求其他标志，否则该实体将不会有任何与转换相关的组件，也不会成为层次结构的一部分。它不会影响源GameObject层次结构可能创建的任何LinkedEntityGroup组件的成员资格。

 * Renderable：表示一个实体需要必要的转换组件进行渲染（如Unity.Transforms.LocalToWorld），但它不需要在运行时移动实体所需的转换组件。只有当它们的层次结构中没有父对象是动态的时，Renderable实体才被放置在WorldSpace中。

 * Dynamic：表示实体需要必要的转换组件在运行时移动（如Unity.Transforms.LocalTransform，Unity.Transforms.LocalToWorld）。一个Dynamic实体的Renderable子实体也被视为Dynamic，并且它们会接收一个Unity.Transforms.Parent组件。Dynamic用法也意味着Renderable，因此，您不需要指出一个Dynamic实体也是Renderable。

 * WorldSpace：表示一个实体需要处于世界空间中，即使它们有一个Dynamic实体作为父对象。这意味着实体没有Unity.Transforms.Parent组件，所有它们的转换组件数据都在世界空间中进行烘焙。WorldSpace用法暗示Renderable但不是Dynamic，但Dynamic可以与WorldSpace标志一起使用。

 * NonUniformScale：表示一个实体需要转换组件来表示非均匀缩放。对于Dynamic实体，所有的缩放信息都存储在Unity.Transforms.PostTransformMatrix组件中。对于仅Renderable的实体，缩放信息会合并到Unity.Transforms.LocalToWorld组件中。如果一个GameObject包含一个非均匀的缩放，那么这个标志被认为是对Renderable和Dynamic实体的隐含的。

 * ManualOverride：表示您希望完全手动控制实体的转换转换。这个标志是一个覆盖：当它被设置时，所有其他标志都被忽略。烘焙不会向实体添加任何与转换相关的组件。实体没有父对象，也没有任何子对象附加到它。
 */
namespace VoxelDancer
{
    public class BoxAuthoring : MonoBehaviour
    {
        class Baker : Baker<BoxAuthoring>
        {
            public override void Bake(BoxAuthoring authoring)
            {
                // 直接GetEntity的是不是就是创建一个Singleton Entity
                // TransformUsageFlags.Dynamic是什么意思
                // 还是说GetEntity(Dynamic)是创建普通Entity(存在多个，在Runtime不断地被创建销毁的)
                // 这里的Dynamic是和LocalTransform关联，和单例没有关系
                AddComponent(GetEntity(TransformUsageFlags.Dynamic), new Box()); 
            }
        }
    }

    public struct Box : IComponentData
    {
        public float time;
        public float velocity;
    }
}
