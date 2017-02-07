using System;
using System.Collections.Generic;
using System.Text;
using GLX;
using GLX.Collisions;
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

        private Polygon polygon;

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
            if (jumpState == JumpStates.Ground)
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
            UpdatePolygon();
        }

        private void UpdatePolygon()
        {
            List<Vector2> vertices = new List<Vector2>();
            vertices.Add(position);
            vertices.Add(new Vector2(position.X + tex.Width, position.Y));
            vertices.Add(new Vector2(position.X + tex.Width, position.Y + tex.Height));
            vertices.Add(new Vector2(position.X, position.Y + tex.Height));
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] = Vector2.Transform((vertices[i] - position), spriteTransform);
            }
            polygon = new Polygon(vertices);
        }

        public void CheckCollisions(List<Block> walls)
        {
            foreach (var wall in walls)
            {
                GLX.Collisions.MTV? mtv = GLX.Collisions.HelperMethods.Colliding(polygon, wall.polygon);
                if (mtv != null)
                {
                    Vector2 vector = mtv.Value.vector;
                    // you would think that you could just multiply vector by -1 if the second case
                    // is true but for some reason that doesn't work...
                    if (position.X > wall.position.X ||
                        position.Y < wall.position.Y)
                    {
                        if (mtv.Value.magnitude == 0)
                        {
                            position += vector;
                        }
                        else
                        {
                            position += vector * mtv.Value.magnitude;
                        }
                    }
                    else if (position.X < wall.position.X ||
                        position.Y > wall.position.Y)
                    {
                        if (mtv.Value.magnitude == 0)
                        {
                            position -= vector;
                        }
                        else
                        {
                            position -= vector * mtv.Value.magnitude;
                        }
                    }

                    jumpState = JumpStates.Ground;
                    velocity.Y = 0;
                }
            }
        }
    }
}
