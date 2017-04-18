using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace DoubleDash.Menus
{
    public class MainMenu
    {
        public const string MainMenuName = "mainMenu";

        public StarBackgroundManager starBackground;

        private MenuState menuState;
        public Camera Camera { get; private set; }
        private TextItem title;

        public MainMenu(MenuState menuState, VirtualResolutionRenderer vrr, Camera camera, GraphicsDeviceManager graphics)
        {
            Camera = camera;
            title = new TextItem(menuState.MenuFont, "Sprint");
            title.position = new Vector2(vrr.VirtualResolution.Width / 2, vrr.VirtualResolution.Height / 2);
            menuState.initialPosition = new Vector2(vrr.VirtualResolution.Width / 2, vrr.VirtualResolution.Height * 0.66f);
            starBackground = new StarBackgroundManager(graphics);
            starBackground.Create(5, vrr);
            this.menuState = menuState;
            SetUpMenuState();
        }

        private void SetUpMenuState()
        {
            menuState.AddDraw(Draw);
        }

        public void Update(GameTimeWrapper gameTime)
        {
            Camera.Update(gameTime);
            starBackground.MoveLeft(3);
            starBackground.Update(gameTime, Camera);
            menuState.UpdateMenuState();
        }

        public void Draw()
        {
            menuState.world.BeginDraw();
            menuState.world.Draw(title.Draw);
            menuState.world.Draw(starBackground.Draw);
            menuState.world.EndDraw();
        }
    }
}
