using System;
using System.Collections.Generic;
using System.Text;
using DoubleDash.Menus;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace DoubleDash
{
    public class StateManager
    {
        public const string MenuCamera = "menuCamera";

        public enum States
        {
            TitleScreen,
            Game,
            MainMenu,
            WorldSelectMenu,
            PauseMenu
        }

        private Game game;
        private World world;
        private GraphicsDeviceManager graphics;
        private TimeManager timeManager;
        private LevelManager levelManager;
        private GameTimer gameTimer;
        private Player player;
        private Song menuMusic;
        private Song gameMusic;
        

        private States state;
        public States State
        {
            get
            {
                return state;
            }
            set
            {
                if (value == States.TitleScreen)
                {
                    world.CurrentCameraName = MenuCamera;
                    world.ActivateMenuState(TitleScreen.TitleScreenName);
                }
                else if (value == States.MainMenu)
                {
                    if (state == States.WorldSelectMenu)
                    {
                        world.DeactivateMenuState(WorldSelectMenu.WorldSelectMenuName);
                        world.ActivateMenuState(MainMenu.MainMenuName);
                    }
                    else if (state == States.TitleScreen)
                    {
                        world.DeactivateMenuState(TitleScreen.TitleScreenName);
                        world.ActivateMenuState(MainMenu.MainMenuName);
                        MediaPlayer.Play(menuMusic);
                    }
                    else
                    {
                        MediaPlayer.Play(menuMusic);
                        world.DeactivateGameState(Game1.MainGame);
                        world.DeactivateMenuState(PauseMenu.PauseMenuName);
                        world.ActivateMenuState(MainMenu.MainMenuName);
                        world.CurrentCameraName = MenuCamera;
                    }
                }
                else if (value == States.WorldSelectMenu)
                {
                    world.DeactivateMenuState(MainMenu.MainMenuName);
                    world.ActivateMenuState(WorldSelectMenu.WorldSelectMenuName);
                }
                else if (value == States.Game)
                {
                    world.CurrentCameraName = World.Camera1Name;
                    if (state == States.MainMenu ||
                        state == States.WorldSelectMenu)
                    {
                        SwitchToGame();
                    }
                    else if (state == States.PauseMenu)
                    {
                        world.DeactivateMenuState(PauseMenu.PauseMenuName);
                        timeManager.Play();
                    }
                }
                else if (value == States.PauseMenu)
                {
                    world.CurrentCameraName = World.Camera1Name;
                    world.ActivateMenuState(PauseMenu.PauseMenuName);
                    timeManager.Pause();
                }
                state = value;
            }
        }

        private Camera menuCamera;

        private MenuState titleScreenState;
        private TitleScreen titleScreen;

        private MenuState mainMenuState;
        private MainMenu mainMenu;

        private MenuState worldSelectMenuState;
        private WorldSelectMenu worldSelectMenu;

        private MenuState pauseMenuState;
        private PauseMenu pauseMenu;

        public StateManager(Game game,
            World world,
            GraphicsDeviceManager graphics,
            TimeManager timeManager,
            LevelManager levelManager,
            GameTimer gameTimer,
            Player player,
            Song menuMusic,
            Song gameMusic)
        {
            this.game = game;
            this.world = world;
            this.graphics = graphics;
            this.timeManager = timeManager;
            this.levelManager = levelManager;
            this.gameTimer = gameTimer;
            this.player = player;
            this.menuMusic = menuMusic;
            this.gameMusic = gameMusic;

            menuCamera = new Camera(world.virtualResolutionRenderer, Camera.CameraFocus.Center);
            world.AddCamera(MenuCamera, menuCamera);

            SetupTitleScreen();
            SetupMainMenu();
            SetUpWorldSelectMenu();
            SetupPauseMenu();
        }

        private void SetupTitleScreen()
        {
            titleScreenState = world.AddMenuState(TitleScreen.TitleScreenName);
            titleScreen = new TitleScreen(titleScreenState, world.virtualResolutionRenderer, menuCamera, World.TextureManager["title_screen"], graphics);
            titleScreenState.AddTime(new GameTimeWrapper(titleScreen.Update, game, 1));
        }

        private void SetupMainMenu()
        {
            mainMenuState = world.AddMenuState(MainMenu.MainMenuName);
            SetMenuStateItems(mainMenuState);
            mainMenuState.AddMenuItems("Play", "World Select", "Quit");
            mainMenuState.SetMenuAction("Play", () => { State = States.Game; });
            mainMenuState.SetMenuAction("World Select", () => { State = States.WorldSelectMenu; });
            mainMenuState.SetMenuAction("Quit", () => { game.Exit(); });
            mainMenu = new MainMenu(mainMenuState, world.virtualResolutionRenderer, menuCamera, graphics);
            mainMenuState.AddTime(new GameTimeWrapper(mainMenu.Update, game, 1));
        }

        private void SetUpWorldSelectMenu()
        {
            worldSelectMenuState = world.AddMenuState(WorldSelectMenu.WorldSelectMenuName);
            SetMenuStateItems(worldSelectMenuState);
            worldSelectMenuState.AddMenuItems("World 1", "World 2", "Back");
            worldSelectMenuState.SetMenuAction("World 1", () =>
            {
                SwitchToGame(LevelManager.Worlds.World1);
            });
            worldSelectMenuState.SetMenuAction("World 2", () =>
            {
                SwitchToGame(LevelManager.Worlds.World2);
            });
            worldSelectMenuState.SetMenuAction("Back", () => { State = States.MainMenu; });
            worldSelectMenuState.BackAction = new Action(() => { State = States.MainMenu; });
            worldSelectMenu = new WorldSelectMenu(worldSelectMenuState, world.virtualResolutionRenderer, menuCamera, graphics);
            worldSelectMenuState.AddTime(new GameTimeWrapper(worldSelectMenu.Update, game, 1));
        }

        private void SetupPauseMenu()
        {
            pauseMenuState = world.AddMenuState(PauseMenu.PauseMenuName);
            SetMenuStateItems(pauseMenuState);
            pauseMenuState.AddMenuItems("Resume", "Main Menu");
            pauseMenuState.SetMenuAction("Resume", () => { State = States.Game; });
            pauseMenuState.SetMenuAction("Main Menu", () => { State = States.MainMenu; });
            pauseMenuState.BackAction = new Action(() => { State = States.Game; });
            pauseMenu = new PauseMenu(pauseMenuState, world.virtualResolutionRenderer, world.Cameras[World.Camera1Name], graphics);
            pauseMenuState.AddTime(new GameTimeWrapper(pauseMenu.Update, game, 1));
        }

        private void SetMenuStateItems(MenuState menuState)
        {
            menuState.MenuFont = World.FontManager["Fonts/Montserrat_Ultra_Light_36"];
            menuState.UnselectedColor = Color.Gray;
            menuState.SelectedColor = Color.White;
        }

        private void SwitchToGame()
        {
            SwitchToGame(LevelManager.Worlds.World1);
        }

        private void SwitchToGame(LevelManager.Worlds startingWorld)
        {
            world.CurrentCameraName = World.Camera1Name;
            MediaPlayer.Play(gameMusic);
            world.DeactivateMenuState(MainMenu.MainMenuName);
            world.DeactivateMenuState(WorldSelectMenu.WorldSelectMenuName);
            world.ActivateGameState(Game1.MainGame);
            levelManager.SetLevel(startingWorld, player, world.Cameras[World.Camera1Name], gameTimer);
            timeManager.Reset();
            state = States.Game;
        }

        public void Update()
        {
            if (titleScreen.Done && State == States.TitleScreen)
            {
                State = States.MainMenu;
            }
        }
    }
}
