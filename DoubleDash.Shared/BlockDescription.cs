using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoubleDash
{
    public class BlockDescription : TileDescription
    {
        public readonly int Width;
        public readonly int Height;

        public BlockDescription(int x, int y, int width, int height) : base(x, y)
        {
            Width = width;
            Height = height;
        }
    }
}
