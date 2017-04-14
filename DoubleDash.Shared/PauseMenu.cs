using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace DoubleDash
{
    public class PauseMenu
    {
        private MenuState menuState;
        private VirtualResolutionRenderer vrr;
        private Camera gameCamera;

        private Vector2 menuStateInitialPosition;
        private TextItem pauseTitle;
        private Vector2 pauseTitleInitialPosition;
        private Sprite background;

        public PauseMenu(MenuState menuState, VirtualResolutionRenderer vrr, Camera gameCamera, GraphicsDeviceManager graphics)
        {
            this.menuState = menuState;
            this.vrr = vrr;
            this.gameCamera = gameCamera;
            pauseTitle = new TextItem(menuState.MenuFont, "Paused");
            pauseTitleInitialPosition = new Vector2(vrr.VirtualResolution.Width / 2, vrr.VirtualResolution.Height / 2);
            pauseTitle.position = pauseTitleInitialPosition;
            menuStateInitialPosition = new Vector2(vrr.VirtualResolution.Width / 2, vrr.VirtualResolution.Height * 0.66f);
            menuState.initialPosition = menuStateInitialPosition;
            background = new Sprite(graphics);
            background.DrawSize = new Size(vrr.WindowResolution.Width, vrr.WindowResolution.Height);
            background.color = new Color(0, 0, 0, 128);
        }

        public void Update(GameTimeWrapper gameTime)
        {
            pauseTitle.position = Vector2.Transform(pauseTitleInitialPosition, gameCamera.InverseTransform);
            menuState.initialPosition = Vector2.Transform(menuStateInitialPosition, gameCamera.InverseTransform);
            background.position = Vector2.Transform(Vector2.Zero, gameCamera.InverseTransform);
            background.DrawSize = new Vector2(
                vrr.WindowResolution.Width * (1 / gameCamera.Zoom),
                vrr.WindowResolution.Height * (1 / gameCamera.Zoom));

            background.Update(gameTime);
            menuState.UpdateMenuState();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            background.Draw(spriteBatch);
            pauseTitle.Draw(spriteBatch);
        }
    }
}
