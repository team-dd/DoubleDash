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
        public enum Worlds
        {
            World1,
            World2
        }

        // this is so bad
        private int world1StartIndex;
        private int world2StartIndex;

        public List<Level> levels;
        public int currentLevel;
        private Texture2D endPointTex;
        private GraphicsDeviceManager graphics;
        private SoundEffect doorSound;
        public Color currentColor;
        private ShapeBackground shapeManager;

        public LevelManager(Texture2D endPointTex, GraphicsDeviceManager graphics, SoundEffect doorSound, ShapeBackground shapeManager)
        {
            levels = new List<Level>();
            this.endPointTex = endPointTex;
            this.graphics = graphics;
            this.doorSound = doorSound;
            this.shapeManager = shapeManager;
        }

        public void AddLevel(Level level)
        {
            levels.Add(level);
        }

        public void AddLevel(Level level, Worlds worldStartTag)
        {
            levels.Add(level);
            if (worldStartTag == Worlds.World1)
            {
                world1StartIndex = levels.Count - 1;
            }
            else if (worldStartTag == Worlds.World2)
            {
                world2StartIndex = levels.Count - 1;
            }
        }

        public void AddLevel(params Level[] levels)
        {
            AddLevel(new List<Level>(levels));
        }

        public void AddLevel(List<Level> levels)
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

        public void SetupLevel(Player player, Camera camera, GameTimer gameTimer)
        {
            currentColor = levels[currentLevel].color;
            shapeManager.ChangeMode();
            shapeManager.UpdateColor(currentColor);
            player.spawnPoint = levels[currentLevel].start;
            player.Reset();
            player.yDeathThreshold = levels[currentLevel].highestY + 2000;
            player.resetGameTimer = gameTimer.Reset;
            player.startGameTimer = gameTimer.Start;
            gameTimer.Reset();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            gameTimer.Start();
        }

        public void Start(Player player, Camera camera, GameTimer gameTimer)
        {
            currentLevel = 0;
            SetupLevel(player, camera, gameTimer);
        }

        public void IncreaseLevel(Player player, Camera camera, GameTimer gameTimer)
        {
            currentLevel++;
            if (currentLevel >= levels.Count)
            {
                currentLevel = 0;
            }
            SetupLevel(player, camera, gameTimer);
        }

        public void DecreaseLevel(Player player, Camera camera, GameTimer gameTimer)
        {
            currentLevel--;
            if (currentLevel < 0)
            {
                currentLevel = levels.Count - 1;
            }
            SetupLevel(player, camera, gameTimer);
        }

        public void SetLevel(int levelIndex, Player player, Camera camera, GameTimer gameTimer)
        {
            currentLevel = levelIndex;
            SetupLevel(player, camera, gameTimer);
        }

        public void SetLevel(Worlds world, Player player, Camera camera, GameTimer gameTimer)
        {
            if (world == Worlds.World1)
            {
                SetLevel(world1StartIndex, player, camera, gameTimer);
            }
            else if (world == Worlds.World2)
            {
                SetLevel(world2StartIndex, player, camera, gameTimer);
            }
        }

        public void Update(GameTimeWrapper gameTime, Player player, Camera camera, GameTimer gameTimer)
        {
            if (levels.Count != 0)
            {
                levels[currentLevel].Update(gameTime, camera);
            }

            if (player.rectangle.Intersects(levels[currentLevel].endPointIndicator.rectangle))
            {
                doorSound.Play();
                IncreaseLevel(player, camera, gameTimer);
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
