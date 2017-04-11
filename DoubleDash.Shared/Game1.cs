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

        GameTimeWrapper mainMenuTime;
        MenuState mainMenuState;
        MainMenu mainMenu;

        GameTimeWrapper mainGameTime;
        GameTimeWrapper collisionGameTime;
        GameTimeWrapper endGameTime;
        GameState mainGameState;

        GameTimeWrapper pauseGameTime;
        MenuState pauseMenuState;

        KeyboardState previousKeyboardState;
        GamePadState previousGamePadState;

        LevelManager levelManager;

        Player player;
        CurrentTime currentTime;
        StarBackgroundManager starBackgroundManager;
        RainManager rainManager;

        Vector2 testCirclePos;
        Sprite testCircle;
        TextItem testCircleText;
        Song song;
        SoundEffect jumpSound;
        SoundEffect blinkSound;
        SoundEffect doorSound;

        SpriteSheetInfo spriteSheetInfo;

        Sprite pauseMenuRect;
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
            DebugText.Initialize(Content.Load<SpriteFont>("Fonts/Courier_New_12"));
            world = new World(graphics);

            mainMenuState = world.AddMenuState(MainMenu);
            mainMenuState.UnselectedColor = Color.Gray;
            mainMenuState.SelectedColor = Color.White;
            mainMenuState.MenuFont = Content.Load<SpriteFont>("Fonts/Montserrat_Ultra_Light_36");
            mainMenuState.AddDraw(MainMenuDraw);
            mainMenuState.AddMenuItem("Play");
            mainMenuState.AddMenuItem("Exit");
            mainMenuState.SetMenuAction("Play", () => { State = States.Game; });
            mainMenuState.SetMenuAction("Exit", () => { this.Exit(); });
            mainMenu = new MainMenu(mainMenuState, world.virtualResolutionRenderer, graphics);
            mainMenuTime = new GameTimeWrapper(mainMenu.Update, this, 1);
            mainMenuState.AddTime(mainMenuTime);
            world.AddCamera(DoubleDash.MainMenu.MainMenuCamera, mainMenu.Camera);

            pauseGameTime = new GameTimeWrapper(PauseMenuUpdate, this, 1);
            pauseMenuState = new MenuState(PauseMenu, graphics, world);
            pauseMenuState.AddTime(pauseGameTime);
            pauseMenuState.AddDraw(PauseMenuDraw);
            world.AddMenuState(pauseMenuState);

            pauseMenuRect = new Sprite(graphics);
            pauseMenuRect.DrawSize = new Size(world.virtualResolutionRenderer.WindowResolution.Width, world.virtualResolutionRenderer.WindowResolution.Height);
            pauseMenuRect.color = new Color(0, 0, 0, 128);

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
            currentTime = new CurrentTime(Content.Load<SpriteFont>("Fonts/Arial_24"));
            currentTime.AddGameTime(mainGameTime, 1);
            currentTime.AddGameTime(collisionGameTime, 0.1m);
            currentTime.AddGameTime(endGameTime, 1);

            jumpSound = Content.Load<SoundEffect>("jumpsound");
            blinkSound = Content.Load<SoundEffect>("blinksound");
            doorSound = Content.Load<SoundEffect>("doorsound");

            levelManager = new LevelManager(Content.Load<Texture2D>("door"), graphics, doorSound);
            //levelManager.AddLevel(LevelReader.Load("Content/Levels/Test Levels/testtestlevel1.json"),
            //LevelReader.Load("Content/Levels/Test Levels/triallevel.json"),
            //LevelReader.Load("Content/Levels/World 1/Level 1/level1.json"),
            //LevelReader.Load("Content/Levels/Test Levels/testlevel5.json"));
            //levelManager.AddLevel(LevelReader.Load("Content/Levels/Test Levels/longtest.json"));

            //levelManager.AddLevel(LevelReader.Load("Content/Levels/Updated World 1/1.json"));
            //levelManager.AddLevel(LevelReader.Load("Content/Levels/Updated World 1/2.json"));

            //levelManager.AddLevel(LevelReader.Load("Content/Levels/World 2/level1.json"));




            levelManager.AddLevel(LevelReader.Load("content/levels/demo world/demo level 1.json"));
            levelManager.AddLevel(LevelReader.Load("content/levels/demo world/demo level 2.json"));
            levelManager.AddLevel(LevelReader.Load("content/levels/test Levels/testlevevl9.json"));
            //levelManager.AddLevel(LevelReader.Load("content/levels/demo world/demo level 4.json"));
            //levelManager.AddLevel(LevelReader.Load("Content/Levels/Test Levels/zacktry1.json"));
            levelManager.AddLevel(LevelReader.Load("content/levels/demo world/demo level 5.json"));
            //levelManager.AddLevel(LevelReader.Load("content/levels/demo world/demo level 7.json"));








            //levelManager.AddLevel(LevelReader.Load("Content/Levels/Test Levels/zacktry1.json"));
            // levelManager.AddLevel(LevelReader.Load("Content/Levels/Test Levels/testlevel1V2.1.json"));
            //levelManager.AddLevel(LevelReader.Load("Content/Levels/Test Levels/testlevel2V1.2.json"));
            //levelManager.AddLevel(LevelReader.Load("Content/Levels/Test Levels/test8.json"));
            //levelManager.AddLevel(LevelReader.Load("Content/Levels/World 1/level1.json"));
            //levelManager.AddLevel(LevelReader.Load("Content/Levels/World 1/level2.json"));
            //levelManager.AddLevel(LevelReader.Load("Content/Levels/World 1/level5.json"));
            //levelManager.AddLevel(LevelReader.Load("Content/Levels/World 1/level3.json"));
            //levelManager.AddLevel(LevelReader.Load("Content/Levels/World 1/level4.json"));
            //levelManager.AddLevel(LevelReader.Load("Content/Levels/World 1/level4.2.json"));
            levelManager.FinishLoading();

            spriteSheetInfo = new SpriteSheetInfo(30, 32);

            player = new Player(spriteSheetInfo,
                Content.Load<Texture2D>("dash_indicator"),
                graphics);
            player.animations["demoanimation"] = player.animations.AddSpriteSheet(Content.Load<Texture2D>("demoanimation"), spriteSheetInfo, 2, 2, 1, SpriteSheet.Direction.LeftToRight, 10, true);
            player.Ready();
            testCircleText = new TextItem(DebugText.spriteFont);
            //DebugText.Add(testCircleText);

            testCircle = new Sprite(Content.Load<Texture2D>("testcircle"));
            testCirclePos = new Vector2(100);

            starBackgroundManager = new StarBackgroundManager(graphics);
            starBackgroundManager.Create(5, world.virtualResolutionRenderer);

            rainManager = new RainManager(graphics);

            bgMusic = Content.Load<Song>("newmusic");
            song = Content.Load<Song>("intro");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(song);

            

            levelManager.Start(player, world.CurrentCamera);

            State = States.MainMenu;
            //State = States.Game;
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
                TogglePauseMenu();
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

            if (keyboardState.IsKeyDownAndUp(Keys.OemTilde, previousKeyboardState) ||
                gamePadState.IsButtonDownAndUp(Buttons.Start, previousGamePadState))
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

            if (keyboardState.IsKeyDown(Keys.W))
            {
                testCirclePos.Y -= 5;
            }
            else if (keyboardState.IsKeyDown(Keys.S))
            {
                testCirclePos.Y += 5;
            }

            if (gamePadState.IsConnected)
            {
                if (gamePadState.IsButtonDownAndUp(Buttons.B, previousGamePadState))
                {
                    blinkSound.Play();
                    player.Dash();
                }
            }
            else
            {
                if (keyboardState.IsKeyDownAndUp(Keys.X, previousKeyboardState))
                {
                    blinkSound.Play();
                    player.Dash();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
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
                if (gamePadState.IsButtonDown(Buttons.A))
                {
                    if (previousGamePadState.IsButtonUp(Buttons.A))
                    {
                        jumpSound.Play(0.3f, 0.0f, 0.0f);
                    }
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
                    if (keyboardState.IsKeyDownAndUp(Keys.Z, previousKeyboardState))
                    {
                        jumpSound.Play(0.3f, 0.0f, 0.0f);
                    }
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
        }

        void EndUpdate(GameTimeWrapper gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

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
            testCircle.position = Vector2.Transform(testCirclePos, world.CurrentCamera.InverseTransform);
            testCircle.Update(gameTime);

            testCircleText.text = $"Test circle visible by camera: {world.CurrentCamera.Contains(testCircle.rectangle)}";

            previousKeyboardState = keyboardState;
            previousGamePadState = gamePadState;
        }

        void PauseMenuUpdate(GameTimeWrapper gameTime)
        {
            pauseMenuRect.position = Vector2.Transform(Vector2.Zero, world.CurrentCamera.InverseTransform);
            pauseMenuRect.Update(gameTime);
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
            //world.Draw(testCircle.Draw);
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
            world.Draw(pauseMenuRect.Draw);
            world.EndDraw();
        }
    }
}
