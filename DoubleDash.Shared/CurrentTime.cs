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

        private List<GameTimeManager> gameTimes;
        public TextItem text;
        public Speeds speed;
        private SoundEffect speedUp;
        private SoundEffect speedUpSlower;
        private SoundEffect slowDown;
        private SoundEffect slowDownSlower;
        
        public CurrentTime(SpriteFont spriteFont, SoundEffect speedUp, SoundEffect slowDown, SoundEffect speedUpSlower, SoundEffect slowDownSlower)
        {
            gameTimes = new List<GameTimeManager>();
            text = new TextItem(spriteFont, "Speed: Normal");
            text.color = Color.White;
            speed = Speeds.Normal;
            this.speedUp = speedUp;
            this.speedUpSlower = speedUpSlower;
            this.slowDown = slowDown;
            this.slowDownSlower = slowDownSlower;
        }

        public void AddGameTime(GameTimeManager gameTimeManager)
        {
            gameTimes.Add(gameTimeManager);
        }

        public void SetToSlow()
        {
            speed = Speeds.Slow;
            foreach (var time in gameTimes)
            {
                time.MultiplyOriginalSpeed(0.5m);
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
                time.Reset();
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
                time.MultiplyOriginalSpeed(1.25m);
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

        public void Draw(SpriteBatch spriteBatch)
        {
            if (text.visible)
            {
                text.Draw(spriteBatch);
            }
        }
    }
}
