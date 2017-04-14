using System;
using System.Collections.Generic;
using System.Text;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace DoubleDash
{
    public class LevelManager
    {
        public List<Level> levels;
        public int currentLevel;
        private Texture2D endPointTex;
        private GraphicsDeviceManager graphics;
        private SoundEffect doorSound;

        public LevelManager(Texture2D endPointTex, GraphicsDeviceManager graphics, SoundEffect doorSound)
        {
            levels = new List<Level>();
            this.endPointTex = endPointTex;
            this.graphics = graphics;
            this.doorSound = doorSound;
        }

        public void AddLevel(params Level[] levels)
        {
            this.levels.AddRange(levels);
        }

        public void FinishLoading()
        {
            for (int i = 0; i < levels.Count; i++)
            {
                levels[i].FinishLoading(endPointTex, graphics, i);
            }
        }

        public void SetupLevel(Player player, Camera camera)
        {
            player.spawnPoint = levels[currentLevel].start;
            player.Reset();
        }

        public void Start(Player player, Camera camera)
        {
            currentLevel = 0;
            SetupLevel(player, camera);
            player.yDeathThreshold = levels[currentLevel].highestY + 2000;
        }

        public void IncreaseLevel(Player player, Camera camera)
        {
            currentLevel++;
            if (currentLevel >= levels.Count)
            {
                currentLevel = 0;
            }
            SetupLevel(player, camera);
            player.yDeathThreshold = levels[currentLevel].highestY + 2000;
        }

        public void DecreaseLevel(Player player, Camera camera)
        {
            currentLevel--;
            if (currentLevel < 0)
            {
                currentLevel = levels.Count - 1;
            }
            SetupLevel(player, camera);
            player.yDeathThreshold = levels[currentLevel].highestY + 2000;
        }

        public void Update(GameTimeWrapper gameTime, Player player, Camera camera)
        {
            if (levels.Count != 0)
            {
                levels[currentLevel].Update(gameTime, camera);
            }

            if (player.rectangle.Intersects(levels[currentLevel].endPointIndicator.rectangle))
            {
                doorSound.Play();
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
