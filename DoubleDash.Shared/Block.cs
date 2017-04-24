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
        public Vector2 center;

        private Line top;
        private Line right;
        private Line bottom;
        private Line left;

        public bool drawTop;
        public bool drawRight;
        public bool drawBottom;
        public bool drawLeft;

        public bool isMoving;
        Vector2 origin;
        public bool isMovingLeft;

        bool blockColorTimeIncreasing;
        TimeSpan blockColorSwitchTime;
        TimeSpan colorSwitchTime;
        Color lowColor;
        Color highColor;
        static Color[] RAINBOW = {
            Color.Red,
            Color.Orange,
            Color.Yellow,
            Color.Green,
            Color.LightBlue,
            Color.MediumPurple
        };
        static Random r = new Random();
        Color lineColor;
        int currentColorIndex;

        public Block(Vector2 position, Size size, bool isMoving, GraphicsDeviceManager graphics, Color outlineColor) : base(graphics)
        {
            color = Color.Black;
            base.position = position;
            base.DrawSize = size;

            if (outlineColor != null)
            {
                lineColor = outlineColor;
            }
            else
            {
                lineColor = Color.White;
            }

            if (isMoving)
            {
                velocity = new Vector2(-3f, 0);
            }

            top = CreateLine(graphics, GameHelpers.BlockOutlineColor, GameHelpers.BlockOutlineThickness);
            right = CreateLine(graphics, GameHelpers.BlockOutlineColor, GameHelpers.BlockOutlineThickness);
            bottom = CreateLine(graphics, GameHelpers.BlockOutlineColor, GameHelpers.BlockOutlineThickness);
            left = CreateLine(graphics, GameHelpers.BlockOutlineColor, GameHelpers.BlockOutlineThickness);
            drawTop = true;
            drawRight = true;
            drawBottom = true;
            drawLeft = true;

            this.isMoving = isMoving;
            isMovingLeft = true;
            origin = position;

            colorSwitchTime = TimeSpan.FromMilliseconds(2500);
            blockColorSwitchTime = colorSwitchTime;
            blockColorTimeIncreasing = false;
            lowColor = GameHelpers.BlockColor1;
            highColor = GameHelpers.BlockColor2;

            UpdatePolygon();
        }

        private Line CreateLine(GraphicsDeviceManager graphics, Color color, float thickness)
        {
            Line line = new Line(graphics);
            //line.color = color;
            line.color = lineColor;
            line.thickness = thickness;
            return line;
        }

        public void Update(GameTimeWrapper gameTime, bool hasStartedLevel)
        {
            MoveBlock(hasStartedLevel);
            if (GameHelpers.LerpBlockColor)
            {
                UpdateColor(gameTime);
            }
            else
            {
                color = GameHelpers.BlockColor1;
            }
            base.Update(gameTime);
            UpdatePolygon();
        }

        public void MoveBlock(bool hasStartedLevel)
        {
            if (!isMoving || !hasStartedLevel)
            {
                return;
            }

            if (isMovingLeft)
            {
                if (origin.X - base.position.X > 150)
                {
                    velocity.X = 3f;
                    isMovingLeft = false;
                }
            }
            else
            {
                if (base.position.X - origin.X > 150)
                {
                    velocity.X = -3f;
                    isMovingLeft = true;
                }
            }
        }

        public void UpdateColor(GameTimeWrapper gameTime)
        {
            if (blockColorTimeIncreasing)
            {
                blockColorSwitchTime += gameTime.ElapsedGameTime;
                if (blockColorSwitchTime >= colorSwitchTime)
                {
                    blockColorSwitchTime = colorSwitchTime;
                    blockColorTimeIncreasing = false;
                }
            }
            else
            {
                blockColorSwitchTime -= gameTime.ElapsedGameTime;
                if (blockColorSwitchTime <= TimeSpan.Zero)
                {
                    blockColorSwitchTime = TimeSpan.Zero;
                    blockColorTimeIncreasing = true;
                }
            }
            color = Color.Lerp(lowColor, highColor, (float)blockColorSwitchTime.Ticks / colorSwitchTime.Ticks);
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
            center = new Vector2(position.X + DrawSize.Width / 2,
                position.Y + DrawSize.Height / 2);
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
            
            if (GameHelpers.DrawBlockOutlines)
            {
                if (drawTop)
                {
                    top.Draw(spriteBatch);
                }
                if (drawRight)
                {
                    right.Draw(spriteBatch);
                }
                if (drawBottom)
                {
                    bottom.Draw(spriteBatch);
                }
                if (drawLeft)
                {
                    left.Draw(spriteBatch);
                }
            }
        }
    }
}
