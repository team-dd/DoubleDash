using System;
using System.Diagnostics;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using DoubleDash.Menus;

namespace DoubleDash
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public const string MainGame = "game1";

        GraphicsDeviceManager graphics;
        World world;

        StateManager stateManager;
        TimeManager timeManager;

        GameTimeWrapper mainGameTime;
        GameTimeManager mainGameTimeManager;

        GameTimeWrapper collisionGameTime;
        GameTimeManager collisionGameTimeManager;

        GameTimeWrapper endGameTime;
        GameTimeManager endGameTimeManager;

        GameTimeWrapper timerGameTime;
        GameTimeManager timerGameTimeManager;

        GameState mainGameState;

        GameTimer gameTimer;

        KeyboardState previousKeyboardState;
        GamePadState previousGamePadState;

        LevelManager levelManager;

        Player player;
        CurrentTime currentTime;
        StarBackgroundManager starBackgroundManager;
        RainManager rainManager;
        ShapeBackground shapeManager;

        Song song;
        SoundEffect doorSound;

        Song bgMusic;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

#if WINDOWS
            Window.IsBorderless = true;
#else
            graphics.IsFullScreen = true;
#endif

#if !WINDOWS_UWP
            Window.Position = Point.Zero;
#endif
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            graphics.ApplyChanges();

            previousKeyboardState = Keyboard.GetState();
            previousGamePadState = GamePad.GetState(PlayerIndex.One);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            world = new World(graphics, Content);

            // Load textures
            World.TextureManager.Load("door");
            World.TextureManager.Load("dash_indicator");
            World.TextureManager.Load("demoanimation");
            World.TextureManager.Load("blink_animation");
            World.TextureManager.Load("title_screen");
            World.TextureManager.Load("xbox_scheme");

            // Load fonts
            World.FontManager.Load("Fonts/Courier_New_12");
            World.FontManager.Load("Fonts/Montserrat_Ultra_Light_36");
            World.FontManager.Load("Fonts/8bit");

            // Load sounds
            World.SoundManager.Load("Audio/Sounds/jumpsound");
            World.SoundManager.Load("Audio/Sounds/blinksound");
            World.SoundManager.Load("Audio/Sounds/doorsound");
            World.SoundManager.Load("Audio/Sounds/death");
            World.SoundManager.Load("Audio/Sounds/fail");
            World.SoundManager.Load("Audio/Sounds/speedup");
            World.SoundManager.Load("Audio/Sounds/slowdown");
            World.SoundManager.Load("Audio/Sounds/speedupslower");
            World.SoundManager.Load("Audio/Sounds/slowdownslower");

            // Load songs
            World.SongManager.Load("Audio/Music/newmusic2");
            World.SongManager.Load("Audio/Music/intro2");
            World.SongManager.Load("Audio/Music/title");

            DebugText.Initialize(World.FontManager["Fonts/Courier_New_12"]);

            player = new Player(World.TextureManager["dash_indicator"],
                World.SoundManager["Audio/Sounds/jumpsound"],
                World.SoundManager["Audio/Sounds/blinksound"],
                World.SoundManager["Audio/Sounds/fail"],
                World.SoundManager["Audio/Sounds/death"],
                graphics);
            player.animations["demoanimation"] = player.animations.AddSpriteSheet(World.TextureManager["demoanimation"], player.spriteSheetInfo, 2, 2, 1, SpriteSheet.Direction.LeftToRight, 10, true);
            player.animations["dashAnimation"] = player.animations.AddSpriteSheet(World.TextureManager["blink_animation"], player.spriteSheetInfo, 3, 3, 1, SpriteSheet.Direction.LeftToRight, 10, false);
            player.animations.CurrentAnimationName = "demoanimation";
            player.Ready();

            mainGameTime = new GameTimeWrapper(MainUpdate, this, 1);
            mainGameTimeManager = new GameTimeManager(mainGameTime, 1);
            collisionGameTime = new GameTimeWrapper(CollisionUpdate, this, 0.1m);
            collisionGameTime.NormalUpdate = false;
            collisionGameTimeManager = new GameTimeManager(collisionGameTime, 0.1m);
            endGameTime = new GameTimeWrapper(EndUpdate, this, 1);
            endGameTimeManager = new GameTimeManager(endGameTime, 1);
            timerGameTime = new GameTimeWrapper(TimerUpdate, this, 1);
            timerGameTimeManager = new GameTimeManager(timerGameTime, 1);

            timeManager = new TimeManager();
            timeManager.AddTimes(mainGameTimeManager, collisionGameTimeManager, endGameTimeManager, timerGameTimeManager);

            doorSound = World.SoundManager["Audio/Sounds/doorsound"];
            shapeManager = new ShapeBackground(graphics, Color.Red);
            levelManager = new LevelManager(World.TextureManager["door"], graphics, doorSound, shapeManager, World.FontManager["Fonts/8bit"], world.virtualResolutionRenderer);

            gameTimer = new GameTimer(World.FontManager["Fonts/8bit"]);

            bgMusic = World.SongManager["Audio/Music/newmusic2"];
            song = World.SongManager["Audio/Music/intro2"];
            MediaPlayer.IsRepeating = true;

            currentTime = new CurrentTime(
                World.FontManager["Fonts/8bit"],
                World.SoundManager["Audio/Sounds/speedup"],
                World.SoundManager["Audio/Sounds/slowdown"],
                World.SoundManager["Audio/Sounds/speedupslower"],
                World.SoundManager["Audio/Sounds/slowdownslower"]
            );
            currentTime.AddGameTime(mainGameTimeManager);
            currentTime.AddGameTime(collisionGameTimeManager);
            currentTime.AddGameTime(endGameTimeManager);

            stateManager = new StateManager(this, world, graphics, timeManager, levelManager, gameTimer, currentTime, player, song, bgMusic, World.SongManager["Audio/Music/title"]);
            MediaPlayer.Play(World.SongManager["Audio/Music/title"]);

            mainGameState = new GameState(MainGame, graphics);
            mainGameState.AddTime(mainGameTime);
            mainGameState.AddTime(collisionGameTime);
            mainGameState.AddTime(endGameTime);
            mainGameState.AddTime(timerGameTime);
            mainGameState.AddDraw(MainDraw);
            world.AddGameState(mainGameState);
            world.Cameras[World.Camera1Name].Focus = Camera.CameraFocus.Center;
            world.Cameras[World.Camera1Name].Zoom = 0.75f;

            //levelManager.AddLevel(LevelReader.Load("Content/Levels/Test Levels/collisiontest.json"));

            //World 1
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 1/level1.json"), LevelManager.Worlds.World1);
            levelManager.AddLevel(LevelReader.Load("Content/Levels/World 1/level2.json"));
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 1/level3.json"));
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 1/level4.json"));
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 1/level5.json"));
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 1/level6.json"));
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 1/level7.json"));

            //World 2
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 2/level1.json"), LevelManager.Worlds.World2);
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 2/level2.json"));
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 2/level3.json"));
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 2/level4.json"));
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 2/level5.json"));
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 2/level6.json"));
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 2/puzzle2.json"));

            //World 3
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 3/level1.json"), LevelManager.Worlds.World3);
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 3/level2.json"));
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 3/level3.json"));
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 3/level4.json"));
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 3/level5.json"));
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 3/level6.json"));
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 3/level7.json"));




            //levelManager.AddLevel(LevelReader.Load("content/levels/demo world/demo level 1.json"));
            //levelManager.AddLevel(LevelReader.Load("content/levels/test Levels/testlevevl9.json"));
            //levelManager.AddLevel(LevelReader.Load("content/levels/demo world/demo level 5.json"));
            //levelManager.AddLevel(LevelReader.Load("Content/Levels/Test Levels/zacktry1.json"));
            //levelManager.AddLevel(LevelReader.Load("content/levels/demo world/demo level 7.json"));

            levelManager.FinishLoading();

            starBackgroundManager = new StarBackgroundManager(graphics);
            starBackgroundManager.Create(5, world.virtualResolutionRenderer);

            if (GameHelpers.Rain)
            {
                rainManager = new RainManager(graphics);
            }

            stateManager.State = StateManager.States.TitleScreen;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            if (keyboardState.GetPressedKeys().Length > 0
                || anyButtonDown(gamePadState))
            {
                if (stateManager.State == StateManager.States.TitleScreen)
                {
                    stateManager.State = StateManager.States.MainMenu;
                }
            }

            if (keyboardState.IsKeyDownAndUp(Keys.Escape, previousKeyboardState) ||
                gamePadState.IsButtonDownAndUp(Buttons.Start, previousGamePadState))
            {
                if (stateManager.State == StateManager.States.Game)
                {
                    stateManager.State = StateManager.States.PauseMenu;
                }
            }

            if (keyboardState.IsKeyDown(Keys.OemPlus))
            {
                world.CurrentCamera.Zoom += 0.01f;
            }
            else if (keyboardState.IsKeyDown(Keys.OemMinus))
            {
                world.CurrentCamera.Zoom -= 0.01f;
            }

            if (keyboardState.IsKeyDownAndUp(Keys.D1, previousKeyboardState))
            {
                currentTime.SetToSlow();
            }
            else if (keyboardState.IsKeyDownAndUp(Keys.D2, previousKeyboardState))
            {
                currentTime.SetToNormal();
            }
            else if (keyboardState.IsKeyDownAndUp(Keys.D3, previousKeyboardState))
            {
                currentTime.SetToFast();
            }

            if (gamePadState.IsButtonDownAndUp(Buttons.LeftShoulder, previousGamePadState))
            {
                currentTime.SetSlower();
            }
            else if (gamePadState.IsButtonDownAndUp(Buttons.RightShoulder, previousGamePadState))
            {
                currentTime.SetFaster();
            }
            
            if (keyboardState.IsKeyDownAndUp(Keys.O, previousKeyboardState))
            {
                shapeManager.ChangeMode();
            }

            stateManager.Update();

            world.Update(gameTime);

            if (stateManager.State != StateManager.States.Game)
            {
                previousKeyboardState = keyboardState;
                previousGamePadState = gamePadState;
            }

            base.Update(gameTime);
        }

        void MainUpdate(GameTimeWrapper gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            if (keyboardState.IsKeyDownAndUp(Keys.OemTilde, previousKeyboardState))
            {
                levelManager.Start(player, world.CurrentCamera, gameTimer, currentTime);
            }

            if (gamePadState.IsConnected)
            {
                if (gamePadState.ThumbSticks.Left.X < 0)
                {
                    starBackgroundManager.MoveRight(10);
                }
                else if (gamePadState.ThumbSticks.Left.X > 0)
                {
                    starBackgroundManager.MoveLeft(10);
                }
                else
                {
                    starBackgroundManager.MoveLeft(2);
                }
            }
            else
            {
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    starBackgroundManager.MoveRight(10);
                }
                else if (keyboardState.IsKeyDown(Keys.Right))
                {
                    starBackgroundManager.MoveLeft(10);
                }
                else
                {
                    starBackgroundManager.MoveLeft(2);
                }
            }
            if (levelManager.hasStartedLevel)
            {
                shapeManager.Update();
            }
        }

        void CollisionUpdate(GameTimeWrapper gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            if (levelManager.hasStartedLevel)
            {
                if (gamePadState.IsConnected)
                {
                    if (gamePadState.ThumbSticks.Left.X < 0)
                    {
                        player.MoveLeft(gamePadState.ThumbSticks.Left.X * -1, gameTime);
                    }
                    else if (gamePadState.ThumbSticks.Left.X > 0)
                    {
                        player.MoveRight(gamePadState.ThumbSticks.Left.X, gameTime);
                    }
                    else
                    {
                        player.LetGo();
                    }

                    if (gamePadState.ThumbSticks.Right.X > -0.05f && gamePadState.ThumbSticks.Right.X < 0.05f &&
                        gamePadState.ThumbSticks.Right.Y > -0.05f && gamePadState.ThumbSticks.Right.Y < 0.05f)
                    {
                        player.SetDashCircle(gamePadState.ThumbSticks.Left);
                    }
                    else
                    {
                        player.SetDashCircle(gamePadState.ThumbSticks.Right);
                    }
                }
                else
                {
                    if (keyboardState.IsKeyDown(Keys.Left))
                    {
                        player.MoveLeft(gameTime);
                    }
                    else if (keyboardState.IsKeyDown(Keys.Right))
                    {
                        player.MoveRight(gameTime);
                    }
                    else
                    {
                        player.LetGo();
                    }
                }

                if (gamePadState.IsConnected)
                {
                    if (gamePadState.Triggers.Right >= 0.5f &&
                        previousGamePadState.Triggers.Right < 0.5f)
                    {
                        player.Dash();
                    }
                }
                else
                {
                    if (keyboardState.IsKeyDownAndUp(Keys.X, previousKeyboardState))
                    {
                        player.Dash();
                    }
                }

                if (gamePadState.IsConnected)
                {
                    if (gamePadState.IsButtonDown(Buttons.A))
                    {
                        player.Jump();
                    }
                    else
                    {
                        player.CancelJump();
                    }
                }
                else
                {
                    if (keyboardState.IsKeyDown(Keys.Z))
                    {
                        player.Jump();
                    }
                    else
                    {
                        player.CancelJump();
                    }
                }
            }

            player.Update(gameTime, levelManager.hasStartedLevel);

            levelManager.Update(gameTime, player, world.CurrentCamera, gameTimer, gamePadState, previousGamePadState, stateManager.justStartedGame, currentTime);

            if (stateManager.justStartedGame)
            {
                stateManager.justStartedGame = false;
            }

            player.CheckCollisions(levelManager.levels[levelManager.currentLevel].blocks, currentTime);

            if (GameHelpers.Rain)
            {
                rainManager.Update(gameTime, world.virtualResolutionRenderer, world.CurrentCamera);
                rainManager.CheckCollisions(levelManager.levels[levelManager.currentLevel].blocks);
            }

            previousKeyboardState = keyboardState;
            previousGamePadState = gamePadState;
        }

        bool anyButtonDown(GamePadState gamePadState)
        {
            return gamePadState.Buttons.A == ButtonState.Pressed
                || gamePadState.Buttons.B == ButtonState.Pressed
                || gamePadState.Buttons.X == ButtonState.Pressed
                || gamePadState.Buttons.Y == ButtonState.Pressed
                || gamePadState.Buttons.Start == ButtonState.Pressed
                || gamePadState.Buttons.Back == ButtonState.Pressed
                || gamePadState.Buttons.BigButton == ButtonState.Pressed;
        }

        void EndUpdate(GameTimeWrapper gameTime)
        {
            if (world.CurrentCamera.Focus == Camera.CameraFocus.Center)
            {
                if (!levelManager.hasStartedLevel)
                {
                    world.CurrentCamera.Pan = player.position;
                }
                else
                {
                    world.CurrentCamera.Pan = Vector2.Lerp(world.CurrentCamera.Pan, player.position, .075f);
                }
            }
            world.UpdateCurrentCamera(gameTime);

            starBackgroundManager.Update(gameTime, world.CurrentCamera);

            // GUI stuff
            player.dashBar.Position = Vector2.Transform(
                new Vector2(world.virtualResolutionRenderer.WindowResolution.Width - DashBar.BarWidth - 100,
                world.virtualResolutionRenderer.WindowResolution.Height - DashBar.BarHeight - 100),
                world.CurrentCamera.InverseTransform);
            player.dashBar.Update(gameTime, world.CurrentCamera);
            DebugText.position = Vector2.Transform(Vector2.Zero, world.CurrentCamera.InverseTransform);

            currentTime.text.position = Vector2.Transform(
                new Vector2(100),
                world.CurrentCamera.InverseTransform);

            if (!levelManager.hasStartedLevel)
            {
                levelManager.levelMessage.position = Vector2.Transform(
                new Vector2(world.virtualResolutionRenderer.WindowResolution.Width / 2 - levelManager.prePostMessage.textSize.X / 2 + 30,
                world.virtualResolutionRenderer.WindowResolution.Height / 2 - levelManager.prePostMessage.textSize.Y / 2 - 200),
                world.CurrentCamera.InverseTransform);
                levelManager.prePostMessage.position = Vector2.Transform(
                new Vector2(world.virtualResolutionRenderer.WindowResolution.Width / 2 - levelManager.prePostMessage.textSize.X / 2 - 50,
                world.virtualResolutionRenderer.WindowResolution.Height / 2 - levelManager.prePostMessage.textSize.Y / 2 - 100),
                world.CurrentCamera.InverseTransform);
            }
        }

        void TimerUpdate(GameTimeWrapper gameTime)
        {
            gameTimer.timerText.position = Vector2.Transform(
                new Vector2(world.virtualResolutionRenderer.WindowResolution.Width/2 - gameTimer.timerText.textSize.X/2 - 35,
                100),
                world.CurrentCamera.InverseTransform);
            gameTimer.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(GameHelpers.GameBackgroundColor);

            world.DrawWorld();

            base.Draw(gameTime);
        }

        void MainDraw()
        {
            world.BeginDraw();
            if (GameHelpers.Rain)
            {
                world.Draw(rainManager.Draw);
            }
            shapeManager.Draw();
            world.Draw(levelManager.Draw);
            world.Draw((spriteBatch) => { player.Draw(spriteBatch, levelManager.hasStartedLevel); });
            if (levelManager.hasStartedLevel)
            {
                world.Draw(currentTime.Draw);
                world.Draw(gameTimer.Draw);
            }
            
            //world.Draw(DebugText.Draw);

            world.EndDraw();
        }
    }
}
