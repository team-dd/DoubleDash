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

        //Player player;
        Walls walls;
        //Wall wall;
        Block block1;
        Block block2;
        Line line;

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

            //player = new Player(Content.Load<Texture2D>("player"));
            //player.position = new Vector2(300, 400);

            line = new Line(graphics);
            block1 = new Block(new Vector2(100), new Size(100), graphics);
            block2 = new Block(new Vector2(300), new Size(100), graphics);

            //wall = new Wall(graphics);
            //wall.DrawSize = new Size(100, 100);
            //wall.position = new Vector2(500);

            //walls = new Walls(graphics);

            //// start floor
            //walls.Create(new Vector2(600, 100), new Vector2(0, 900));
            //// jump to other section floor
            //walls.Create(new Vector2(1000, 100), new Vector2(900, 900));
            //// right wall
            //walls.Create(new Vector2(500, 1000), new Vector2(1800, 0));
            //// end block
            //walls.Create(new Vector2(100, 300), new Vector2(1700, 600));
            //// mid block
            //walls.Create(new Vector2(100, 300), new Vector2(1600, 300));
            //// top block
            //walls.Create(new Vector2(100, 300), new Vector2(1700, 0));
            //// top right floor
            //walls.Create(new Vector2(1000, 25), new Vector2(900, 275));
            //// top left floor
            //walls.Create(new Vector2(600, 25), new Vector2(0, 275));
            //// shaft left wall
            //walls.Create(new Vector2(25, 550), new Vector2(1575, 300));
            //// death floor
            //walls.Create(new Vector2(300, 500), new Vector2(600, 1000));
            //walls.Create(new Vector2(25, 550), new Vector2(1425, 300));
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
            world.UpdateCurrentCamera(gameTime);
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

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                block2.position.X -= 5;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                block2.position.X += 5;
            }

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                block2.position.Y -= 5;
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                block2.position.Y += 5;
            }

            if (keyboardState.IsKeyDown(Keys.Q))
            {
                block1.rotation -= 1;
            }
            else if (keyboardState.IsKeyDown(Keys.E))
            {
                block1.rotation += 1;
            }

            if (keyboardState.IsKeyDown(Keys.A))
            {
                block2.rotation -= 1;
            }
            else if (keyboardState.IsKeyDown(Keys.D))
            {
                block2.rotation += 1;
            }

            //if (keyboardState.IsKeyDown(Keys.Left))
            //{
            //    player.MoveLeft();
            //}
            //else if (keyboardState.IsKeyDown(Keys.Right))
            //{
            //    player.MoveRight();
            //}

            //if (keyboardState.IsKeyDownAndUp(Keys.Space, previousKeyboardState))
            //{
            //    player.Jump();
            //}

            //player.Update(gameTime);
            //walls.Update(gameTime, player);
            //wall.Update(gameTime);
            block1.Update(gameTime);
            block2.Update(gameTime);

            previousKeyboardState = keyboardState;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GLX.Collisions.MTV? mtv = GLX.Collisions.HelperMethods.Colliding(block1.polygon, block2.polygon);
            if (mtv == null)
            {
                line.visible = false;
                GraphicsDevice.Clear(Color.CornflowerBlue);
            }
            else
            {
                line.visible = true;
                line.point1 = new Vector2(500);
                line.point2 = new Vector2(500) + mtv.Value.vector * mtv.Value.magnitude;
                GraphicsDevice.Clear(Color.Red);
            }

            world.DrawWorld();

            base.Draw(gameTime);
        }

        void MainDraw()
        {
            world.BeginDraw();
            //world.Draw(walls.Draw);
            //world.Draw(player.Draw);
            //world.Draw(wall.Draw);
            world.Draw(block1.Draw);
            world.Draw(block2.Draw);
            if (line.visible)
            {
                world.Draw(line.Draw);
            }
            world.EndDraw();
        }
    }
}
