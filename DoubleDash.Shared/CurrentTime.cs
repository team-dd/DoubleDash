using System;
using System.Collections.Generic;
using System.Text;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoubleDash
{
    public class CurrentTime
    {
        public enum Speeds
        {
            Slow,
            Normal,
            Fast
        }

        private GameTimeWrapper gameTime;
        public TextItem text;
        private Speeds speed;
        
        public CurrentTime(GameTimeWrapper gameTime, SpriteFont spriteFont)
        {
            text = new TextItem(spriteFont, "1.0x");
            text.color = Color.Black;
            this.gameTime = gameTime;
            gameTime.GameSpeed = 1m;
            speed = Speeds.Normal;
        }

        public void SetToSlow()
        {
            speed = Speeds.Slow;
            gameTime.GameSpeed = 0.5m;
            text.text = "0.5x";
            text.alpha = 1;
            text.visible = true;
        }

        public void SetToNormal()
        {
            speed = Speeds.Normal;
            gameTime.GameSpeed = 1m;
            text.text = "1.0x";
            text.alpha = 1;
            text.visible = true;
        }

        public void SetToFast()
        {
            speed = Speeds.Fast;
            gameTime.GameSpeed = 1.75m;
            text.text = "2.0x";
            text.alpha = 1;
            text.visible = true;
        }

        public void SetSlower()
        {
            if (speed == Speeds.Fast)
            {
                SetToNormal();
            }
            else if (speed == Speeds.Normal)
            {
                SetToSlow();
            }
        }

        public void SetFaster()
        {
            if (speed == Speeds.Slow)
            {
                SetToNormal();
            }
            else if (speed == Speeds.Normal)
            {
                SetToFast();
            }
        }

        public void Update(GameTimeWrapper gameTime)
        {
            /*text.alpha -= 0.01f;
            if (text.alpha <= 0)
            {
                text.visible = false;
                text.alpha = 0;
            }*/
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (text.visible)
            {
                text.Draw(spriteBatch);
            }
        }
    }
}
