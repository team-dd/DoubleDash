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
        public List<Block> walls;

        public Walls(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            walls = new List<Block>();
        }

        public void Create(Size size, Vector2 position, bool isMoving)
        {
            Block block = new Block(position, size, isMoving, graphics);
            walls.Add(block);
        }

        public void Update(GameTimeWrapper gameTime)
        {
            foreach (var wall in walls)
            {
                wall.Update(gameTime);
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
