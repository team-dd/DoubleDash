using System;
using System.Collections.Generic;
using System.Text;

namespace DoubleDash
{
    public static class Settings
    {
        public static bool BackgroundOn { get; set; }

        static Settings()
        {
            BackgroundOn = true;
        }
    }
}
