using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoubleDash
{
    public class Wall : Sprite
    {
        private GraphicsDeviceManager graphics;
        private Line top;
        private Line right;
        private Line bottom;
        private Line left;

        private Line topNormal;
        private Line rightNormal;
        private Line bottomNormal;
        private Line leftNormal;

        public Wall(GraphicsDeviceManager graphics) : base(graphics)
        {
            color = Color.Black;
            top = new Line(graphics);
            right = new Line(graphics);
            bottom = new Line(graphics);
            left = new Line(graphics);

            topNormal = new Line(graphics);
            rightNormal = new Line(graphics);
            bottomNormal = new Line(graphics);
            leftNormal = new Line(graphics);
        }

        public override void Update(GameTimeWrapper gameTime)
        {
            top.point1 = new Vector2(position.X, position.Y);
            top.point2 = new Vector2(top.point1.X + DrawSize.Width, top.point1.Y);
            right.point1 = top.point2;
            right.point2 = new Vector2(right.point1.X, right.point1.Y + DrawSize.Height);
            bottom.point1 = right.point2;
            bottom.point2 = new Vector2(top.point1.X, bottom.point1.Y);
            left.point1 = bottom.point2;
            left.point2 = top.point1;

            topNormal.point1 = new Vector2(top.point1.X + ((top.point2.X - top.point1.X) / 2),
                top.point1.Y + ((top.point2.Y - top.point1.Y) / 2));
            Vector2 topNorm = (top.point2 - top.point1).LeftNormal();
            topNorm = Vector2.Normalize(topNorm) * 50;
            topNormal.point2 = topNormal.point1 - topNorm;
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            top.Draw(spriteBatch);
            right.Draw(spriteBatch);
            bottom.Draw(spriteBatch);
            left.Draw(spriteBatch);
            topNormal.Draw(spriteBatch);
        }
    }
}
