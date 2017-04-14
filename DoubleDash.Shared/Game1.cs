using System.Diagnostics;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace DoubleDash
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        const string MainGame = "game1";
        const string MainMenu = "mainMenu";
        const string PauseMenu = "pauseMenu";

        public enum States
        {
            Game,
            MainMenu,
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
                    MediaPlayer.Play(song);
                    world.DeactivateGameState(MainGame);
                    world.DeactivateMenuState(PauseMenu);
                    world.ActivateMenuState(MainMenu);
                    world.CurrentCameraName = DoubleDash.MainMenu.MainMenuCamera;
                }
                else if (value == States.Game)
                {
                    world.CurrentCameraName = World.Camera1Name;
                    if (state == States.MainMenu)
                    {
                        MediaPlayer.Play(bgMusic);
                        world.DeactivateMenuState(MainMenu);
                        world.ActivateGameState(MainGame);
                        levelManager.Start(player, world.Cameras[World.Camera1Name]);
                        mainGameTime.GameSpeed = 1;
                        collisionGameTime.GameSpeed = 0.1m;
                        endGameTime.GameSpeed = 1;
                    }
                    else if (state == States.PauseMenu)
                    {
                        world.DeactivateMenuState(PauseMenu);
                        mainGameTime.GameSpeed = 1;
                        collisionGameTime.GameSpeed = 0.1m;
                        endGameTime.GameSpeed = 1;
                    }
                }
                else if (value == States.PauseMenu)
                {
                    world.CurrentCameraName = World.Camera1Name;
                    world.ActivateMenuState(PauseMenu);
                    mainGameTime.GameSpeed = 0;
                    collisionGameTime.GameSpeed = 0;
                    endGameTime.GameSpeed = 0;
                }
                state = value;
            }
        }

        GraphicsDeviceManager graphics;
        World world;

        MenuState mainMenuState;
        MainMenu mainMenu;

        MenuState pauseMenuState;
        PauseMenu pauseMenu;

        GameTimeWrapper mainGameTime;
        GameTimeWrapper collisionGameTime;
        GameTimeWrapper endGameTime;
        GameState mainGameState;

        KeyboardState previousKeyboardState;
        GamePadState previousGamePadState;

        LevelManager levelManager;

        Player player;
        CurrentTime currentTime;
        StarBackgroundManager starBackgroundManager;
        RainManager rainManager;

        Song song;
        SoundEffect doorSound;

        SpriteSheetInfo spriteSheetInfo;

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

            // Load songs
            World.SongManager.Load("Audio/Music/newmusic");
            World.SongManager.Load("Audio/Music/intro");

            DebugText.Initialize(World.FontManager["Fonts/Courier_New_12"]);

            SetupMainMenu();
            SetupPauseMenu();
            
            world.AddCamera(DoubleDash.MainMenu.MainMenuCamera, mainMenu.Camera);

            mainGameTime = new GameTimeWrapper(MainUpdate, this, 1);
            collisionGameTime = new GameTimeWrapper(CollisionUpdate, this, 0.1m);
            collisionGameTime.NormalUpdate = false;
            endGameTime = new GameTimeWrapper(EndUpdate, this, 1);
            mainGameState = new GameState(MainGame, graphics);
            mainGameState.AddTime(mainGameTime);
            mainGameState.AddTime(collisionGameTime);
            mainGameState.AddTime(endGameTime);
            mainGameState.AddDraw(MainDraw);
            world.AddGameState(mainGameState);
            world.CurrentCamera.Focus = Camera.CameraFocus.Center;
            world.CurrentCamera.Zoom = 0.75f;
            currentTime = new CurrentTime(World.FontManager["Fonts/Arial_24"]);
            currentTime.AddGameTime(mainGameTime, 1);
            currentTime.AddGameTime(collisionGameTime, 0.1m);
            currentTime.AddGameTime(endGameTime, 1);

            doorSound = World.SoundManager["Audio/Sounds/doorsound"];

            levelManager = new LevelManager(World.TextureManager["door"], graphics, doorSound);

            levelManager.AddLevel(LevelReader.Load("Content/Levels/World 1/level1.json"));
            levelManager.AddLevel(LevelReader.Load("content/levels/demo world/demo level 1.json"));
            levelManager.AddLevel(LevelReader.Load("content/levels/demo world/demo level 2.json"));
            levelManager.AddLevel(LevelReader.Load("content/levels/test Levels/testlevevl9.json"));
            levelManager.AddLevel(LevelReader.Load("content/levels/demo world/demo level 5.json"));
            levelManager.AddLevel(LevelReader.Load("Content/Levels/Test Levels/zacktry1.json"));
            levelManager.AddLevel(LevelReader.Load("content/levels/demo world/demo level 7.json"));

            levelManager.FinishLoading();

            spriteSheetInfo = new SpriteSheetInfo(30, 32);

            player = new Player(spriteSheetInfo,
                World.TextureManager["dash_indicator"],
                World.SoundManager["Audio/Sounds/jumpsound"],
                World.SoundManager["Audio/Sounds/blinksound"],
                World.SoundManager["Audio/Sounds/fail"],
                World.SoundManager["Audio/Sounds/death"],
                graphics);
            player.animations["demoanimation"] = player.animations.AddSpriteSheet(World.TextureManager["demoanimation"], spriteSheetInfo, 2, 2, 1, SpriteSheet.Direction.LeftToRight, 10, true);
            player.Ready();

            starBackgroundManager = new StarBackgroundManager(graphics);
            starBackgroundManager.Create(5, world.virtualResolutionRenderer);

            rainManager = new RainManager(graphics);

            bgMusic = World.SongManager["Audio/Music/newmusic"];
            song = World.SongManager["Audio/Music/intro"];
            MediaPlayer.IsRepeating = true;

            State = States.MainMenu;
        }

        private void SetupMainMenu()
        {
            // I can't figure out a good way of making this look better :|
            mainMenuState = world.AddMenuState(MainMenu);
            mainMenuState.UnselectedColor = Color.Gray;
            mainMenuState.SelectedColor = Color.White;
            mainMenuState.MenuFont = World.FontManager["Fonts/Montserrat_Ultra_Light_36"];
            mainMenuState.AddDraw(MainMenuDraw);
            mainMenuState.AddMenuItems("Play", "Exit");
            mainMenuState.SetMenuAction("Play", () => { State = States.Game; });
            mainMenuState.SetMenuAction("Exit", () => { this.Exit(); });
            mainMenu = new MainMenu(mainMenuState, world.virtualResolutionRenderer, graphics);
            mainMenuState.AddTime(new GameTimeWrapper(mainMenu.Update, this, 1));
        }

        private void SetupPauseMenu()
        {
            pauseMenuState = world.AddMenuState(PauseMenu);
            pauseMenuState.UnselectedColor = Color.Gray;
            pauseMenuState.SelectedColor = Color.White;
            pauseMenuState.MenuFont = World.FontManager["Fonts/Montserrat_Ultra_Light_36"];
            pauseMenuState.AddDraw(PauseMenuDraw);
            pauseMenuState.AddMenuItems("Resume", "Main Menu", "Quit");
            pauseMenuState.SetMenuAction("Resume", () => { State = States.Game; });
            pauseMenuState.SetMenuAction("Main Menu", () => { State = States.MainMenu; });
            pauseMenuState.SetMenuAction("Quit", () => { this.Exit(); });
            pauseMenu = new PauseMenu(pauseMenuState, world.virtualResolutionRenderer, world.Cameras[World.Camera1Name], graphics);
            pauseMenuState.AddTime(new GameTimeWrapper(pauseMenu.Update, this, 1));
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
                levelManager.Start(player, world.CurrentCamera);
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
            levelManager.Update(gameTime, player, world.CurrentCamera);
            player.CheckCollisions(levelManager.levels[levelManager.currentLevel].blocks);

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
                new Vector2(world.virtualResolutionRenderer.WindowResolution.Width - DashBar.BarWidth * 1.5f - 100,
                world.virtualResolutionRenderer.WindowResolution.Height - DashBar.BarHeight * 2 - 100),
                world.CurrentCamera.InverseTransform);
            player.dashBar.Update(gameTime);
            DebugText.position = Vector2.Transform(Vector2.Zero, world.CurrentCamera.InverseTransform);

            currentTime.text.position = Vector2.Transform(
                new Vector2(100),
                world.CurrentCamera.InverseTransform);
            currentTime.Update(gameTime);
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
            //world.Draw(starBackgroundManager.Draw);
            world.Draw(levelManager.Draw);
            world.Draw(player.Draw);
            world.Draw(currentTime.Draw);
            world.Draw(DebugText.Draw);
            
            world.EndDraw();
        }

        void MainMenuDraw()
        {
            world.BeginDraw();
            world.Draw(mainMenu.Draw);
            world.EndDraw();
        }

        void PauseMenuDraw()
        {
            world.BeginDraw();
            world.Draw(pauseMenu.Draw);
            world.EndDraw();
        }
    }
}
