using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GLX;

namespace DoubleDash
{
    public class RainManager
    {
        private const int NumberOfLines = 150;

        private GraphicsDeviceManager graphics;
        private List<Rain> lines;
        private TimeSpan spawnCountdown;
        private TimeSpan SpawnTime { get { return TimeSpan.FromMilliseconds(10); } }

        public RainManager(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            lines = new List<Rain>(NumberOfLines);
            for (int i = 0; i < NumberOfLines; i++)
            {
                Rain rain = new Rain(graphics);
                rain.color = new Color(200, 200, 200);
                rain.thickness = (i % 3) + 1;
                rain.visible = false;
                rain.velocity = new Vector2(0, ((i % 3) + 4) * 12);
                lines.Add(rain);
            }
            spawnCountdown = SpawnTime;
        }

        public void Update(GameTimeWrapper gameTime, VirtualResolutionRenderer vrr, Camera camera)
        {
            spawnCountdown -= gameTime.ElapsedGameTime;
            foreach (var line in lines)
            {
                if (line.visible)
                {
                    line.Update(gameTime);
                }
                else
                {
                    if (spawnCountdown <= TimeSpan.Zero)
                    {
                        float magnitude = (float)World.random.NextDouble(150, 350);
                        line.point1 = new Vector2(World.random.Next(0, (int)vrr.WindowResolution.Width), 0);
                        line.point1 = Vector2.Transform(line.point1, camera.InverseTransform);
                        line.point2 = line.point1 - (new Vector2(0, 120) - line.velocity) / 50 * magnitude;
                        line.visible = true;
                        line.block.visible = false;
                        spawnCountdown = SpawnTime;
                    }
                }
                if (line.point2.Y > Vector2.Transform(new Vector2(0, vrr.WindowResolution.Height), camera.InverseTransform).Y)
                {
                    line.visible = false;
                }
                if (line.visible)
                {
                    line.UpdatePolygon();
                }
            }
        }

        public void CheckCollisions(List<Block> walls)
        {
            foreach (var line in lines)
            {
                if (line.visible)
                {
                    line.CheckCollisions(walls);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var line in lines)
            {
                if (line.visible)
                {
                    line.Draw(spriteBatch);
                }
            }
        }
    }
}
