using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private const float GroundXMovement = 5;

        private Polygon polygon;
        public JumpStates jumpState;
        public float storedXVelocity;

        bool canJump;
        bool hasLetGoOfJump;
        TimeSpan maxJumpTime;
        TimeSpan jumpTime;

        private const int MaxDashes = 2;
        int dashes;
        TimeSpan dashTimer;
        TimeSpan dashRefreshTime;
        DashBar dashBar;

        public Player(Texture2D loadedTex, GraphicsDeviceManager graphics) : base(loadedTex)
        {
            jumpState = JumpStates.Air;
            storedXVelocity = 0;
            canJump = false;
            hasLetGoOfJump = true;
            maxJumpTime = TimeSpan.FromMilliseconds(400);
            jumpTime = maxJumpTime;
            dashes = MaxDashes;
            dashRefreshTime = TimeSpan.FromSeconds(5);
            dashTimer = dashRefreshTime;
            dashBar = new DashBar(graphics);
            dashBar.CurrentDashPercent = (float)dashes / MaxDashes;
            dashBar.CooldownBarPercent = (float)dashTimer.Ticks / dashRefreshTime.Ticks;
        }

        public void MoveLeft()
        {
            if (acceleration.X > 0)
            {
                ResetXAcceleration();
            }

            if (jumpState == JumpStates.Ground)
            {
                acceleration.X -= 0.3f;
            }
            else if (jumpState == JumpStates.Air)
            {
                acceleration.X -= 0.1f;
            }
        }

        public void MoveRight()
        {
            if (acceleration.X < 0)
            {
                ResetXAcceleration();
            }

            if (jumpState == JumpStates.Ground)
            {
                acceleration.X += 0.3f;
            }
            else if (jumpState == JumpStates.Air)
            {
                acceleration.X += 0.1f;
            }
        }

        public void ResetXAcceleration()
        {
            acceleration.X = 0;
        }

        public void Jump()
        {
            hasLetGoOfJump = false;
            if (canJump)
            {
                velocity.Y = -10f;
                if (jumpState == JumpStates.WallLeft)
                {
                    velocity.X = 5;
                }
                else if (jumpState == JumpStates.WallRight)
                {
                    velocity.X = -5;
                }
                jumpState = JumpStates.Air;
            }
        }

        public void CancelJump()
        {
            canJump = false;
            hasLetGoOfJump = true;
        }

        public void ResetJump()
        {
            if (hasLetGoOfJump)
            {
                canJump = true;
                jumpTime = maxJumpTime;
            }
        }

        public void Dash()
        {
            if (dashes > 0)
            {
                dashes--;
                position += Vector2.Normalize(velocity) * 200;
            }
        }

        public override void Update(GameTimeWrapper gameTime)
        {
            if (dashes < MaxDashes)
            {
                dashTimer -= gameTime.ElapsedGameTime;
            }

            if (dashTimer <= TimeSpan.Zero)
            {
                if (dashes < MaxDashes)
                {
                    dashes++;
                }
                dashTimer = dashRefreshTime;
            }

            dashBar.CurrentDashPercent = (float)dashes / MaxDashes;
            dashBar.CooldownBarPercent = (1 - (float)dashTimer.Ticks / dashRefreshTime.Ticks) * (1f / MaxDashes);

            // slow down player if they are not holding a direction
            if (jumpState == JumpStates.Ground)
            {
                // ground friction
                velocity.X *= 0.87f;
            }
            else
            {
                // air friction
                velocity.X *= 0.9f;
            }

            if (canJump &&
                jumpState == JumpStates.Air)
            {
                jumpTime -= gameTime.ElapsedGameTime;
            }

            if (jumpTime <= TimeSpan.Zero)
            {
                canJump = false;
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

            acceleration.X = MathHelper.Clamp(acceleration.X, -10, 10);
            velocity.X = MathHelper.Clamp(velocity.X, -15, 15);
            //velocity.Y = MathHelper.Clamp(velocity.Y, -50, 50);

            base.Update(gameTime);
            UpdatePolygon();
            dashBar.Position = new Vector2(position.X - 500, position.Y - 500);
            dashBar.Update(gameTime);
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

                    if (mtv.Value.vector.Y == 0)
                    {
                        canJump = false;
                        velocity = Vector2.Zero;
                        acceleration = Vector2.Zero;
                        if (position.X > wall.center.X)
                        {
                            jumpState = JumpStates.WallLeft;
                        }
                        else if (position.X < wall.center.X)
                        {
                            jumpState = JumpStates.WallRight;
                        }
                        ResetJump();
                    }
                    else
                    {
                        jumpState = JumpStates.Ground;
                        velocity.Y = 0;
                        acceleration.Y = 0;
                        ResetJump();
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            dashBar.Draw(spriteBatch);
        }
    }
}
