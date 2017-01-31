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

        Sprite testImage;
        Sprite testSquare;

        VirtualResolutionRenderer vrr;
        Camera minimapCamera;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 2000;
            graphics.PreferredBackBufferHeight = 1000;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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
            testImage = new Sprite(Content.Load<Texture2D>("test_image"));
            testImage.origin = Vector2.Zero;
            testSquare = new Sprite(Content.Load<Texture2D>("test_square"));

            vrr = new VirtualResolutionRenderer(graphics, new Size(300, 300), new Size(300, 300));
            minimapCamera = new Camera(vrr, Camera.CameraFocus.TopLeft);
            world.AddCamera("minimap", minimapCamera);
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

        public void MainUpdate(GameTimeWrapper gameTime)
        {
            world.UpdateCurrentCamera(gameTime);
            world.UpdateCamera("minimap", gameTime);
            minimapCamera.Zoom = 0.1f;

            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                testSquare.position.X -= 5;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                testSquare.position.X += 5;
            }

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                testSquare.position.Y -= 5;
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                testSquare.position.Y += 5;
            }
            testImage.Update(gameTime);
            testSquare.Update(gameTime);
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

        public void MainDraw()
        {
            world.CurrentCameraName = World.Camera1Name;
            world.BeginDraw();
            world.Draw(testImage.Draw);
            world.Draw(testSquare.Draw);
            world.EndDraw();
            world.CurrentCameraName = "minimap";
            world.BeginDraw();
            world.Draw(testImage.Draw);
            world.Draw(testSquare.Draw);
            world.EndDraw();
        }
    }
}
