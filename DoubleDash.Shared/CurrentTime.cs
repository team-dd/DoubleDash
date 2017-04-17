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

        /// <summary>
        /// List of Tuples of GameTimeWrappers
        /// Item1 - GameTimeWrapper
        /// Item2 - GameTimeWrapper original speed
        /// </summary>
        private List<Tuple<GameTimeWrapper, decimal>> gameTimes;
        public TextItem text;
        private Speeds speed;
        
        public CurrentTime(SpriteFont spriteFont)
        {
            gameTimes = new List<Tuple<GameTimeWrapper, decimal>>();
            text = new TextItem(spriteFont, "Normal");
            text.color = Color.White;
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
            text.text = "Slow";
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
            text.text = "Normal";
            text.alpha = 1;
            text.visible = true;
        }

        public void SetToFast()
        {
            speed = Speeds.Fast;
            foreach (var time in gameTimes)
            {
                time.Item1.GameSpeed = time.Item2 * 1.25m;
            }
            text.text = "Fast";
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
