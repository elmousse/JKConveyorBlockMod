using HarmonyLib;
using JumpKing.BodyCompBehaviours;
using JumpKing.Level;
using Microsoft.Xna.Framework;
using JumpKing.API;
using System.Collections.Generic;
using ConveyorBlockMod.BlocksBehaviour;
using System.Reflection;

namespace ConveyorBlockMod.Patches
{
    [HarmonyPatch(typeof(ResolveXCollisionBehaviour), nameof(ResolveXCollisionBehaviour.ExecuteBehaviour))]
    class ResolveXCollisionBehaviourPatch
    {
        [HarmonyPrefix]
        static void UpdateXIfCompressedAgainstWall(ResolveXCollisionBehaviour __instance, BehaviourContext behaviourContext)
        {
            var collisionQueryField = AccessTools.Field(typeof(ResolveXCollisionBehaviour), "m_collisionQuery");
            var _m_collisionQuery = (ICollisionQuery)collisionQueryField.GetValue(__instance);

            var blockBehavioursField = AccessTools.Field(typeof(ResolveXCollisionBehaviour), "m_blockBehaviours");
            var _m_blockBehaviours = (LinkedList<IBlockBehaviour>)blockBehavioursField.GetValue(__instance);

            var bodyComp = behaviourContext.BodyComp;
            var hitbox = bodyComp.GetHitbox();
            var collisionCheck = _m_collisionQuery.CheckCollision(hitbox, out Rectangle _, out AdvCollisionInfo _);

            using (LinkedList<IBlockBehaviour>.Enumerator enumerator = _m_blockBehaviours.GetEnumerator())
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    if (current.GetType() == typeof(ConveyorBlockBehaviour))
                    {
                        var instanceConveyorBlockBehaviour = (ConveyorBlockBehaviour)current;
                        if (collisionCheck && instanceConveyorBlockBehaviour.IsPlayerOnBlock)
                        {
                            bodyComp.Position.X -= instanceConveyorBlockBehaviour._collidedConveyorBlock.Speed;
                        }
                    }
                }
        }
    }
}
