using System;
using System.Collections.Generic;
using System.Text;
using GLX;
using GLX.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoubleDash
{
    public class Block : Sprite
    {
        public Polygon polygon;

        public Block(Vector2 position, Size size, GraphicsDeviceManager graphics) : base(graphics)
        {
            color = Color.Black;
            base.position = position;
            base.DrawSize = size;
            UpdatePolygon();
        }

        public void Update(GameTimeWrapper gameTime)
        {
            base.Update(gameTime);
            UpdatePolygon();
        }

        private void UpdatePolygon()
        {
            List<Vector2> vertices = new List<Vector2>();
            vertices.Add(position);
            vertices.Add(new Vector2(position.X + DrawSize.Width, position.Y));
            vertices.Add(new Vector2(position.X + DrawSize.Width, position.Y + DrawSize.Height));
            vertices.Add(new Vector2(position.X, position.Y + DrawSize.Height));
            polygon = new Polygon(vertices);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
