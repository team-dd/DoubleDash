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
        private const int WALL_JUMP_BUFFER = 10 * 10; // change to a timer

        public float yDeathThreshold;

        private Polygon polygon;
        public JumpStates jumpState;
        public float storedXVelocity;

        bool canJump;
        bool hasLetGoOfJump;
        TimeSpan maxJumpTime;
        TimeSpan jumpTime;

        private const int MaxDashes = 2;
        private const int DashDistance = 300;
        Sprite dashIndicator;
        int dashes;
        TimeSpan dashTimer;
        TimeSpan dashRefreshTime;
        public DashBar dashBar;

        TextItem canJumpText;
        TextItem hasLetGoOfJumpText;
        TextItem jumpTimeText;
        TextItem velocityText;

        private bool justHitWall;

        private bool isOnMovingBlock;

        public Vector2 spawnPoint;

        int wallJumpCounter;

        public Player(SpriteSheetInfo info, Texture2D dashIndicatorTex, GraphicsDeviceManager graphics) : base(info, graphics)
        {
            maxJumpTime = TimeSpan.FromMilliseconds(400);
            dashIndicator = new Sprite(dashIndicatorTex);
            dashRefreshTime = TimeSpan.FromSeconds(1);
            dashBar = new DashBar(graphics);
            justHitWall = false;
            Reset();
            yDeathThreshold = 0;

            LoadDebugTexts();
        }

        public void LoadDebugTexts()
        {
            canJumpText = new TextItem(DebugText.spriteFont);
            hasLetGoOfJumpText = new TextItem(DebugText.spriteFont);
            jumpTimeText = new TextItem(DebugText.spriteFont);
            velocityText = new TextItem(DebugText.spriteFont);
            DebugText.Add(canJumpText, hasLetGoOfJumpText, jumpTimeText, velocityText);
        }

        public void Reset()
        {
            position = spawnPoint;
            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
            jumpState = JumpStates.Air;
            storedXVelocity = 0;
            canJump = false;
            hasLetGoOfJump = true;
            jumpTime = maxJumpTime;
            dashes = MaxDashes;
            dashTimer = dashRefreshTime;
            dashBar.CurrentDashPercent = (float)dashes / MaxDashes;
            dashBar.CooldownBarPercent = (float)dashTimer.Ticks / dashRefreshTime.Ticks;
            wallJumpCounter = 0;
        }

        public void MoveLeft(GameTimeWrapper gameTime)
        {
            MoveLeft(1, gameTime);
        }

        public void MoveLeft(float multiplier, GameTimeWrapper gameTime)
        {
            if (acceleration.X > 0)
            {
                ResetXAcceleration();
            }

            else if (jumpState == JumpStates.Ground)
            {
                acceleration.X -= 0.005f * multiplier;
            }
            else if (jumpState == JumpStates.Air)
            {
                acceleration.X -= 0.005f * multiplier;
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
                    acceleration.X -= 0.25f * multiplier;
                    wallJumpCounter = 0;
                }
            }
        }

        public void MoveRight(GameTimeWrapper gameTime)
        {
            MoveRight(1, gameTime);
        }

        public void MoveRight(float multiplier, GameTimeWrapper gameTime)
        {
            if (acceleration.X < 0)
            {
                ResetXAcceleration();
            }

            if (jumpState == JumpStates.Ground)
            {
                acceleration.X += 0.005f * multiplier;
            }
            else if (jumpState == JumpStates.Air)
            {
                acceleration.X += 0.005f * multiplier;
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
                    acceleration.X += 0.25f * multiplier;
                    wallJumpCounter = 0;
                }
            }
        }

        public void ResetXAcceleration()
        {
            if (jumpState == JumpStates.Ground)
            {
                if (acceleration.X >= 0)
                {
                    acceleration.X = Math.Max(0, acceleration.X - .1f);
                }
                else
                {
                    acceleration.X = Math.Min(0, acceleration.X + .1f);
                }
            } else if (jumpState == JumpStates.Air)
            {
                if (acceleration.X >= 0)
                {
                    acceleration.X = Math.Max(0, acceleration.X - .05f);
                }
                else
                {
                    acceleration.X = Math.Min(0, acceleration.X + .05f);
                }
            }
            
            wallJumpCounter = 0;
        }

        public void LetGo()
        {
            if (jumpState == JumpStates.Ground)
            {
                if (acceleration.X >= 0)
                {
                    acceleration.X = Math.Max(0, acceleration.X - .1f);
                }
                else
                {
                    acceleration.X = Math.Min(0, acceleration.X + .1f);
                }
            }
            else if (jumpState == JumpStates.Air)
            {
                if (acceleration.X >= 0)
                {
                    acceleration.X = Math.Max(0, acceleration.X - .0025f);
                }
                else
                {
                    acceleration.X = Math.Min(0, acceleration.X + .0025f);
                }
            }

            wallJumpCounter = 0;
        }

        public void Jump()
        {
            hasLetGoOfJump = false;
            if (canJump && jumpTime == maxJumpTime)
            {
                velocity.Y = -16f;
                
                if (jumpState == JumpStates.WallLeft)
                {
                    velocity.X = 8;
                    acceleration.X = 1f;
                }
                else if (jumpState == JumpStates.WallRight)
                {
                    velocity.X = -8;
                    acceleration.X = -1f;
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
                if (velocity == Vector2.Zero)
                {
                    position += Vector2.Normalize(new Vector2(0, 1)) * DashDistance;
                }
                else
                {
                    position += Vector2.Normalize(velocity) * DashDistance;
                }
            }
        }

        private void UpdateDebugTexts(GameTimeWrapper gameTime)
        {
            canJumpText.text = $"{nameof(canJump)}: {canJump.ToString()}";
            hasLetGoOfJumpText.text = $"{nameof(hasLetGoOfJump)}: {hasLetGoOfJump.ToString()}";
            jumpTimeText.text = $"{nameof(jumpTime)}: {jumpTime.ToString()}";
            velocityText.text = $"{nameof(velocity)}: {velocity.ToString()}";
        }

        private bool isOutOfBounds()
        {
            return position.Y > yDeathThreshold;
        }

        public override void Update(GameTimeWrapper gameTime)
        {
            if (isOutOfBounds())
            {
                velocity = Vector2.Zero;
                acceleration = Vector2.Zero;
                position = spawnPoint;
                jumpState = JumpStates.Ground;
            }

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
                velocity.X *= 0.99f;
            }
            else
            {
                // air friction
                velocity.X *= 0.99f;
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

            if (jumpState == JumpStates.WallLeft || jumpState == JumpStates.WallRight)
            {
                float lowestWallSlideSpeed = 20;
                if (velocity.Y <= lowestWallSlideSpeed)
                {
                    velocity.Y += (GameHelpers.Gravity / 1.7f) * (float)gameTime.GameSpeed;
                }
                else
                {
                    velocity.Y = lowestWallSlideSpeed;
                }
            }
            else
            {
                if (!hasLetGoOfJump && canJump)
                {
                    velocity.Y += (GameHelpers.Gravity / 3f) * (float)gameTime.GameSpeed;
                }
                else
                {
                    velocity.Y += GameHelpers.Gravity * (float)gameTime.GameSpeed;
                }
            }
            
            acceleration.X = MathHelper.Clamp(acceleration.X, -1.8f, 1.8f);

            velocity.X = MathHelper.Clamp(velocity.X, -14f, 14f);

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
            bool anyCollision = false;
            bool onGround = false;
            foreach (var wall in walls)
            {
                GLX.Collisions.MTV? mtv = GLX.Collisions.HelperMethods.Colliding(polygon, wall.polygon);
                if (mtv != null)
                {
                    anyCollision = true;
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

                    if (mtv.Value.vector.Y == 0 && !onGround)
                    {
                        if (position.X > wall.center.X)
                        {
                            jumpState = JumpStates.WallLeft;

                            if (wall.isMoving)
                            {
                                if (wall.isMovingLeft)
                                {
                                    position.X -= .3f;
                                }
                                else
                                {
                                    position.X += .3f;
                                }
                            }
                        }
                        else if (position.X < wall.center.X)
                        {
                            jumpState = JumpStates.WallRight;

                            if (wall.isMoving)
                            {
                                if (wall.isMovingLeft)
                                {
                                    position.X -= .3f;
                                }
                                else
                                {
                                    position.X += .3f;
                                }
                            }
                        }
                    }
                    else
                    {
                        onGround = true;
                        jumpState = JumpStates.Ground;
                        velocity.Y = 0;
                        acceleration.Y = 0;
                        ResetJump();

                        if (wall.isMoving)
                        {
                            if (wall.isMovingLeft)
                            {
                                position.X -= .3f;
                            }
                            else
                            {
                                position.X += .3f;
                            }
                        }

                        justHitWall = false;
                    }
                }
            }

            if (!anyCollision)
            {
                justHitWall = false;
                jumpState = JumpStates.Air;
            }
            else if (!onGround)
            {
                canJump = false;
                acceleration = Vector2.Zero;
                velocity.X = 0;
                if (velocity.Y < 0)
                {
                    //velocity.Y += .005f;
                }
                else if (!justHitWall)
                {
                    justHitWall = true;
                    velocity.Y /= 2f;
                    acceleration.Y = 3f;
                }
                
                ResetJump();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (jumpState == JumpStates.Ground && acceleration.X != 0) {
                if (acceleration.X > 0)
                {
                    animations.active = true;
                    spriteEffects = SpriteEffects.None;
                } else if (acceleration.X < 0)
                {
                    animations.active = true;
                    spriteEffects = SpriteEffects.FlipHorizontally;
                }
            }
            else if (jumpState == JumpStates.Air && acceleration.X != 0)
            {
                if (acceleration.X > 0)
                {
                    spriteEffects = SpriteEffects.None;
                } else if (acceleration.X < 0)
                {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                }
            }
            else
            {
                animations.active = false;

                if (jumpState == JumpStates.Ground)
                {
                    animations.currentFrame = 0;
                }
                else if (jumpState == JumpStates.Air)
                {
                    animations.currentFrame = 1;
                }
            }
            base.Draw(spriteBatch);
            dashBar.Draw(spriteBatch);
            if (dashIndicator.visible)
            {
                dashIndicator.Draw(spriteBatch);
            }
        }
    }
}
