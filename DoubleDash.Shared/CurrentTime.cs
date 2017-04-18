using System;
using System.Collections.Generic;
using System.Text;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
        private SoundEffect speedUp;
        private SoundEffect speedUpSlower;
        private SoundEffect slowDown;
        private SoundEffect slowDownSlower;
        
        public CurrentTime(SpriteFont spriteFont, SoundEffect speedUp, SoundEffect slowDown, SoundEffect speedUpSlower, SoundEffect slowDownSlower)
        {
            gameTimes = new List<Tuple<GameTimeWrapper, decimal>>();
            text = new TextItem(spriteFont, "Normal");
            text.color = Color.White;
            speed = Speeds.Normal;
            this.speedUp = speedUp;
            this.speedUpSlower = speedUpSlower;
            this.slowDown = slowDown;
            this.slowDownSlower = slowDownSlower;
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
            text.text = "Speed: Slow";
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
            text.text = "Speed: Normal";
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
            text.text = "Speed: Fast";
            text.alpha = 1;
            text.visible = true;
        }

        public void SetSlower()
        {
            if (speed == Speeds.Fast)
            {
                slowDown.Play();
                SetToNormal();
            }
            else if (speed == Speeds.Normal)
            {
                slowDownSlower.Play();
                SetToSlow();
            }
        }

        public void SetFaster()
        {
            if (speed == Speeds.Slow)
            {
                speedUpSlower.Play();
                SetToNormal();
            }
            else if (speed == Speeds.Normal)
            {
                speedUp.Play();
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
