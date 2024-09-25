using BehaviorTree;
using EntityComponent;
using EntityComponent.BT;
using JumpKing.API;
using JumpKing.BodyCompBehaviours;
using JumpKing.Level;
using JumpKing.Player;
using System.Linq;
using ConveyorBlockMod.Blocks;
using System.Collections.Generic;

namespace ConveyorBlockMod.BlocksBehaviour
{
    /// <summary>
    /// An implementation of <see cref="IBlockBehaviour"/> representing how the <see cref="ConveyorBlock"/> will
    /// affect the player during runtime
    /// </summary>
    public class ConveyorBlockBehaviour : IBlockBehaviour
    {
        /// <inheritdoc/>
        public float BlockPriority => 2f;

        /// <inheritdoc/>
        public bool IsPlayerOnBlock { get; set; }
        public List<bool> CachePlayerOnBlock { get; set; }
        public bool IsPlayerOnBlockLastFrame;
        public bool IsPlayerOnBlock2FramesBefore;
        public bool IsPlayerOnBlock3FramesBefore;
        public ConveyorBlock _collidedConveyorBlock;

        /// <inheritdoc/>
        public bool AdditionalXCollisionCheck(AdvCollisionInfo info, BehaviourContext behaviourContext)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool AdditionalYCollisionCheck(AdvCollisionInfo info, BehaviourContext behaviourContext)
        {
            return false;
        }

        /// <summary>
        /// <para>Update the X velocity depending on the values (speed and direction) of the <see cref="ConveyorBlock"/> the player is collinding with.</para>
        /// </summary>
        /// <returns>Modified X velocity</returns>
        public float ModifyXVelocity(float inputXVelocity, BehaviourContext behaviourContext)
        {
            if (behaviourContext?.LastFrameCollisionInfo?.PreResolutionCollisionInfo == null)
            {
                return inputXVelocity;
            }
            var newXVelocity = inputXVelocity;
            if (IsPlayerOnBlock)
            {
                _collidedConveyorBlock = (ConveyorBlock)behaviourContext.LastFrameCollisionInfo.PreResolutionCollisionInfo.GetCollidedBlocks<ConveyorBlock>().FirstOrDefault();
                newXVelocity += _collidedConveyorBlock.Speed;
            }
            IsPlayerOnBlock3FramesBefore = IsPlayerOnBlock2FramesBefore;
            IsPlayerOnBlock2FramesBefore = IsPlayerOnBlockLastFrame;
            IsPlayerOnBlockLastFrame = IsPlayerOnBlock;

            return newXVelocity;
        }

        /// <inheritdoc/>
        public float ModifyYVelocity(float inputYVelocity, BehaviourContext behaviourContext)
        {
            return inputYVelocity;
        }

        /// <inheritdoc/>
        public float ModifyGravity(float inputGravity, BehaviourContext behaviourContext)
        {
            return inputGravity;
        }

        /// <summary>
        /// <para>Refresh the state if the player is colliding with a <see cref="ConveyorBlock"/></para>
        /// <para>Then, if the player is not on the block this frame but was on the block last frame and 2 frames before :</para>
        /// <para> - Check if the player is not jumping exactly when leaving the block by himself, if no, resets the <see cref="JumpState"/> result
        /// (this avoids a bug that if the jump key is released one frame after entered this condition, it jumps, but doesn't keep the momentum).</para>
        /// <para>- Add the speed of the block to the player velocity to keep the momentum in the air</para>
        /// </summary>
        /// <returns>If it continues or not</returns>
        public bool ExecuteBlockBehaviour(BehaviourContext behaviourContext)
        {
            if (behaviourContext?.CollisionInfo?.PreResolutionCollisionInfo == null)
            {
                return true;
            }

            IsPlayerOnBlock = behaviourContext.CollisionInfo.PreResolutionCollisionInfo.IsCollidingWith<ConveyorBlock>();

            UpdateXVelocityIfExitingTheBlock(behaviourContext);

            return true;
        }

        #region Private

        private void UpdateXVelocityIfExitingTheBlock(BehaviourContext behaviourContext)
        {
            if (!IsPlayerOnBlock && IsPlayerOnBlockLastFrame && IsPlayerOnBlock2FramesBefore && IsPlayerOnBlock3FramesBefore)
            {
                if (behaviourContext.BodyComp.Velocity.Y >= 0)
                {
                    PlayerEntity player = EntityManager.instance.Find<PlayerEntity>();
                    BehaviorTreeComp behaviourTreeComponent = player.GetComponent<BehaviorTreeComp>();
                    BTmanager behaviourTree = behaviourTreeComponent.GetRaw();
                    JumpState jumpState = behaviourTree.FindNode<JumpState>();
                    jumpState.ResetResult();
                }
                behaviourContext.BodyComp.Velocity.X += _collidedConveyorBlock.Speed;
            }
        }
        #endregion
    }
}