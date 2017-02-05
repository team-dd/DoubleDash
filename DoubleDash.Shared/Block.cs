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

        private Line top;
        private Line right;
        private Line bottom;
        private Line left;

        public Block(Vector2 position, Size size, GraphicsDeviceManager graphics) : base(graphics)
        {
            color = Color.Black;
            base.position = position;
            base.DrawSize = size;

            top = new Line(graphics);
            right = new Line(graphics);
            bottom = new Line(graphics);
            left = new Line(graphics);

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
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] = Vector2.Transform((vertices[i] - position), spriteTransform);
            }
            top.point1 = vertices[0];
            top.point2 = vertices[1];
            right.point1 = top.point2;
            right.point2 = vertices[2];
            bottom.point1 = right.point2;
            bottom.point2 = vertices[3];
            left.point1 = bottom.point2;
            left.point2 = top.point1;
            polygon = new Polygon(vertices);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            top.Draw(spriteBatch);
            right.Draw(spriteBatch);
            bottom.Draw(spriteBatch);
            left.Draw(spriteBatch);
        }
    }
}
