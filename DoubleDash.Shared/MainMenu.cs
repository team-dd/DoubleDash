using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace DoubleDash
{
    public class MainMenu
    {
        public const string MainMenuCamera = "mainMenuCamera";

        public StarBackgroundManager starBackground;

        private KeyboardState previousKeyboardState;
        private GamePadState previousGamePadState;

        private MenuState menuState;
        public Camera Camera { get; private set; }
        private TextItem title;

        public MainMenu(MenuState menuState, VirtualResolutionRenderer vrr, GraphicsDeviceManager graphics)
        {
            previousKeyboardState = Keyboard.GetState();
            previousGamePadState = GamePad.GetState(PlayerIndex.One);
            this.menuState = menuState;
            Camera = new Camera(vrr, Camera.CameraFocus.Center);
            title = new TextItem(menuState.MenuFont, "Double Dash");
            title.position = new Vector2(vrr.VirtualResolution.Width / 2, vrr.VirtualResolution.Height / 2);
            menuState.initialPosition = new Vector2(vrr.VirtualResolution.Width / 2, vrr.VirtualResolution.Height * 0.66f);
            starBackground = new StarBackgroundManager(graphics);
            starBackground.Create(5, vrr);
        }

        public void Update(GameTimeWrapper gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            if (keyboardState.IsKeyDownAndUp(Keys.Up, previousKeyboardState) ||
                gamePadState.IsButtonDownAndUp(Buttons.DPadUp, previousGamePadState))
            {
                menuState.CurrentSelection--;
            }
            if (keyboardState.IsKeyDownAndUp(Keys.Down, previousKeyboardState) ||
                gamePadState.IsButtonDownAndUp(Buttons.DPadDown, previousGamePadState))
            {
                menuState.CurrentSelection++;
            }

            if (keyboardState.IsKeyDownAndUp(Keys.Enter, previousKeyboardState) ||
                gamePadState.IsButtonDownAndUp(Buttons.A, previousGamePadState))
            {
                menuState.DoAction();
            }

            Camera.Update(gameTime);
            starBackground.MoveLeft(3);
            starBackground.Update(gameTime, Camera);

            previousKeyboardState = keyboardState;
            previousGamePadState = gamePadState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            title.Draw(spriteBatch);
            starBackground.Draw(spriteBatch);
        }
    }
}
