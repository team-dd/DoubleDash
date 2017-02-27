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
        private const int WALL_JUMP_BUFFER = 25;

        private Polygon polygon;
        public JumpStates jumpState;
        public float storedXVelocity;

        bool canJump;
        bool hasLetGoOfJump;
        TimeSpan maxJumpTime;
        TimeSpan jumpTime;

        private const int MaxDashes = 2;
        private const int DashDistance = 200;
        Sprite dashIndicator;
        int dashes;
        TimeSpan dashTimer;
        TimeSpan dashRefreshTime;
        public DashBar dashBar;

        TextItem canJumpText;
        TextItem hasLetGoOfJumpText;
        TextItem jumpTimeText;

        int wallJumpCounter;

        public Player(Texture2D loadedTex, Texture2D dashIndicatorTex, GraphicsDeviceManager graphics) : base(loadedTex)
        {
            jumpState = JumpStates.Air;
            storedXVelocity = 0;
            canJump = false;
            hasLetGoOfJump = true;
            maxJumpTime = TimeSpan.FromMilliseconds(400);
            jumpTime = maxJumpTime;
            dashIndicator = new Sprite(dashIndicatorTex);
            dashes = MaxDashes;
            dashRefreshTime = TimeSpan.FromSeconds(5);
            dashTimer = dashRefreshTime;
            dashBar = new DashBar(graphics);
            dashBar.CurrentDashPercent = (float)dashes / MaxDashes;
            dashBar.CooldownBarPercent = (float)dashTimer.Ticks / dashRefreshTime.Ticks;
            wallJumpCounter = 0;

            LoadDebugTexts();
        }

        public void LoadDebugTexts()
        {
            canJumpText = new TextItem(DebugText.spriteFont);
            hasLetGoOfJumpText = new TextItem(DebugText.spriteFont);
            jumpTimeText = new TextItem(DebugText.spriteFont);
            DebugText.Add(canJumpText, hasLetGoOfJumpText, jumpTimeText);
        }

        public void MoveLeft()
        {
            if (acceleration.X > 0)
            {
                ResetXAcceleration();
            }

            if (jumpState == JumpStates.Ground)
            {
                acceleration.X -= 0.02f;
            }
            else if (jumpState == JumpStates.Air)
            {
                acceleration.X -= 0.01f;
            }
            else if (jumpState == JumpStates.WallRight)
            {
                if (wallJumpCounter < WALL_JUMP_BUFFER)
                {
                    wallJumpCounter += 1;
                }
                else
                {
                    jumpState = JumpStates.Air;
                    acceleration.X -= 0.01f;
                    wallJumpCounter = 0;
                }
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
                acceleration.X += 0.02f;
            }
            else if (jumpState == JumpStates.Air)
            {
                acceleration.X += 0.01f;
            }
            else if (jumpState == JumpStates.WallLeft)
            {
                if (wallJumpCounter < WALL_JUMP_BUFFER)
                {
                    wallJumpCounter += 1;
                }
                else
                {
                    jumpState = JumpStates.Air;
                    acceleration.X += 0.01f;
                    wallJumpCounter = 0;
                }
            }
        }

        public void ResetXAcceleration()
        {
            acceleration.X = 0;
            wallJumpCounter = 0;
        }

        public void Jump()
        {
            hasLetGoOfJump = false;
            if (canJump)
            {
                velocity.Y = -7f;
                if (jumpState == JumpStates.WallLeft)
                {
                    velocity.X = 5;
                    acceleration.X = .1f;
                }
                else if (jumpState == JumpStates.WallRight)
                {
                    velocity.X = -5;
                    acceleration.X = -.1f;
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
                position += Vector2.Normalize(velocity) * DashDistance;
            }
        }

        private void UpdateDebugTexts(GameTimeWrapper gameTime)
        {
            canJumpText.text = $"{nameof(canJump)}: {canJump.ToString()}";
            hasLetGoOfJumpText.text = $"{nameof(hasLetGoOfJump)}: {hasLetGoOfJump.ToString()}";
            jumpTimeText.text = $"{nameof(jumpTime)}: {jumpTime.ToString()}";
        }

        public override void Update(GameTimeWrapper gameTime)
        {
            if (dashes < MaxDashes)
            {
                dashTimer -= gameTime.ElapsedGameTime;

                if (dashes <= 0)
                {
                    dashIndicator.visible = false;
                }
            }

            if (dashTimer <= TimeSpan.Zero)
            {
                if (dashes < MaxDashes)
                {
                    dashes++;
                }
                dashTimer = dashRefreshTime;

                if (dashes > 0)
                {
                    dashIndicator.visible = true;
                }
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

            acceleration.X = MathHelper.Clamp(acceleration.X, -1f, 1f);
            velocity.X = MathHelper.Clamp(velocity.X, -10, 10);
            //velocity.Y = MathHelper.Clamp(velocity.Y, -50, 50);

            base.Update(gameTime);
            UpdatePolygon();

            if (dashIndicator.visible)
            {
                dashIndicator.position = position + Vector2.Normalize(velocity) * DashDistance;
            }

            UpdateDebugTexts(gameTime);
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
                    if (position.X > wall.position.X)
                    {
                       position.X += vector.X * mtv.Value.magnitude;
                    }
                    else if (position.X < wall.position.X)
                    {
                        position.X -= vector.X * mtv.Value.magnitude;
                    }

                    if (position.Y < wall.position.Y)
                    {
                        position.Y += vector.Y * mtv.Value.magnitude;
                    }
                    else if (position.Y > wall.position.Y)
                    {
                        position.Y -= vector.Y * mtv.Value.magnitude;
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
            if (dashIndicator.visible)
            {
                dashIndicator.Draw(spriteBatch);
            }
        }
    }
}
