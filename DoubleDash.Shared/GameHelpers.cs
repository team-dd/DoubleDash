using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoubleDash
{
    public static class GameHelpers
    {
        public const float Gravity = 0.7f;

        public static Color GameBackgroundColor { get { return new Color(25, 25, 30); } }
        public const bool Rain = false;

        public const bool DrawBlockOutlines = true;
        public static Color BlockOutlineColor { get { return Color.White; } }
        public const int BlockOutlineThickness = 3;
        public const bool LerpBlockColor = false;

        public static Color BlockColor1 { get { return Color.Black; } }
        public static Color BlockColor2 { get { return new Color(45, 45, 45); } }
    }
}
