using System;
using System.Collections.Generic;
using System.Text;
using GLX;
using Microsoft.Xna.Framework;

namespace DoubleDash.Menus
{
    public class WorldSelectMenu
    {
        public const string WorldSelectMenuName = "wordSelectMenu";

        public StarBackgroundManager starBackground;

        private MenuState menuState;
        public Camera Camera { get; private set; }
        private TextItem title;

        public WorldSelectMenu(MenuState menuState, VirtualResolutionRenderer vrr, Camera camera, GraphicsDeviceManager graphics)
        {
            Camera = camera;
            title = new TextItem(menuState.MenuFont, "World Select");
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
