using System.Diagnostics;
using GLX;
using Microsoft.Xna.Framework;
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
        const string CollisionGameState = "collision";
        const string EndGameTimeState = "end";

        GraphicsDeviceManager graphics;
        World world;
        GameTimeWrapper mainGameTime;
        GameTimeWrapper collisionGameTime;
        GameTimeWrapper endGameTime;
        KeyboardState previousKeyboardState;
        GamePadState previousGamePadState;

        LevelManager levelManager;
        Walls walls;

        Player player;
        CurrentTime currentTime;
        StarBackgroundManager starBackgroundManager;

        Vector2 testCirclePos;
        Sprite testCircle;
        TextItem testCircleText;
        Song song;

        SpriteSheet spriteSheet;
        SpriteSheetInfo spriteSheetInfo;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

#if WINDOWS
            Window.IsBorderless = true;
#else
            graphics.IsFullScreen = true;
#endif
            Window.Position = Point.Zero;
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
            //graphics.PreferredBackBufferHeight = 1688;
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
            mainGameTime = new GameTimeWrapper(MainUpdate, this, 1);
            collisionGameTime = new GameTimeWrapper(CollisionUpdate, this, 0.1m);
            collisionGameTime.NormalUpdate = false;
            endGameTime = new GameTimeWrapper(EndUpdate, this, 1);
            world.AddGameState(MainGame, mainGameTime, MainDraw);
            world.AddGameState(CollisionGameState, collisionGameTime);
            world.AddGameState(EndGameTimeState, endGameTime);
            world.ActivateGameState(MainGame);
            world.ActivateGameState(CollisionGameState);
            world.ActivateGameState(EndGameTimeState);
            world.CurrentCamera.Focus = Camera.CameraFocus.Center;
            world.CurrentCamera.Zoom = 0.75f;
            world.CurrentCamera.Origin *= 1 / .75f;
            currentTime = new CurrentTime(mainGameTime, Content.Load<SpriteFont>("Fonts/Arial_24"));

            levelManager = new LevelManager(Content.Load<Texture2D>("end_point_indicator"), graphics);
            /*levelManager.AddLevel(LevelReader.Load("Content/Levels/Test Levels/testlevel1.json"),
                //LevelReader.Load("Content/Levels/Test Levels/triallevel.json"),
                LevelReader.Load("Content/Levels/World 1/Level 1/level1.json"),
                LevelReader.Load("Content/Levels/Test Levels/testlevel5.json"));*/
            //levelManager.AddLevel(LevelReader.Load("Content/Levels/Test Levels/longtest.json"));


            //levelManager.AddLevel(LevelReader.Load("Content/Levels/Test Levels/testlevel1V2.1.json"));
            levelManager.AddLevel(LevelReader.Load("Content/Levels/Test Levels/testlevel2V1.2.json"));
            levelManager.FinishLoading();

            spriteSheetInfo = new SpriteSheetInfo(30, 32);

            player = new Player(spriteSheetInfo,
                Content.Load<Texture2D>("dash_indicator"),
                graphics);
            player.animations["demoanimation"] = player.animations.AddSpriteSheet(Content.Load<Texture2D>("demoanimation"), spriteSheetInfo, 2, 2, 1, SpriteSheet.Direction.LeftToRight, 2, true);
            player.Ready();
            testCircleText = new TextItem(DebugText.spriteFont);
            //DebugText.Add(testCircleText);

            testCircle = new Sprite(Content.Load<Texture2D>("testcircle"));
            testCirclePos = new Vector2(100);

            // start floor
            //walls.Create(new Vector2(10000, 100), new Vector2(0, 900));
            //walls.Create(new Size(100, 1000), new Vector2(0, 0));

            starBackgroundManager = new StarBackgroundManager(graphics);
            starBackgroundManager.Create(5, world.virtualResolutionRenderer);

            song = Content.Load<Song>("music");
            MediaPlayer.IsRepeating = true;
            //MediaPlayer.Play(song);

            levelManager.Start(player);
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

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

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

            base.Update(gameTime);
        }

        void MainUpdate(GameTimeWrapper gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            if (keyboardState.IsKeyDownAndUp(Keys.OemTilde, previousKeyboardState) ||
                gamePadState.IsButtonDownAndUp(Buttons.Start, previousGamePadState))
            {
                levelManager.Start(player);
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
                    player.ResetXAcceleration();
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
                    player.ResetXAcceleration();
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

            if (keyboardState.IsKeyDownAndUp(Keys.X, previousKeyboardState))
            {
                //player.Dash();
            }

            player.Update(gameTime);
            levelManager.Update(gameTime, player);
            player.CheckCollisions(levelManager.levels[levelManager.currentLevel].blocks);
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
                new Vector2(world.virtualResolutionRenderer.VirtualResolution.Width - DashBar.BarWidth - 50,
                world.virtualResolutionRenderer.VirtualResolution.Height - DashBar.BarHeight - 100),
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

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(216, 216, 216));

            world.DrawWorld();

            base.Draw(gameTime);
        }

        void MainDraw()
        {
            world.BeginDraw();
            world.Draw(starBackgroundManager.Draw);
            //world.Draw(walls.Draw);
            world.Draw(player.Draw);
            world.Draw(levelManager.Draw);
            //world.Draw(testCircle.Draw);
            world.Draw(currentTime.Draw);
            world.Draw(DebugText.Draw);
            world.EndDraw();
        }
    }
}
