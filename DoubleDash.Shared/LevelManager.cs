using System;
using System.Collections.Generic;
using System.Text;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DoubleDash
{
    public class LevelManager
    {
        public enum Worlds
        {
            World1,
            World2,
            World3
        }

        // this is so bad
        private int world1StartIndex;
        private int world2StartIndex;
        private int world3StartIndex;

        public List<Level> levels;
        public int currentLevel;
        private Texture2D endPointTex;
        private GraphicsDeviceManager graphics;
        private SoundEffect doorSound;
        public Color currentColor;
        private ShapeBackground shapeManager;
        public bool hasStartedLevel;
        public TextItem prePostMessage;
        public Sprite background;
        private VirtualResolutionRenderer vrr;

        public LevelManager(Texture2D endPointTex, GraphicsDeviceManager graphics, SoundEffect doorSound, ShapeBackground shapeManager, SpriteFont font, VirtualResolutionRenderer vrr)
        {
            this.vrr = vrr;
            levels = new List<Level>();
            this.endPointTex = endPointTex;
            this.graphics = graphics;
            this.doorSound = doorSound;
            this.shapeManager = shapeManager;
            this.hasStartedLevel = false;
            prePostMessage = new TextItem(font);
            //prePostMessage.position = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            prePostMessage.origin = Vector2.Zero;
            prePostMessage.scale = 2f;
            background = new Sprite(graphics);
            background.DrawSize = new Size(vrr.WindowResolution.Width, vrr.WindowResolution.Height);
            background.color = new Color(0, 0, 0, 128);
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
            else if (worldStartTag == Worlds.World3)
            {
                world3StartIndex = levels.Count - 1;
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
            hasStartedLevel = false;
            prePostMessage.text = "Press A to start";
            currentColor = levels[currentLevel].color;
            shapeManager.ChangeMode();
            shapeManager.UpdateColor(currentColor);
            player.spawnPoint = levels[currentLevel].start;
            player.Reset();
            player.yDeathThreshold = levels[currentLevel].highestY + 2000;
            player.resetGameTimer = gameTimer.Reset;
            player.startGameTimer = gameTimer.Start;
            gameTimer.Stop();
            gameTimer.Reset();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void MaybeZoomOut(Camera camera)
        {
            // world 3, level 2
            if (world3StartIndex + 1 == currentLevel)
            {
                camera.Zoom = .55f;
            }
            // world 3, level 3
            else if (world3StartIndex + 2 == currentLevel)
            {
                camera.Zoom = .60f;
            }
            // world 3, level 4
            else if  (world3StartIndex + 3 == currentLevel)
            {
                camera.Zoom = .60f;
            }
            else
            {
                camera.Zoom = .75f;
            }
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
            else if (world == Worlds.World3)
            {
                SetLevel(world3StartIndex, player, camera, gameTimer);
            }
        }

        public void Update(GameTimeWrapper gameTime, Player player, Camera camera, GameTimer gameTimer, GamePadState gamePadState, GamePadState previousGamePadState, bool justStartedLevel)
        {
            if (levels.Count != 0)
            {
                levels[currentLevel].Update(gameTime, camera, hasStartedLevel, justStartedLevel);
            }

            if (player.rectangle.Intersects(levels[currentLevel].endPointIndicator.rectangle))
            {
                doorSound.Play();
                IncreaseLevel(player, camera, gameTimer);
                MaybeZoomOut(camera);
            }

            if (!hasStartedLevel)
            {
                background.position = Vector2.Transform(Vector2.Zero, camera.InverseTransform);
                background.DrawSize = new Vector2(
                    vrr.WindowResolution.Width * (1 / camera.Zoom),
                    vrr.WindowResolution.Height * (1 / camera.Zoom));
                background.Update(gameTime);
            }

            if (!hasStartedLevel && gamePadState.IsButtonDownAndUp(Buttons.A, previousGamePadState) && !justStartedLevel)
            {
                hasStartedLevel = true;
                gameTimer.Start();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (levels.Count != 0)
            {
                levels[currentLevel].Draw(spriteBatch);
            }

            if (!hasStartedLevel)
            {
                background.Draw(spriteBatch);
                prePostMessage.Draw(spriteBatch);
            }
        }
    }
}
