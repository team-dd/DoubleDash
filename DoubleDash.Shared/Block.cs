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

        public bool isMoving;
        Vector2 origin;
        public bool isMovingLeft;

        bool blockColorTimeIncreasing;
        TimeSpan blockColorSwitchTime;
        TimeSpan colorSwitchTime;
        Color lowColor;
        Color highColor;
        // http://www.colourlovers.com/palette/127957/A_Beach_in_the_Stars
        static Color[] COLORS_1 = {
            new Color(247, 108, 180),
            new Color(248, 194, 99),
            new Color(187, 240, 109),
            new Color(182, 173, 228),
            new Color(177, 121, 219)
        };
        // http://www.colourlovers.com/palette/2723761/N_e_o_n_~
        static Color[] COLORS_2 = {
            new Color(255,0,102),
            new Color(255,179,0),
            new Color(176,255,5),
            new Color(0,255,200),
            new Color(112,141,145)
        };
        // http://www.colourlovers.com/palette/1778460/Aquaris
        static Color[] COLORS_3 = {
            new Color(61,10,73),
            new Color(80,21,189),
            new Color(2,127,233),
            new Color(0,202,248),
            new Color(224,218,247)
        };
        static Color[] RAINBOW = {
            Color.Red,
            Color.Orange,
            Color.Yellow,
            Color.Green,
            Color.Blue,
            Color.Indigo,
            Color.Violet
        };
        static Random r = new Random();
        Color lineColor;

        public Block(Vector2 position, Size size, bool isMoving, GraphicsDeviceManager graphics, Color[] palette) : base(graphics)
        {
            color = Color.Black;
            base.position = position;
            base.DrawSize = size;

            if (palette != null)
            {
                lineColor = palette[r.Next(palette.Length)];
            }
            else
            {
                lineColor = COLORS_1[r.Next(4)];
            }


            top = CreateLine(graphics, GameHelpers.BlockOutlineColor, GameHelpers.BlockOutlineThickness);
            right = CreateLine(graphics, GameHelpers.BlockOutlineColor, GameHelpers.BlockOutlineThickness);
            bottom = CreateLine(graphics, GameHelpers.BlockOutlineColor, GameHelpers.BlockOutlineThickness);
            left = CreateLine(graphics, GameHelpers.BlockOutlineColor, GameHelpers.BlockOutlineThickness);

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

        public override void Update(GameTimeWrapper gameTime)
        {
            MoveBlock();
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

        public void MoveBlock()
        {
            if (!isMoving)
            {
                return;
            }

            if (isMovingLeft)
            {
                base.position.X -= .3f;
                if (origin.X - base.position.X > 150)
                {
                    isMovingLeft = false;
                }
            }
            else
            {
                base.position.X += .3f;
                if (base.position.X - origin.X > 150)
                {
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
                top.Draw(spriteBatch);
                right.Draw(spriteBatch);
                bottom.Draw(spriteBatch);
                left.Draw(spriteBatch);
            }
        }
    }
}
