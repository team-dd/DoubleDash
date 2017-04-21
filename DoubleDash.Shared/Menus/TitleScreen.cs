using System;
using System.Collections.Generic;
using System.Text;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoubleDash.Menus
{
    public class TitleScreen
    {
        public const string TitleScreenName = "titleScreen";
        private MenuState menuState;
        public Camera Camera { get; private set; }
        private Sprite title;
        private Sprite cover;
        private float fade;
        private bool fadingIn;
        private TimeSpan displayTime;
        public bool Done
        {
            get
            {
                return !fadingIn && fade >= 1;
            }
        }

        public TitleScreen(MenuState menuState, VirtualResolutionRenderer vrr, Camera camera, Texture2D texture, GraphicsDeviceManager graphics)
        {
            Camera = camera;
            title = new Sprite(texture);
            title.origin = Vector2.Zero;
            cover = new Sprite(graphics);
            cover.DrawSize = new Size(vrr.WindowResolution.Width, vrr.WindowResolution.Height);
            cover.color = Color.Black;
            this.menuState = menuState;
            SetUpMenuState();
            fade = 1;
            fadingIn = true;
            displayTime = TimeSpan.FromSeconds(3);
        }

        private void SetUpMenuState()
        {
            menuState.AddDraw(Draw);
        }

        public void Update(GameTimeWrapper gameTime)
        {
            if (fadingIn)
            {
                fade -= 0.01f;
            }
            if (fade <= 0)
            {
                fadingIn = false;
                displayTime -= gameTime.ElapsedGameTime;
            }
            if (displayTime <= TimeSpan.Zero)
            {
                fade += 0.01f;
            }
            if (fade >= 1)
            {
                fade = 1;
            }
            cover.alpha = fade;
        }

        public void Draw()
        {
            menuState.world.BeginDraw();
            menuState.world.Draw(title.Draw);
            menuState.world.Draw(cover.Draw);
            menuState.world.EndDraw();
        }
    }
}
