using System;
using System.Collections.Generic;
using System.Text;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoubleDash
{
    public class GameTimer
    {
        TimeSpan timer;
        public TextItem timerText;
        bool active;

        public GameTimer(SpriteFont spriteFont)
        {
            timerText = new TextItem(spriteFont);
            timerText.origin = Vector2.Zero;
            timer = TimeSpan.Zero;
            active = false;
        }

        public void Start()
        {
            active = true;
        }

        public void Stop()
        {
            active = false;
        }

        public void Reset()
        {
            timer = TimeSpan.Zero;
        }

        public void Update(GameTimeWrapper gameTime)
        {
            if (active)
            {
                timer += gameTime.ElapsedGameTime;
            }
            timerText.text = timer.ToString();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            timerText.Draw(spriteBatch);
        }
    }
}
