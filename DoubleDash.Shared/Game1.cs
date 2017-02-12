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
            world = new World(graphics);
            mainGameTime = new GameTimeWrapper(MainUpdate, this, 1);
            world.AddGameState(MainGame, mainGameTime, MainDraw);
            world.ActivateGameState(MainGame);
            world.CurrentCamera.Focus = Camera.CameraFocus.Center;

            level = LevelReader.Load("Content/World 1 - Level 1 V2.1.json");
            level.FinishLoading(graphics);

            player = new Player(Content.Load<Texture2D>("circle_player"));
            player.position = new Vector2(300, 800);
            testImage = new Sprite(Content.Load<Texture2D>("testimage"));
            testImage.origin = Vector2.Zero;

            walls = new Walls(graphics);

            // start floor
            walls.Create(new Vector2(10000, 100), new Vector2(0, 900));
            walls.Create(new Size(100, 1000), new Vector2(0, 0));
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            world.Update(gameTime);

            base.Update(gameTime);
        }

        void MainUpdate(GameTimeWrapper gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDownAndUp(Keys.Tab, previousKeyboardState))
            {
                if (mainGameTime.GameSpeed == 1)
                {
                    mainGameTime.GameSpeed = 2;
                }
                else
                {
                    mainGameTime.GameSpeed = 1;
                }
            }

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
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                player.MoveRight();
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

            testImage.Update(gameTime);
            player.Update(gameTime);
            walls.Update(gameTime);
            player.CheckCollisions(walls.walls);
            if (world.CurrentCamera.Focus == Camera.CameraFocus.Center)
            {
                world.CurrentCamera.Pan = player.position;
            }
            world.UpdateCurrentCamera(gameTime);
            //wall.Update(gameTime);
            //level.Update(gameTime);

            previousKeyboardState = keyboardState;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            world.DrawWorld();

            base.Draw(gameTime);
        }

        void MainDraw()
        {
            world.BeginDraw();
            world.Draw(testImage.Draw);
            world.Draw(walls.Draw);
            world.Draw(player.Draw);
            world.Draw(level.Draw);
            world.EndDraw();
        }
    }
}
