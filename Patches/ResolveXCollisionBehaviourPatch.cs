using HarmonyLib;
using JumpKing.BodyCompBehaviours;
using JumpKing.Level;
using Microsoft.Xna.Framework;
using JumpKing.API;
using System.Collections.Generic;
using ConveyorBlockMod.BlocksBehaviour;
using System;
using ConveyorBlockMod.Blocks;
using JumpKing.Player;
using System.Linq;

namespace ConveyorBlockMod.Patches
{
    [HarmonyPatch(typeof(ResolveXCollisionBehaviour), nameof(ResolveXCollisionBehaviour.ExecuteBehaviour))]
    class ResolveXCollisionBehaviourPatch
    {
        private static BodyComp _bodyComp;
        private static AdvCollisionInfo _advCollisionInfo;
        private static ConveyorBlockBehaviour _instanceConveyorBlockBehaviour;
        private static Rectangle _overlap;

        [HarmonyPrefix]
        static void UpdateXIfCompressedAgainstWall(ResolveXCollisionBehaviour __instance, BehaviourContext behaviourContext)
        {
            var collisionQueryField = AccessTools.Field(typeof(ResolveXCollisionBehaviour), "m_collisionQuery");
            var _m_collisionQuery = (ICollisionQuery)collisionQueryField.GetValue(__instance);

            var blockBehavioursField = AccessTools.Field(typeof(ResolveXCollisionBehaviour), "m_blockBehaviours");
            var _m_blockBehaviours = (LinkedList<IBlockBehaviour>)blockBehavioursField.GetValue(__instance);

            _bodyComp = behaviourContext.BodyComp;
            var hitbox = _bodyComp.GetHitbox();
            var collisionCheck = _m_collisionQuery.CheckCollision(hitbox, out Rectangle overlap, out AdvCollisionInfo advCollisionInfo);
            _advCollisionInfo = advCollisionInfo;
            _overlap = overlap;

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
                        _instanceConveyorBlockBehaviour = (ConveyorBlockBehaviour)current;

                        if (_instanceConveyorBlockBehaviour._collidedConveyorBlock == null)
                        {
                            return;
                        }

                        if (IsBouncingAgainstConveyorBlock())
                        {
                            return;
                        }

                        if (_instanceConveyorBlockBehaviour.IsPlayerOnBlock)
                        {
                            if (AnySlopeCollided())
                            {
                                _bodyComp.Position.X -= _instanceConveyorBlockBehaviour._collidedConveyorBlock.Speed;
                                return;
                            }

                            _bodyComp.Position.X -= _overlap.Width * Math.Sign(_instanceConveyorBlockBehaviour._collidedConveyorBlock.Speed) * (IsCompressedAgainstWall() ? 1 : -1);
                            
                            return;
                        }

                        if ((_instanceConveyorBlockBehaviour.IsPlayerOnBlock2FramesBefore || _instanceConveyorBlockBehaviour.IsPlayerOnBlock3FramesBefore) && !AnySlopeCollided())
                        {
                            _bodyComp.Position.X -= _overlap.Width * Math.Sign(_instanceConveyorBlockBehaviour._collidedConveyorBlock.Speed) * (IsCompressedAgainstWall() ? 1 : -1);
                            _bodyComp.Velocity.X = 0;
                            return;
                        }
                    }
                }
        }

        private static bool AnySlopeCollided()
        {
            var collidedBlocks = _advCollisionInfo.GetCollidedBlocks();
            foreach (var block in collidedBlocks)
            {
                if (block is SlopeBlock)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsCompressedAgainstWall()
        {
            var collidedBlocks = _advCollisionInfo.GetCollidedBlocks<BoxBlock>().Except(_advCollisionInfo.GetCollidedBlocks<ConveyorBlock>());
            var collidedConveyorBlock = _instanceConveyorBlockBehaviour._collidedConveyorBlock;

            foreach (var block in collidedBlocks)
            {
                if (block.GetRect().Location.Y >= collidedConveyorBlock.GetRect().Location.Y)
                {
                    continue;
                }
                if (collidedConveyorBlock.Speed >= 0)
                {
                    if (block.GetRect().Location.X > collidedConveyorBlock.GetRect().Location.X)
                    {
                        return true;
                    }
                }
                else
                {
                    if (block.GetRect().Location.X < collidedConveyorBlock.GetRect().Location.X)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool IsBouncingAgainstConveyorBlock()
        {
            if (!_instanceConveyorBlockBehaviour.IsPlayerOnBlock && !_instanceConveyorBlockBehaviour.IsPlayerOnBlockLastFrame && !_instanceConveyorBlockBehaviour.IsPlayerOnBlock2FramesBefore && !_instanceConveyorBlockBehaviour.IsPlayerOnBlock3FramesBefore)
            {
                return false;
            }
            if (_instanceConveyorBlockBehaviour._collidedConveyorBlock.GetRect().Location.Y >= (_bodyComp.Position.Y + _bodyComp.GetHitbox().Height))
            {
                return false;
            }
            if (Math.Sign(_instanceConveyorBlockBehaviour._collidedConveyorBlock.Speed) != Math.Sign(_bodyComp.Velocity.X))
            {
                _bodyComp.Position.X -= 2 * _overlap.Width * Math.Sign(_instanceConveyorBlockBehaviour._collidedConveyorBlock.Speed);
            }
            return true;

        }
    }
}
