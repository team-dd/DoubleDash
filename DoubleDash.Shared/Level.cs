﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoubleDash
{
    public class Level
    {
        internal readonly List<BlockDescription> blocksDescription;
        public readonly List<Block> blocks;
        public Vector2 start;
        public Vector2 end;
        public Sprite endPointIndicator;

        public Level()
        {
            blocksDescription = new List<BlockDescription>();
            blocks = new List<Block>();
        }

        public void FinishLoading(Texture2D endPointTex, GraphicsDeviceManager graphics)
        {
            endPointIndicator = new Sprite(endPointTex);
            endPointIndicator.position = end;
            foreach (var blockDescription in blocksDescription)
            {
                Block block = new Block(new Vector2(blockDescription.X * 4, blockDescription.Y * 4),
                    new Size(blockDescription.Width * 4, blockDescription.Height * 4),
                    graphics);
                blocks.Add(block);
            }
        }

        public void Update(GameTimeWrapper gameTime)
        {
            foreach (var block in blocks)
            {
                block.Update(gameTime);
            }
            endPointIndicator.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var block in blocks)
            {
                block.Draw(spriteBatch);
            }

            if (end != Vector2.Zero)
            {
                endPointIndicator.Draw(spriteBatch);
            }
        }
    }
}
