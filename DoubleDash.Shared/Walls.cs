using System;
using System.Collections.Generic;
using System.Text;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoubleDash
{
    public class Walls
    {
        private GraphicsDeviceManager graphics;
        public List<Wall> walls;

        public Walls(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            walls = new List<Wall>();
        }

        public void Create(Size size, Vector2 position)
        {
            Wall wall = new Wall(graphics);
            wall.sprite.DrawSize = size;
            wall.sprite.position = position;
            walls.Add(wall);
        }

        public void Update(GameTimeWrapper gameTime, Player player)
        {
            foreach (var wall in walls)
            {
                wall.Update(gameTime, player);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var wall in walls)
            {
                wall.Draw(spriteBatch);
            }
        }
    }
}
