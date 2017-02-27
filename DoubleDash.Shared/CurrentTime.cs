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
        private GameTimeWrapper gameTime;
        public TextItem text;
        
        public CurrentTime(GameTimeWrapper gameTime, SpriteFont spriteFont)
        {
            text = new TextItem(spriteFont, "1.0x");
            text.color = Color.Black;
            this.gameTime = gameTime;
            gameTime.GameSpeed = 2m;
        }

        public void SetToSlow()
        {
            gameTime.GameSpeed = 1.5m;
            text.text = "0.5x";
            text.alpha = 1;
            text.visible = true;
        }

        public void SetToNormal()
        {
            gameTime.GameSpeed = 2m;
            text.text = "1.0x";
            text.alpha = 1;
            text.visible = true;
        }

        public void SetToFast()
        {
            gameTime.GameSpeed = 3m;
            text.text = "2.0x";
            text.alpha = 1;
            text.visible = true;
        }

        public void Update(GameTimeWrapper gameTime)
        {
            text.alpha -= 0.01f;
            if (text.alpha <= 0)
            {
                text.visible = false;
                text.alpha = 0;
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
