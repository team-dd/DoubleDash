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
        private const int NumberOfLines = 120;

        private GraphicsDeviceManager graphics;
        private List<ShapeNPoints> shapes;
        public Color color;
        public int n;
        static Random rand = new Random();

        public ShapeBackground(GraphicsDeviceManager graphics, Color color)
        {
            this.graphics = graphics;
            n = rand.Next(12, 40);
            CreateShapes();
        }

        public void CreateShapes()
        {
            shapes = new List<ShapeNPoints>(NumberOfLines);

            for (int i = 1; i <= NumberOfLines; i++)
            {
                ShapeNPoints shape = new ShapeNPoints(graphics, i * .025f, color, n);
                shapes.Add(shape);
            }
        }

        public void ChangeMode()
        {
            int pren = rand.Next(6, 20);
            n = pren - pren % 2;
            CreateShapes();
        }

        public void Update(GameTimeWrapper gameTime)
        {
            for (int i = 0; i < NumberOfLines; i++)
            {
                shapes[i].Update();    
            }
        }

        public void UpdateColor(Color newColor)
        {
            color = newColor;
            for (int i = 0; i < NumberOfLines; i++)
            {
                shapes[i].UpdateColor(newColor);
            }
        }

        public void Draw()
        {
            foreach (ShapeNPoints shape in shapes)
            {
                shape.Draw();
            }
        }
    }
}
