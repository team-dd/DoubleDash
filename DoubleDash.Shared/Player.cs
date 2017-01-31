using System;
using System.Collections.Generic;
using System.Text;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoubleDash
{
    public class Player : Sprite
    {
        public enum JumpStates
        {
            Ground,
            WallRight,
            WallLeft,
            Air
        }

        public JumpStates jumpState;
        public float storedXVelocity;

        public Player(Texture2D loadedTex) : base(loadedTex)
        {
            jumpState = JumpStates.Ground;
            storedXVelocity = 0;
        }

        public void MoveLeft()
        {
            velocity.X -= 1.25f;
        }

        public void MoveRight()
        {
            velocity.X += 1.25f;
        }

        public void Jump()
        {
            if (jumpState == JumpStates.Ground)
            {
                velocity.Y -= 14.5f;
            }
            else if (jumpState == JumpStates.WallRight)
            {
                velocity.Y = -10;
                velocity.X = -12;
            }
            else if (jumpState == JumpStates.WallLeft)
            {
                velocity.Y = -10;
                velocity.X = -12;
            }
            jumpState = JumpStates.Air;
        }

        public override void Update(GameTimeWrapper gameTime)
        {
            // slow down player if they are not holding a direction
            if (jumpState == Player.JumpStates.Ground)
            {
                // ground friction
                velocity.X *= 0.8f;
            }
            else
            {
                // air friction
                velocity.X *= 0.8f;
            }

            if (jumpState == JumpStates.WallLeft ||
                jumpState == JumpStates.WallRight)
            {
                velocity.Y += GameHelpers.Gravity / 1.5f;
            }
            else
            {
                velocity.Y += GameHelpers.Gravity;
            }

            velocity.X = MathHelper.Clamp(velocity.X, -100, 100);
            velocity.Y = MathHelper.Clamp(velocity.Y, -50, 50);

            base.Update(gameTime);
        }
    }
}
