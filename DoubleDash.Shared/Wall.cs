using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoubleDash
{
    public class Wall
    {
        public Sprite sprite;
        public Wall(GraphicsDeviceManager graphics)
        {
            sprite = new Sprite(graphics);
        }

        public void Update(GameTimeWrapper gameTime, Player player)
        {
            if (player.rectangle.Intersects(sprite.rectangle))
            {
                SortedDictionary<float, Action> moveFuncs = new SortedDictionary<float, Action>();
                if (player.rectangle.Right < sprite.rectangle.Center.X)
                {
                    player.jumpState = Player.JumpStates.WallRight;
                    float distance = Math.Abs(player.position.X - (sprite.rectangle.Left - player.tex.Height / 2f));
                    moveFuncs.Add(distance, () =>
                    {
                        if (player.velocity.X != 0)
                        {
                            player.storedXVelocity = player.velocity.X;
                        }
                        player.velocity.X = 0;
                        player.velocity.Y /= 1.5f;
                        player.position.X = sprite.rectangle.Left - player.tex.Height / 2f;
                    });
                }
                else if (player.rectangle.Left > sprite.rectangle.Center.X)
                {
                    player.jumpState = Player.JumpStates.WallLeft;
                    float distance = Math.Abs(player.position.X - (sprite.rectangle.Right + player.tex.Height / 2f));
                    moveFuncs.Add(distance, () =>
                    {
                        if (player.velocity.X != 0)
                        {
                            player.storedXVelocity = player.velocity.X;
                        }
                        player.velocity.X = 0;
                        player.velocity.Y /= 4;
                        player.position.X = sprite.rectangle.Right + player.tex.Height / 2f;
                    });
                }

                if (player.rectangle.Bottom < sprite.rectangle.Center.Y)
                {
                    player.jumpState = Player.JumpStates.Ground;
                    float distance = Math.Abs(player.position.Y - (sprite.rectangle.Top - player.tex.Height / 2f));
                    moveFuncs.Add(distance, () =>
                    {
                        player.velocity.Y = 0;
                        player.position.Y = sprite.rectangle.Top - player.tex.Height / 2f;
                    });
                }
                else if (player.rectangle.Top < sprite.rectangle.Center.Y)
                {
                    player.jumpState = Player.JumpStates.Ground;
                    float distance = Math.Abs(player.position.Y - (sprite.rectangle.Bottom + player.tex.Height / 2f));
                    moveFuncs.Add(distance, () =>
                    {
                        player.velocity.Y = 0;
                        player.position.Y = sprite.rectangle.Bottom + player.tex.Height / 2f;
                    });
                }

                if (moveFuncs.Count > 0)
                {
                    moveFuncs.First().Value.Invoke();
                }
            }
            sprite.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }
    }
}
