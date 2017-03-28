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
        private bool zoomingIn;

        public Level()
        {
            blocksDescription = new List<BlockDescription>();
            blocks = new List<Block>();
            zoomingIn = false;
        }

        public void FinishLoading(Texture2D endPointTex, GraphicsDeviceManager graphics)
        {
            endPointIndicator = new Sprite(endPointTex);
            endPointIndicator.position = end;
            foreach (var blockDescription in blocksDescription)
            {
                Block block = new Block(new Vector2(blockDescription.X * 4, blockDescription.Y * 4),
                    new Size(blockDescription.Width * 4, blockDescription.Height * 4),
                    blockDescription.IsMoving,
                    graphics);
                blocks.Add(block);
            }
        }

        public void StartZoomIn(Camera camera)
        {
            zoomingIn = true;
            camera.Zoom = 0.01f;
        }

        public void Update(GameTimeWrapper gameTime, Camera camera)
        {
            if (zoomingIn)
            {
                if (camera.Zoom < 1)
                {
                    camera.Zoom += 0.001f * (float)gameTime.GameSpeed;
                }
                else
                {
                    camera.Zoom = 1;
                    zoomingIn = false;
                }
            }
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
