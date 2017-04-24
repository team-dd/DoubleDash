using System;
using System.Collections.Generic;
using System.Text;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoubleDash.Menus
{
    public class ControlsMenu
    {
        public const string ControlMenuName = "controlMenu";

        private StarBackgroundManager starBackground;
        private Sprite controlImage;

        private MenuState menuState;
        public Camera Camera { get; private set; }

        public ControlsMenu(Texture2D controlImage, MenuState menuState, VirtualResolutionRenderer vrr, Camera camera, GraphicsDeviceManager graphics)
        {
            Camera = camera;
            this.controlImage = new Sprite(controlImage);
            this.controlImage.position = new Vector2(vrr.VirtualResolution.Width / 2, vrr.VirtualResolution.Height / 2);
            menuState.initialPosition = new Vector2(vrr.VirtualResolution.Width / 2, vrr.VirtualResolution.Height * 0.75f);
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
            menuState.world.Draw(starBackground.Draw);
            menuState.world.Draw(controlImage.Draw);
            menuState.world.EndDraw();
        }
    }
}
