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

        private List<Tuple<GameTimeWrapper, decimal>> gameTimes;
        public TextItem text;
        private Speeds speed;
        
        public CurrentTime(SpriteFont spriteFont)
        {
            gameTimes = new List<Tuple<GameTimeWrapper, decimal>>();
            text = new TextItem(spriteFont, "1.0x");
            text.color = Color.Black;
            speed = Speeds.Normal;
        }

        public void AddGameTime(GameTimeWrapper gameTime, decimal baseTime)
        {
            gameTimes.Add(new Tuple<GameTimeWrapper, decimal>(gameTime, baseTime));
        }

        public void SetToSlow()
        {
            speed = Speeds.Slow;
            foreach (var time in gameTimes)
            {
                time.Item1.GameSpeed = time.Item2 * 0.5m;
            }
            text.text = "0.5x";
            text.alpha = 1;
            text.visible = true;
        }

        public void SetToNormal()
        {
            speed = Speeds.Normal;
            foreach (var time in gameTimes)
            {
                time.Item1.GameSpeed = time.Item2;
            }
            text.text = "1.0x";
            text.alpha = 1;
            text.visible = true;
        }

        public void SetToFast()
        {
            speed = Speeds.Fast;
            foreach (var time in gameTimes)
            {
                time.Item1.GameSpeed = time.Item2 * 1.75m;
            }
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
