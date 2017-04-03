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

        public static Color GameBackgroundColor { get { return new Color(0, 0, 0); } }
        public const bool Rain = true;

        public const bool DrawBlockOutlines = true;
        public static Color BlockOutlineColor { get { return Color.White; } }
        public const int BlockOutlineThickness = 2;
        public const bool LerpBlockColor = false;

        public static Color BlockColor1 { get { return Color.Black; } }
        public static Color BlockColor2 { get { return new Color(238, 130, 238); } }
    }
}
