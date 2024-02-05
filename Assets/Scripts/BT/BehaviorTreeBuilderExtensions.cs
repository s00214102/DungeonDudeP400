using CleverCrow.Fluid.BTs.Trees;
using UnityEngine;

public static class BehaviorTreeBuilderExtensions
{
    public static BehaviorTreeBuilder AgentDestination(this BehaviorTreeBuilder builder, string name, Transform target)
    {
        return builder.AddNode(new AgentDestination{ Name = name, target = target });
    }
}
