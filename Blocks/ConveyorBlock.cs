using JumpKing.Level;
using Microsoft.Xna.Framework;

namespace ConveyorBlockMod.Blocks
{
    public class ConveyorBlock : BoxBlock, IBlockDebugColor
    {
        private readonly Rectangle m_collider;
        public float Speed { get; set; }

        public Color DebugColor => new Color(255, 100, 0);

        public ConveyorBlock(Rectangle p_collider, byte speed, byte direction) : base(p_collider)
        {
            m_collider = p_collider;
            Speed = (((float)speed) * CalculateDirectionFromColor(direction)) / 2;
        }

        public Rectangle GetRect()
        {
            return m_collider;
        }

        private int CalculateDirectionFromColor(byte direction)
        {
            if (direction == 100)
            {
                return (int)1;
            }
            else
            {
                return (int)-1;
            }
        }
    }
}