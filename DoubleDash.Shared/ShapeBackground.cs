using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GLX;

namespace DoubleDash
{
    public class ShapeBackground
    {
        private const int NumberOfLines = 160;

        private GraphicsDeviceManager graphics;
        private List<ShapeTriangle> lines;
        private TimeSpan spawnCountdown;
        private TimeSpan SpawnTime { get { return TimeSpan.FromMilliseconds(10); } }
        public Color color;

        public ShapeBackground(GraphicsDeviceManager graphics, Color color)
        {
            this.graphics = graphics;
            lines = new List<ShapeTriangle>(NumberOfLines);
            for (int i = 1; i <= NumberOfLines; i++)
            {
                ShapeTriangle triangle = new ShapeTriangle(graphics, i * .025f, color);
                lines.Add(triangle);
            }
            spawnCountdown = SpawnTime;
        }

        public void Update(GameTimeWrapper gameTime)
        {
            for (int i = 0; i < NumberOfLines; i++)
            {
                lines[i].Update();
            }
        }

        public void UpdateColor(Color newColor)
        {
            color = newColor;
            for (int i = 0; i < NumberOfLines; i++)
            {
                lines[i].UpdateColor(newColor);
            }
        }

        /*public void CheckCollisions(List<Block> walls)
        {
            foreach (var line in lines)
            {
                if (line.visible)
                {
                    line.CheckCollisions(walls);
                }
            }
        }*/

        public void Draw()
        {
            foreach (var line in lines)
            {
                line.Draw();
            }
        }
    }
}
