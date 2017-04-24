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

        private readonly int[] PossibleNs = new int[] {
            6,
            8,
            10,
            12,
            14,
            20,
            24,
            36,
            38
        };

        private GraphicsDeviceManager graphics;
        private List<ShapeNPoints> shapes;
        public Color color;
        public int n;
        static Random rand = new Random();

        public ShapeBackground(GraphicsDeviceManager graphics, Color color)
        {
            this.graphics = graphics;
            n = PossibleNs[rand.Next(PossibleNs.Length)];
            CreateShapes();
        }

        public void CreateShapes()
        {
            shapes = new List<ShapeNPoints>(NumberOfLines);

            for (int i = 1; i <= NumberOfLines; i++)
            {
                ShapeNPoints shape = new ShapeNPoints(graphics, i*n/32.0f, color * .35f, n);
                shapes.Add(shape);
            }
        }

        public void ChangeMode()
        {
            int[] PossibleNsMinusCurrentN = (int[]) PossibleNs.Clone();
            n = PossibleNsMinusCurrentN[rand.Next(PossibleNsMinusCurrentN.Length)];
            CreateShapes();
            Update();
        }

        public void Update()
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
