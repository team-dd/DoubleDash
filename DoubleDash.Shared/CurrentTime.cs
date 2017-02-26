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
        }

        public void SetToSlow()
        {
            gameTime.GameSpeed = 0.5m;
            text.text = "0.5x";
        }

        public void SetToNormal()
        {
            gameTime.GameSpeed = 1m;
            text.text = "1.0x";
        }

        public void SetToFast()
        {
            gameTime.GameSpeed = 2m;
            text.text = "2.0x";
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            text.Draw(spriteBatch);
        }
    }
}
