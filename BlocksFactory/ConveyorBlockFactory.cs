using JumpKing.API;
using JumpKing.Level;
using JumpKing.Level.Sampler;
using JumpKing.Workshop;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ConveyorBlockMod.Blocks
{
    public class ConveyorBlockFactory : IBlockFactory
    {
        private readonly HashSet<Color> supportedBlockCodes = new HashSet<Color>()
        {
            // move right
            new Color(255, 100, 1),
            new Color(255, 100, 2),
            new Color(255, 100, 3),
            new Color(255, 100, 4),
            new Color(255, 100, 5),
            new Color(255, 100, 6),
            new Color(255, 100, 7),
            new Color(255, 100, 8),
            new Color(255, 100, 9),
            new Color(255, 100, 10),
            new Color(255, 100, 11),
            new Color(255, 100, 12),
            new Color(255, 100, 13),
            new Color(255, 100, 14),
            new Color(255, 100, 15),
            new Color(255, 100, 16),

            // move left
            new Color(255, 101, 1),
            new Color(255, 101, 2),
            new Color(255, 101, 3),
            new Color(255, 101, 4),
            new Color(255, 101, 5),
            new Color(255, 101, 6),
            new Color(255, 101, 7),
            new Color(255, 101, 8),
            new Color(255, 101, 9),
            new Color(255, 101, 10),
            new Color(255, 101, 11),
            new Color(255, 101, 12),
            new Color(255, 101, 13),
            new Color(255, 101, 14),
            new Color(255, 101, 15),
            new Color(255, 101, 16)
        };

        /// <inheritdoc/>
        public bool CanMakeBlock(Color blockCode, Level level)
        {
            return supportedBlockCodes.Contains(blockCode);
        }

        /// <inheritdoc/>
        public bool IsSolidBlock(Color blockCode)
        {
            return supportedBlockCodes.Contains(blockCode);
        }

        /// <inheritdoc/>
        public IBlock GetBlock(Color blockCode, Rectangle blockRect, JumpKing.Workshop.Level level, LevelTexture textureSrc, int currentScreen, int x, int y)
        {
            if (supportedBlockCodes.Contains(blockCode))
            {
                return new ConveyorBlock(blockRect, blockCode.B, blockCode.G);
            }
            else
            {
                throw new InvalidOperationException($"{nameof(ConveyorBlockFactory)} is unable to create a block of Color code ({blockCode.R}, {blockCode.G}, {blockCode.B})");
            }
        }
    }
}