using System.Diagnostics;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DoubleDash
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        const string MainGame = "game1";
        GraphicsDeviceManager graphics;
        World world;
        GameTimeWrapper mainGameTime;
        KeyboardState previousKeyboardState;

        Level level;
        Walls walls;

        Player player;
        Sprite testImage;
        CurrentTime currentTime;
        StarBackgroundManager starBackgroundManager;

        Vector2 testCirclePos;
        Sprite testCircle;
        TextItem testCircleText;

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
            graphics.ApplyChanges();

            previousKeyboardState = Keyboard.GetState();

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
            world.AddGameState(MainGame, mainGameTime, MainDraw);
            world.ActivateGameState(MainGame);
            world.CurrentCamera.Focus = Camera.CameraFocus.Center;
            currentTime = new CurrentTime(mainGameTime, Content.Load<SpriteFont>("Fonts/Arial_24"));

            level = LevelReader.Load("Content/emptyspace.json");
            level.FinishLoading(graphics);

            player = new Player(Content.Load<Texture2D>("circle_player"),
                Content.Load<Texture2D>("dash_indicator"),
                graphics);
            player.position = level.start;
            testImage = new Sprite(Content.Load<Texture2D>("testimage"));
            testImage.origin = Vector2.Zero;
            testCircleText = new TextItem(DebugText.spriteFont);
            DebugText.Add(testCircleText);

            testCircle = new Sprite(Content.Load<Texture2D>("testcircle"));
            testCirclePos = new Vector2(100);

            // start floor
            //walls.Create(new Vector2(10000, 100), new Vector2(0, 900));
            //walls.Create(new Size(100, 1000), new Vector2(0, 0));

            starBackgroundManager = new StarBackgroundManager(graphics);
            starBackgroundManager.Create(5, world.virtualResolutionRenderer);
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

            world.Update(gameTime);

            base.Update(gameTime);
        }

        void MainUpdate(GameTimeWrapper gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDownAndUp(Keys.LeftControl, previousKeyboardState))
            {
                if (world.CurrentCamera.Focus == Camera.CameraFocus.Center)
                {
                    world.CurrentCamera.Focus = Camera.CameraFocus.TopLeft;
                    world.CurrentCamera.Pan = Vector2.Zero;
                }
                else
                {
                    world.CurrentCamera.Focus = Camera.CameraFocus.Center;
                    world.CurrentCamera.Pan = player.position;
                }
            }


            if (keyboardState.IsKeyDown(Keys.Left))
            {
                player.MoveLeft();
                starBackgroundManager.MoveLeft(5);
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                player.MoveRight();
                starBackgroundManager.MoveRight(5);
            }
            else
            {
                player.ResetXAcceleration();
            }
            
            if (keyboardState.IsKeyDown(Keys.Z))
            {
                player.Jump();
            }
            else
            {
                player.CancelJump();
            }

            if (keyboardState.IsKeyDownAndUp(Keys.X, previousKeyboardState))
            {
                player.Dash();
            }

            if (keyboardState.IsKeyDown(Keys.W))
            {
                testCirclePos.Y -= 5;
            }
            else if (keyboardState.IsKeyDown(Keys.S))
            {
                testCirclePos.Y += 5;
            }

            testImage.Update(gameTime);
            player.Update(gameTime);
            level.Update(gameTime);
            player.CheckCollisions(level.blocks);
            if (world.CurrentCamera.Focus == Camera.CameraFocus.Center)
            {
                world.CurrentCamera.Pan = player.position;
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
            //level.Update(gameTime);

            previousKeyboardState = keyboardState;
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
            //world.Draw(testImage.Draw);
            world.Draw(starBackgroundManager.Draw);
            //world.Draw(walls.Draw);
            world.Draw(player.Draw);
            world.Draw(testCircle.Draw);
            world.Draw(currentTime.Draw);
            world.Draw(level.Draw);
            world.Draw(DebugText.Draw);
            world.EndDraw();
        }
    }
}
