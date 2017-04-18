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
        const string MainGame = "game1";
        const string PauseMenu = "pauseMenu";
        const string MenuCamera = "menuCamera";

        public enum States
        {
            Game,
            MainMenu,
            WorldSelectMenu,
            PauseMenu
        }

        private States state;
        States State
        {
            get
            {
                return state;
            }
            set
            {
                if (value == States.MainMenu)
                {
                    if (state != States.WorldSelectMenu)
                    {
                        MediaPlayer.Play(song);
                        world.DeactivateGameState(MainGame);
                        world.DeactivateMenuState(PauseMenu);
                        world.ActivateMenuState(MainMenu.MainMenuName);
                        world.CurrentCameraName = MenuCamera;
                    }
                    else
                    {
                        world.DeactivateMenuState(WorldSelectMenu.WorldSelectMenuName);
                        world.ActivateMenuState(MainMenu.MainMenuName);
                    }
                }
                else if (value == States.WorldSelectMenu)
                {
                    world.DeactivateMenuState(MainMenu.MainMenuName);
                    world.ActivateMenuState(WorldSelectMenu.WorldSelectMenuName);
                }
                else if (value == States.Game)
                {
                    world.CurrentCameraName = World.Camera1Name;
                    if (state == States.MainMenu ||
                        state == States.WorldSelectMenu)
                    {
                        SwitchToGame();
                    }
                    else if (state == States.PauseMenu)
                    {
                        world.DeactivateMenuState(PauseMenu);
                        mainGameTime.GameSpeed = 1;
                        collisionGameTime.GameSpeed = 0.1m;
                        endGameTime.GameSpeed = 1;
                        timerGameTime.GameSpeed = 1;
                    }
                }
                else if (value == States.PauseMenu)
                {
                    world.CurrentCameraName = World.Camera1Name;
                    world.ActivateMenuState(PauseMenu);
                    mainGameTime.GameSpeed = 0;
                    collisionGameTime.GameSpeed = 0;
                    endGameTime.GameSpeed = 0;
                    timerGameTime.GameSpeed = 0;
                }
                state = value;
            }
        }

        GraphicsDeviceManager graphics;
        World world;

        Camera menuCamera;

        MenuState mainMenuState;
        MainMenu mainMenu;

        MenuState worldSelectMenuState;
        WorldSelectMenu worldSelectMenu;

        MenuState pauseMenuState;
        PauseMenu pauseMenu;

        GameTimeWrapper mainGameTime;
        GameTimeWrapper collisionGameTime;
        GameTimeWrapper endGameTime;
        GameTimeWrapper timerGameTime;
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

            // Load fonts
            World.FontManager.Load("Fonts/Courier_New_12");
            World.FontManager.Load("Fonts/Montserrat_Ultra_Light_36");
            World.FontManager.Load("Fonts/Arial_24");

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

            DebugText.Initialize(World.FontManager["Fonts/Courier_New_12"]);

            menuCamera = new Camera(world.virtualResolutionRenderer, Camera.CameraFocus.Center);

            SetupMainMenu();
            SetUpWorldSelectMenu();
            SetupPauseMenu();
            
            world.AddCamera(MenuCamera, mainMenu.Camera);

            mainGameTime = new GameTimeWrapper(MainUpdate, this, 1);
            collisionGameTime = new GameTimeWrapper(CollisionUpdate, this, 0.1m);
            collisionGameTime.NormalUpdate = false;
            endGameTime = new GameTimeWrapper(EndUpdate, this, 1);
            timerGameTime = new GameTimeWrapper(TimerUpdate, this, 1);
            mainGameState = new GameState(MainGame, graphics);
            mainGameState.AddTime(mainGameTime);
            mainGameState.AddTime(collisionGameTime);
            mainGameState.AddTime(endGameTime);
            mainGameState.AddTime(timerGameTime);
            mainGameState.AddDraw(MainDraw);
            world.AddGameState(mainGameState);
            world.CurrentCamera.Focus = Camera.CameraFocus.Center;
            world.CurrentCamera.Zoom = 0.75f;
            currentTime = new CurrentTime(
                World.FontManager["Fonts/Arial_24"], 
                World.SoundManager["Audio/Sounds/speedup"],
                World.SoundManager["Audio/Sounds/slowdown"],
                World.SoundManager["Audio/Sounds/speedupslower"],
                World.SoundManager["Audio/Sounds/slowdownslower"]
            );
            currentTime.AddGameTime(mainGameTime, 1);
            currentTime.AddGameTime(collisionGameTime, 0.1m);
            currentTime.AddGameTime(endGameTime, 1);

            gameTimer = new GameTimer(World.FontManager["Fonts/Arial_24"]);

            doorSound = World.SoundManager["Audio/Sounds/doorsound"];
            shapeManager = new ShapeBackground(graphics, Color.Red);
            levelManager = new LevelManager(World.TextureManager["door"], graphics, doorSound, shapeManager);

            //levelManager.AddLevel(LevelReader.Load("Content/Levels/Test Levels/collisiontest.json"));

            //World 1
