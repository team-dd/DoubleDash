using System;
using System.Collections.Generic;
using System.Text;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoubleDash
{
    public class StarBackgroundManager
    {
        private GraphicsDeviceManager graphics;
        public List<StarBackground> backgrounds;

        public StarBackgroundManager(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            backgrounds = new List<StarBackground>();
        }

        public StarBackground Create(VirtualResolutionRenderer vrr)
        {
            return new StarBackground(graphics, vrr);
        }

        public void Create(int number, VirtualResolutionRenderer vrr)
        {
            for (int i = 0; i < number; i++)
            {
                backgrounds.Add(new StarBackground(graphics, vrr));
            }
        }

        public void MoveUp(float distance)
        {
            Move(new Vector2(0, -distance));
        }

        public void MoveDown(float distance)
        {
            Move(new Vector2(0, distance));
        }

        public void MoveLeft(float distance)
        {
            Move(new Vector2(-distance, 0));
        }

        public void MoveRight(float distance)
        {
            Move(new Vector2(distance, 0));
        }

        private void Move(Vector2 vector)
        {
            for (int i = 1; i <= backgrounds.Count; i++)
            {
                backgrounds[i - 1].Move(vector * ((float)i / backgrounds.Count));
            }
        }

        public void Update(GameTimeWrapper gameTime, Camera camera)
        {
            foreach (var background in backgrounds)
            {
                background.Update(gameTime, camera);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var background in backgrounds)
            {
                background.Draw(spriteBatch);
            }
        }
    }
}
