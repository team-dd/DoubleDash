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
        private List<ShapeTriangle> triangles;
        private List<ShapeSquare> squares;
        private List<ShapeTwenty> twenties;
        public Color color;
        public Mode mode;

        public enum Mode {
            Triangle, Square, Twenty
        };

        public ShapeBackground(GraphicsDeviceManager graphics, Color color)
        {
            this.graphics = graphics;
            this.mode = Mode.Triangle;
            triangles = new List<ShapeTriangle>(NumberOfLines);
            squares = new List<ShapeSquare>(NumberOfLines);
            twenties = new List<ShapeTwenty>(NumberOfLines);
            for (int i = 1; i <= NumberOfLines; i++)
            {
                ShapeTriangle triangle = new ShapeTriangle(graphics, i * .025f, color);
                triangles.Add(triangle);
            }
            for (int i = 1; i <= NumberOfLines; i++)
            {
                ShapeSquare square = new ShapeSquare(graphics, i * .025f, color);
                squares.Add(square);
            }
            for (int i = 1; i <= NumberOfLines; i++)
            {
                ShapeTwenty twenty = new ShapeTwenty(graphics, i * .025f, color);
                twenties.Add(twenty);
            }
        }

        public void ChangeMode()
        {
            switch (mode)
            {
                case Mode.Triangle:
                    mode = Mode.Square;
                    break;
                case Mode.Square:
                    mode = Mode.Twenty;
                    break;
                case Mode.Twenty:
                    mode = Mode.Triangle;
                    break;
            }
        }

        public void Update(GameTimeWrapper gameTime)
        {
            for (int i = 0; i < NumberOfLines; i++)
            {
                if (mode == Mode.Triangle)
                {
                    triangles[i].Update();
                }
                else if (mode == Mode.Square)
                {
                    squares[i].Update();
                }
                else if (mode == Mode.Twenty)
                {
                   twenties[i].Update();
                }
            }
        }

        public void UpdateColor(Color newColor)
        {
            color = newColor;
            for (int i = 0; i < NumberOfLines; i++)
            {
                if (mode == Mode.Triangle)
                {
                    triangles[i].UpdateColor(newColor);
                }
                else if (mode == Mode.Square)
                {
                    squares[i].UpdateColor(newColor);
                }
                else if (mode == Mode.Twenty)
                {
                    twenties[i].UpdateColor(newColor);
                }
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
            if (mode == Mode.Triangle)
            {
                foreach (var triangle in triangles)
                {
                    triangle.Draw();
                }
            }
            else if (mode == Mode.Square)
            {
                foreach (var square in squares)
                {
                    square.Draw();
                }
            }
            else if (mode == Mode.Twenty)
            {
                foreach (var twenty in twenties)
                {
                    twenty.Draw();
                }
            }
        }
    }
}