<<<<<<< Updated upstream
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 1/level0.json"), LevelManager.Worlds.World1);
            levelManager.AddLevel(LevelReader.Load("Content/Levels/World 1/level1.json"));
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 1/level2.json"));
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 1/level4.json"));
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 1/level3.json"));
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 1/level5.json"));
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 1/level6.json"));

            //World 2
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 2/level1.json"), LevelManager.Worlds.World2);
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 2/level2.json"));
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 2/level3.json"));
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 2/level4.json"));
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 2/level5.json"));
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 2/level6.json"));
            levelManager.AddLevel(LevelReader.Load("Content/levels/World 2/puzzle2.json"));

            //levelManager.AddLevel(LevelReader.Load("content/levels/demo world/demo level 1.json"));
            //levelManager.AddLevel(LevelReader.Load("content/levels/test Levels/testlevevl9.json"));
            //levelManager.AddLevel(LevelReader.Load("content/levels/demo world/demo level 5.json"));
            //levelManager.AddLevel(LevelReader.Load("Content/Levels/Test Levels/zacktry1.json"));
            //levelManager.AddLevel(LevelReader.Load("content/levels/demo world/demo level 7.json"));

            levelManager.FinishLoading();



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

            starBackgroundManager = new StarBackgroundManager(graphics);
            starBackgroundManager.Create(5, world.virtualResolutionRenderer);

            rainManager = new RainManager(graphics);

            bgMusic = World.SongManager["Audio/Music/newmusic2"];
            song = World.SongManager["Audio/Music/intro2"];
            MediaPlayer.IsRepeating = true;

            State = States.MainMenu;
        }

        private void SetupMainMenu()
        {
            mainMenuState = world.AddMenuState(MainMenu.MainMenuName);
            SetMenuStateItems(mainMenuState);
            mainMenuState.AddMenuItems("Play", "World Select", "Exit");
            mainMenuState.SetMenuAction("Play", () => { State = States.Game; });
            mainMenuState.SetMenuAction("World Select", () => { State = States.WorldSelectMenu; });
            mainMenuState.SetMenuAction("Exit", () => { this.Exit(); });
            mainMenu = new MainMenu(mainMenuState, world.virtualResolutionRenderer, menuCamera, graphics);
            mainMenuState.AddTime(new GameTimeWrapper(mainMenu.Update, this, 1));
        }

        private void SetUpWorldSelectMenu()
        {
            worldSelectMenuState = world.AddMenuState(WorldSelectMenu.WorldSelectMenuName);
            SetMenuStateItems(worldSelectMenuState);
            worldSelectMenuState.AddMenuItems("World 1", "World 2", "Back");
            worldSelectMenuState.SetMenuAction("World 1", () =>
            {
                SwitchToGame(LevelManager.Worlds.World1);
            });
            worldSelectMenuState.SetMenuAction("World 2", () =>
            {
                SwitchToGame(LevelManager.Worlds.World2);
            });
            worldSelectMenuState.SetMenuAction("Back", () => { State = States.MainMenu; });
            worldSelectMenu = new WorldSelectMenu(worldSelectMenuState, world.virtualResolutionRenderer, menuCamera, graphics);
            worldSelectMenuState.AddTime(new GameTimeWrapper(worldSelectMenu.Update, this, 1));
        }

        private void SetupPauseMenu()
        {
            pauseMenuState = world.AddMenuState(PauseMenu);
            SetMenuStateItems(pauseMenuState);
            pauseMenuState.AddMenuItems("Resume", "Main Menu", "Quit");
            pauseMenuState.SetMenuAction("Resume", () => { State = States.Game; });
            pauseMenuState.SetMenuAction("Main Menu", () => { State = States.MainMenu; });
            pauseMenuState.SetMenuAction("Quit", () => { this.Exit(); });
            pauseMenu = new PauseMenu(pauseMenuState, world.virtualResolutionRenderer, world.Cameras[World.Camera1Name], graphics);
            pauseMenuState.AddTime(new GameTimeWrapper(pauseMenu.Update, this, 1));
        }

        private void SetMenuStateItems(MenuState menuState)
        {
            menuState.MenuFont = World.FontManager["Fonts/Montserrat_Ultra_Light_36"];
            menuState.UnselectedColor = Color.Gray;
            menuState.SelectedColor = Color.White;
        }

        private void SwitchToGame()
        {
            SwitchToGame(LevelManager.Worlds.World1);
        }

        private void SwitchToGame(LevelManager.Worlds startingWorld)
        {
            world.CurrentCameraName = World.Camera1Name;
            MediaPlayer.Play(bgMusic);
            world.DeactivateMenuState(MainMenu.MainMenuName);
            world.DeactivateMenuState(WorldSelectMenu.WorldSelectMenuName);
            world.ActivateGameState(MainGame);
            levelManager.SetLevel(startingWorld, player, world.Cameras[World.Camera1Name], gameTimer);
            mainGameTime.GameSpeed = 1;
            collisionGameTime.GameSpeed = 0.1m;
            endGameTime.GameSpeed = 1;
            timerGameTime.GameSpeed = 1;
            state = States.Game;
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

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            if (keyboardState.IsKeyDownAndUp(Keys.Space, previousKeyboardState))
            {
                TogglePauseMenu();
            }

            if (gamePadState.IsButtonDownAndUp(Buttons.Start, previousGamePadState))
            {
                if (State == States.Game)
                {
                    State = States.PauseMenu;
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

            world.Update(gameTime);

            if (State != States.Game)
            {
                previousKeyboardState = keyboardState;
                previousGamePadState = gamePadState;
            }

            base.Update(gameTime);
        }

        private void TogglePauseMenu()
        {
            if (State == States.PauseMenu)
            {
                State = States.Game;
            }
            else if (State == States.Game)
            {
                State = States.PauseMenu;
            }
        }

        void MainUpdate(GameTimeWrapper gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            if (keyboardState.IsKeyDownAndUp(Keys.OemTilde, previousKeyboardState))
            {
                levelManager.Start(player, world.CurrentCamera, gameTimer);
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

            shapeManager.Update(gameTime);
        }

        void CollisionUpdate(GameTimeWrapper gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

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

                player.SetDashCircle(gamePadState.ThumbSticks.Left);
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
                if (gamePadState.IsButtonDownAndUp(Buttons.B, previousGamePadState))
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

            player.Update(gameTime);
            levelManager.Update(gameTime, player, world.CurrentCamera, gameTimer);
            player.CheckCollisions(levelManager.levels[levelManager.currentLevel].blocks, currentTime);

            if (GameHelpers.Rain)
            {
                rainManager.Update(gameTime, world.virtualResolutionRenderer, world.CurrentCamera);
                rainManager.CheckCollisions(levelManager.levels[levelManager.currentLevel].blocks);
            }

            previousKeyboardState = keyboardState;
            previousGamePadState = gamePadState;
        }

        void EndUpdate(GameTimeWrapper gameTime)
        {
            if (world.CurrentCamera.Focus == Camera.CameraFocus.Center)
            {
                world.CurrentCamera.Pan = Vector2.Lerp(world.CurrentCamera.Pan, player.position, .075f);
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
            currentTime.Update(gameTime);
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
            if (State == States.MainMenu)
            {
                GraphicsDevice.Clear(GameHelpers.GameBackgroundColor);
            }
            else
            {
                GraphicsDevice.Clear(GameHelpers.GameBackgroundColor);
            }

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
            //world.Draw(starBackgroundManager.Draw);
            world.Draw(levelManager.Draw);
            world.Draw(player.Draw);
            world.Draw(currentTime.Draw);
            world.Draw(gameTimer.Draw);
            //world.Draw(DebugText.Draw);

            world.EndDraw();
        }
    }
}
