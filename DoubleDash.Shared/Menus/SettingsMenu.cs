﻿using System;
using System.Collections.Generic;
using System.Text;
using GLX;
using Microsoft.Xna.Framework;

namespace DoubleDash.Menus
{
    public class SettingsMenu
    {
        public const string SettingsMenuName = "settingsMenu";

        public StarBackgroundManager starBackground;

        private MenuState menuState;
        public Camera Camera { get; private set; }
        private TextItem title;

        public SettingsMenu(MenuState menuState, VirtualResolutionRenderer vrr, Camera camera, GraphicsDeviceManager graphics)
        {
            Camera = camera;
            title = new TextItem(menuState.MenuFont, "Settings");
            title.scale = 2.5f;
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
