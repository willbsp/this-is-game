using System;
using Microsoft.Xna.Framework.Input;

namespace computing_project
{
    public static class InputHandler
    {
        static KeyboardState currentKeyState;
        static KeyboardState previousKeyState;

        static GamePadState currentGamepadState;
        static GamePadState previousGamepadState;

        // Get the current state of the keyboard.
        public static KeyboardState GetState()
        {
            previousKeyState = currentKeyState;
            currentKeyState = Keyboard.GetState();
            return currentKeyState;
        }

        public static GamePadState GetGamepadState()
        {

            if (GamePad.GetCapabilities(0).IsConnected)
                previousGamepadState = currentGamepadState;
                currentGamepadState = GamePad.GetState(0);
                return currentGamepadState;

        }

        // Find if the key was pressed, oneShot should be true if the key is not expected to be continuously held.
        public static bool IsKeyPressed(Keys key, bool oneShot = false)
        {
            if (!oneShot)
                return currentKeyState.IsKeyDown(key);
            return currentKeyState.IsKeyDown(key) && !previousKeyState.IsKeyDown(key);
        }

        public static bool IsButtonPressed(Buttons button, bool oneShot = false)
        {
            if (!oneShot)
                return currentGamepadState.IsButtonDown(button);
            return (currentGamepadState.IsButtonDown(button) && !previousGamepadState.IsButtonDown(button));
        }

        // Find if the key is up.
        public static bool IsKeyUp(Keys key)
        {
            return currentKeyState.IsKeyUp(key);
        }

        public static bool IsButtonUp(Buttons button)
        {
            return currentGamepadState.IsButtonUp(button);
        }

    }
}

