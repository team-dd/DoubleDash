using System;
using System.Collections.Generic;
using System.Text;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoubleDash
{
    public class DashBar
    {
        public const int BarWidth = 500;
        public const int BarHeight = 25;

        private Sprite fullBar;
        private Sprite currentDashBar;
        private Sprite cooldownBar;

        private Vector2 position;
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                fullBar.position = position;
                currentDashBar.position = position;
                cooldownBar.position = position;
            }
        }

        public float CurrentDashPercent
        {
            set
            {
                currentDashBar.DrawSize = new Size(BarWidth * value, BarHeight);
            }
        }

        public float CooldownBarPercent
        {
            set
            {
                cooldownBar.DrawSize = new Size(BarWidth * value, BarHeight);
            }
        }

        public DashBar(GraphicsDeviceManager graphics)
        {
            fullBar = new Sprite(graphics);
            fullBar.DrawSize = new Size(BarWidth, BarHeight);
            fullBar.color = Color.Black;
            currentDashBar = new Sprite(graphics);
            currentDashBar.DrawSize = new Size(0, BarHeight);
            currentDashBar.color = Color.White;
            cooldownBar = new Sprite(graphics);
            cooldownBar.DrawSize = new Size(0, BarHeight);
            cooldownBar.color = Color.Gray;
        }

        public void Update(GameTimeWrapper gameTime)
        {
            cooldownBar.position = new Vector2(position.X + currentDashBar.DrawSize.Width,
                position.Y);
            fullBar.Update(gameTime);
            currentDashBar.Update(gameTime);
            cooldownBar.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            fullBar.Draw(spriteBatch);
            cooldownBar.Draw(spriteBatch);
            currentDashBar.Draw(spriteBatch);
        }
    }
}
