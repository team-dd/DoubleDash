using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoubleDash
{
    public static class GameHelpers
    {
        public const float Gravity = 0.0f;

        public static Color GameBackgroundColor { get { return new Color(25, 25, 30); } }
        public const bool Rain = false;

        public const bool DrawBlockOutlines = true;
        public static Color BlockOutlineColor { get { return Color.White; } }
        public const int BlockOutlineThickness = 3;
        public const bool LerpBlockColor = false;

        public static Color BlockColor1 { get { return Color.Black; } }
        public static Color BlockColor2 { get { return new Color(45, 45, 45); } }

        public static Color[] Rainbow =
        {
            // Red
            new Color(231, 76, 60),
            // Orange
            new Color(230, 126, 34),
            // Yellow
            new Color(241, 196, 15),
            // Green
            new Color(46, 204, 113),
            // Blue
            new Color(52, 152, 219),
            // Purple
            new Color(155, 89, 182),
            Color.Violet
        };
    }
}
