using System;
using System.Collections.Generic;
using System.Text;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoubleDash
{
    public class LevelManager
    {
        public List<Level> levels;
        public int currentLevel;
        private Texture2D endPointTex;
        private GraphicsDeviceManager graphics;

        public LevelManager(Texture2D endPointTex, GraphicsDeviceManager graphics)
        {
            levels = new List<Level>();
            this.endPointTex = endPointTex;
            this.graphics = graphics;
        }

        public void AddLevel(params Level[] levels)
        {
            this.levels.AddRange(levels);
        }

        public void FinishLoading()
        {
            foreach (var level in levels)
            {
                level.FinishLoading(endPointTex, graphics);
            }
        }

        public void SetupLevel(Player player, Camera camera)
        {
            player.spawnPoint = levels[currentLevel].start;
            player.Reset();
            levels[currentLevel].StartZoomIn(camera);
        }

        public void Start(Player player, Camera camera)
        {
            currentLevel = 0;
            SetupLevel(player, camera);
        }

        public void IncreaseLevel(Player player, Camera camera)
        {
            currentLevel++;
            if (currentLevel >= levels.Count)
            {
                currentLevel = 0;
            }
            SetupLevel(player, camera);
        }

        public void DecreaseLevel(Player player, Camera camera)
        {
            currentLevel--;
            if (currentLevel < 0)
            {
                currentLevel = levels.Count - 1;
            }
            SetupLevel(player, camera);
        }

        public void Update(GameTimeWrapper gameTime, Player player, Camera camera)
        {
            if (levels.Count != 0)
            {
                levels[currentLevel].Update(gameTime, camera);
            }

            if (player.rectangle.Intersects(levels[currentLevel].endPointIndicator.rectangle))
            {
                IncreaseLevel(player, camera);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (levels.Count != 0)
            {
                levels[currentLevel].Draw(spriteBatch);
            }
        }
    }
}
