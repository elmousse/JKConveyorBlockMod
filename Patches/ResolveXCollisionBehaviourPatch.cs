using HarmonyLib;
using JumpKing.BodyCompBehaviours;
using JumpKing.Level;
using Microsoft.Xna.Framework;
using JumpKing.API;
using System.Collections.Generic;
using ConveyorBlockMod.BlocksBehaviour;
using System.Reflection;
using ConveyorBlockMod.Utils;
using System.Linq;
using ConveyorBlockMod.Blocks;
using JumpKing.Player;
using System;

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
            var collisionCheck = _m_collisionQuery.CheckCollision(hitbox, out Rectangle overlap, out AdvCollisionInfo advCollisionInfo);

            var collidedSlopeBlocks = advCollisionInfo.GetCollidedBlocks<SlopeBlock>();

            if (!collisionCheck)
            {
                return;
            }

            using (LinkedList<IBlockBehaviour>.Enumerator enumerator = _m_blockBehaviours.GetEnumerator())
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    if (current.GetType() == typeof(ConveyorBlockBehaviour))
                    {
                        var instanceConveyorBlockBehaviour = (ConveyorBlockBehaviour)current;
                        if (!instanceConveyorBlockBehaviour.IsPlayerOnBlock)
                        {
                            return;
                        }
                        var slope = UpdateXPositionIfSlopeAfterConveyorBlock(instanceConveyorBlockBehaviour._collidedConveyorBlock, collidedSlopeBlocks, bodyComp);
                        if (slope)
                        {
                            return;
                        }
                        bodyComp.Position.X -= overlap.Width * Math.Sign(instanceConveyorBlockBehaviour._collidedConveyorBlock.Speed);
                    }
                }
        }
        private static bool UpdateXPositionIfSlopeAfterConveyorBlock(ConveyorBlock collidedConveyorBlock, IReadOnlyList<IBlock> collidedSlopeBlocks, BodyComp bodyComp)
        {
            if (collidedSlopeBlocks.Count == 0)
            {
                return false;
            }

            var collidedConveyorBlockYPosition = collidedConveyorBlock.GetRect().Location.Y;
            var affectingSlope = (SlopeBlock)collidedSlopeBlocks.Where(x => x.GetRect().Location.Y == (collidedConveyorBlockYPosition - 8)).FirstOrDefault();

            if (affectingSlope == null)
            {
                return false;
            }
            if (!(((affectingSlope.GetSlopeType() == SlopeType.TopRight) && (collidedConveyorBlock.Speed < 0)) || ((affectingSlope.GetSlopeType() == SlopeType.TopLeft) && (collidedConveyorBlock.Speed > 0))))
            {
                return false;
            }
            do
            {
                bodyComp.Position.Y -= 1;
            } while (affectingSlope.GetRect().Intersects(bodyComp.GetHitbox()));
            return true;
        }
    }
}
