using System;
using System.Collections.Generic;
using System.Text;
using GLX;
using GLX.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoubleDash
{
    public class Rain : Line
    {
        private Polygon polygon;
        public Sprite block;

        public Rain(GraphicsDeviceManager graphics) : base(graphics)
        {
            block = new Sprite(graphics);
            block.DrawSize = new Size(5);
            block.visible = false;
        }

        public override void Update()
        {
            base.Update();
            UpdatePolygon();
        }

        public void UpdatePolygon()
        {
            List<Vector2> vertices = new List<Vector2>();
            vertices.Add(point1);
            vertices.Add(point2);
            polygon = new Polygon(vertices);
        }

        public void CheckCollisions(List<Block> walls)
        {
            foreach (var wall in walls)
            {
                GLX.Collisions.MTV? mtv = GLX.Collisions.HelperMethods.Colliding(polygon, wall.polygon);
                if (mtv != null)
                {
                    Vector2 vector = mtv.Value.vector;
                    // you would think that you could just multiply vector by -1 if the second case
                    // is true but for some reason that doesn't work...
                    if (point2.X > wall.position.X)
                    {
                        point2.X += vector.X * mtv.Value.magnitude;
                    }
                    else if (point2.X < wall.position.X)
                    {
                        point2.X -= vector.X * mtv.Value.magnitude;
                    }

                    if (point2.Y < wall.position.Y)
                    {
                        point2.Y += vector.Y * mtv.Value.magnitude;
                    }
                    else if (point2.Y > wall.position.Y)
                    {
                        point2.Y -= vector.Y * mtv.Value.magnitude;
                    }
                    block.position = point2;
                    visible = false;
                    block.visible = true;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                base.Draw(spriteBatch);
            }
            if (block.visible)
            {
                block.Draw(spriteBatch);
            }
        }
    }
}
