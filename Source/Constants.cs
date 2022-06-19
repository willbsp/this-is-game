using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace computing_project
{

    public static class RenderConstants
    {

        // Constants for the rendering resolution.
        public const int RENDER_WIDTH = 256;
        public const int RENDER_HEIGHT = 224;
        // The amount of times to scale to rendering resolution to get the window resolution.
        public const int VIEWPORT_SCALE = 4;
        // To draw colliders
        public const bool RENDER_DEBUG = false;

    }

    public static class InputConstants
    {

        public const Keys UP = Keys.W;
        public const Keys UP_ALT = Keys.Up;
        public const Keys DOWN = Keys.S;
        public const Keys DOWN_ALT = Keys.Down;
        public const Keys LEFT = Keys.A;
        public const Keys LEFT_ALT = Keys.Left;
        public const Keys RIGHT = Keys.D;
        public const Keys RIGHT_ALT = Keys.Right;
        public const Keys ATTACK = Keys.E;
        public const Keys ATTACK_ALT = Keys.Space;
        public const Keys PAUSED = Keys.Escape;

        public const Buttons GAMEPAD_UP = Buttons.LeftThumbstickUp;
        public const Buttons GAMEPAD_DOWN = Buttons.LeftThumbstickDown;
        public const Buttons GAMEPAD_LEFT = Buttons.LeftThumbstickLeft;
        public const Buttons GAMEPAD_RIGHT = Buttons.LeftThumbstickRight;
        public const Buttons GAMEPAD_ATTACK = Buttons.B;
        public const Buttons GAMEPAD_PAUSED = Buttons.Start;

    }

    public static class UIConstants
    {
        public const string MENU_START_TEXT = "START GAME";
        public const string MENU_TITLE_TEXT = "THIS IS GAME";
        public const string MENU_QUIT_TEXT = "QUIT";

        public const string GAME_OVER_START_TEXT = "CONTINUE";
        public const string GAME_OVER_TITLE_TEXT = "YOU DIED!";
        public const string GAME_OVER_QUIT_TEXT = "QUIT";

        public const string PAUSE_START_TEXT = "CONTINUE";
        public const string PAUSE_TITLE_TEXT = "PAUSED";
        public const string PAUSE_QUIT_TEXT = "QUIT";

        public const string WON_START_TEXT = "PLAY AGAIN";
        public const string WON_TITLE_TEXT = "YOU WON!";
        public const string WON_QUIT_TEXT = "QUIT";
    }

    public static class PlayerConstants 
    {

        public const float MOVE_SPEED = 0.06f;
        public const int MAX_HEALTH = 6;

        public const int BOUNDING_BOX_TOP_X = 5;
        public const int BOUNDING_BOX_TOP_Y = 9;
        public const int BOUNDING_BOX_TOP_WIDTH = 13;
        public const int BOUNDING_BOX_TOP_HEIGHT = 13;

        public const float ATTACK_DURATION = 0.2f;
        public const float ATTACK_COOLDOWN = 0.35f;
        public const float ATTACK_KNOCKBACK = 18;
        public const int ATTACK_DAMAGE = 1;

        public const int ATTACK_BOX_TOP_X = 4;
        public const int ATTACK_BOX_TOP_Y = 0;
        public const int ATTACK_BOX_TOP_WIDTH = 16;
        public const int ATTACK_BOX_TOP_HEIGHT = 9;

        public const int ATTACK_BOX_BOTTOM_X = 4;
        public const int ATTACK_BOX_BOTTOM_Y = 21;
        public const int ATTACK_BOX_BOTTOM_WIDTH = 16;
        public const int ATTACK_BOX_BOTTOM_HEIGHT = 9;

        public const int ATTACK_BOX_LEFT_X = 1;
        public const int ATTACK_BOX_LEFT_Y = 5;
        public const int ATTACK_BOX_LEFT_WIDTH = 7;
        public const int ATTACK_BOX_LEFT_HEIGHT = 16;

        public const int ATTACK_BOX_RIGHT_X = 16;
        public const int ATTACK_BOX_RIGHT_Y = 5;
        public const int ATTACK_BOX_RIGHT_WIDTH = 7;
        public const int ATTACK_BOX_RIGHT_HEIGHT = 16;

    }

}
