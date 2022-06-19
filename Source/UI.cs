using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace computing_project
{

    public enum GameState
    {
        INGAME,
        GAMEOVER,
        MENU,
        PAUSED,
        WON
    }

    public class UI
    {
        private Sprite _healthImage; // The image to display the health.
        public static int Health; // The value of the players health.
        public static bool Key;
        public static bool Died { get; set; } // If the player has just died.
        public static bool Won { get; set; } // If the player has won

        public static GameState CurrentGameState; // Current state that the game is in.
        private bool topSelected = true; // Currently selected menu item.

        private bool _deathSoundPlayed = false; // Whether or not the sound has been played at all.
        private Timer _deathSoundTimer; // Time until the death sound finishes playing.
        private bool _winSoundPlayed = false; // Whether or not the sound has been played.
        private Timer _winTimer; // Time until the UI shows when the player wins.

        private SpriteFont _font; // The font of the UI.

        public UI()
        {
            _healthImage = new Sprite("ui_health", 0, 0, 256, 16);
            _deathSoundTimer = new Timer();
            _winTimer = new Timer();
            CurrentGameState = GameState.MENU;
            Died = false;
            Won = false;
        }

        public void Load(Game game)
        {
            _healthImage.Load(game);
            _font = game.Content.Load<SpriteFont>("font");
        }

        public void Update(GameTime gameTime, Game game)
        {

            // Play the death sound after the player dies.
            if (Died && !_deathSoundPlayed)
            {
                AudioHandler.LoadSong("deathsound", game);
                AudioHandler.PlaySong(false);
                _deathSoundPlayed = true;
                _deathSoundTimer.WaitTime(AudioHandler.GetSongDuration());
            }

            if (Won && !_winSoundPlayed)
            {
                AudioHandler.LoadSong("victory", game);
                AudioHandler.MusicVolume = 0.7f;
                AudioHandler.PlaySong(true);
                _winSoundPlayed = true;
                _winTimer.WaitTime(6f);
            }

            if (_winSoundPlayed && _winTimer.Done)
            {
                CurrentGameState = GameState.WON;
            }

            // Once it has finished playing, change the game state to a game over.
            if (!(CurrentGameState == GameState.GAMEOVER) && _deathSoundPlayed && _deathSoundTimer.Done)
            {
                CurrentGameState = GameState.GAMEOVER;
            }

            // In game ...
            if (CurrentGameState == GameState.INGAME && (InputHandler.IsKeyPressed(InputConstants.PAUSED, true) || InputHandler.IsButtonPressed(InputConstants.GAMEPAD_PAUSED, true)) && !Died)
            {
                CurrentGameState = GameState.PAUSED;
            }

            // On the menus .....
            if (CurrentGameState == GameState.GAMEOVER || CurrentGameState == GameState.MENU || CurrentGameState == GameState.PAUSED || CurrentGameState == GameState.WON)
            {

                // If up or down keys are pressed then change the currently selected option.
                if (InputHandler.IsKeyPressed(InputConstants.UP, true) || InputHandler.IsKeyPressed(InputConstants.DOWN, true) || InputHandler.IsKeyPressed(InputConstants.UP_ALT, true) || InputHandler.IsKeyPressed(InputConstants.DOWN_ALT, true) || InputHandler.IsButtonPressed(InputConstants.GAMEPAD_DOWN, true) || InputHandler.IsButtonPressed(InputConstants.GAMEPAD_UP, true))
                {
                    topSelected = !topSelected;
                }

                // If the top item is selected then start the game else exit the game if the bottom is selected.
                if (topSelected)
                {
                    if (InputHandler.IsKeyPressed(InputConstants.ATTACK, true) || InputHandler.IsKeyPressed(InputConstants.ATTACK_ALT, true) || InputHandler.IsButtonPressed(InputConstants.GAMEPAD_ATTACK, true))
                    {
                        if (CurrentGameState == GameState.GAMEOVER || CurrentGameState == GameState.MENU || CurrentGameState == GameState.WON)
                        {
                            Main.StartGame = true;
                            InputHandler.GetState();
                            InputHandler.GetGamepadState();
                        }
                        else if(CurrentGameState == GameState.PAUSED)
                        {
                            CurrentGameState = GameState.INGAME;
                        }
                    }
                }
                else
                {
                    if (InputHandler.IsKeyPressed(InputConstants.ATTACK, true) || InputHandler.IsKeyPressed(InputConstants.ATTACK_ALT, true) || InputHandler.IsButtonPressed(InputConstants.GAMEPAD_ATTACK))
                    {
                        game.Exit();
                    }
                }

            }

            _deathSoundTimer.Update(gameTime);
            _winTimer.Update(gameTime);

        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {

            if (CurrentGameState == GameState.GAMEOVER)
            {

                // Find the middle point for the gameover text, so that it is centred.
                Vector2 gameoverPoint = _font.MeasureString(UIConstants.GAME_OVER_TITLE_TEXT) / 2;
                Vector2 gameoverPosition = new Vector2(RenderConstants.RENDER_WIDTH / 2, (RenderConstants.RENDER_HEIGHT / 2) - 20);

                // Draw the game over message.
                spriteBatch.DrawString(_font, UIConstants.GAME_OVER_TITLE_TEXT, gameoverPosition, Color.White, 0, gameoverPoint, 1f, SpriteEffects.None, 0.5f);

                // Find the middle points.
                Vector2 continuePoint = _font.MeasureString(UIConstants.GAME_OVER_START_TEXT) / 2;
                Vector2 continuePosition = new Vector2(RenderConstants.RENDER_WIDTH / 2, (RenderConstants.RENDER_HEIGHT / 2) + 20);
                Vector2 quitPoint = _font.MeasureString(UIConstants.GAME_OVER_QUIT_TEXT) / 2;
                Vector2 quitPosition = new Vector2(RenderConstants.RENDER_WIDTH / 2, (RenderConstants.RENDER_HEIGHT / 2) + 40);

                // If they are selected then draw them in blue rather then white.
                if (topSelected)
                {
                    spriteBatch.DrawString(_font, UIConstants.GAME_OVER_START_TEXT, continuePosition, Color.Blue, 0, continuePoint, 0.5f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(_font, UIConstants.GAME_OVER_QUIT_TEXT, quitPosition, Color.White, 0, quitPoint, 0.5f, SpriteEffects.None, 0.5f);
                }
                else
                {
                    spriteBatch.DrawString(_font, UIConstants.GAME_OVER_START_TEXT, continuePosition, Color.White, 0, continuePoint, 0.5f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(_font, UIConstants.GAME_OVER_QUIT_TEXT, quitPosition, Color.Blue, 0, quitPoint, 0.5f, SpriteEffects.None, 0.5f);
                }

            }

            if (CurrentGameState == GameState.WON)
            {

                // Find the middle point for the gameover text, so that it is centred.
                Vector2 gameoverPoint = _font.MeasureString(UIConstants.WON_TITLE_TEXT) / 2;
                Vector2 gameoverPosition = new Vector2(RenderConstants.RENDER_WIDTH / 2, (RenderConstants.RENDER_HEIGHT / 2) - 20);

                // Draw the game over message.
                spriteBatch.DrawString(_font, UIConstants.WON_TITLE_TEXT, gameoverPosition, Color.White, 0, gameoverPoint, 1f, SpriteEffects.None, 0.5f);

                // Find the middle points.
                Vector2 continuePoint = _font.MeasureString(UIConstants.WON_START_TEXT) / 2;
                Vector2 continuePosition = new Vector2(RenderConstants.RENDER_WIDTH / 2, (RenderConstants.RENDER_HEIGHT / 2) + 20);
                Vector2 quitPoint = _font.MeasureString(UIConstants.WON_QUIT_TEXT) / 2;
                Vector2 quitPosition = new Vector2(RenderConstants.RENDER_WIDTH / 2, (RenderConstants.RENDER_HEIGHT / 2) + 40);

                // If they are selected then draw them in blue rather then white.
                if (topSelected)
                {
                    spriteBatch.DrawString(_font, UIConstants.WON_START_TEXT, continuePosition, Color.Blue, 0, continuePoint, 0.5f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(_font, UIConstants.WON_QUIT_TEXT, quitPosition, Color.White, 0, quitPoint, 0.5f, SpriteEffects.None, 0.5f);
                }
                else
                {
                    spriteBatch.DrawString(_font, UIConstants.WON_START_TEXT, continuePosition, Color.White, 0, continuePoint, 0.5f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(_font, UIConstants.WON_QUIT_TEXT, quitPosition, Color.Blue, 0, quitPoint, 0.5f, SpriteEffects.None, 0.5f);
                }

            }

            if (CurrentGameState == GameState.MENU)
            {

                // Find the middle point for the gameover text, so that it is centred.
                Vector2 gameoverPoint = _font.MeasureString(UIConstants.MENU_TITLE_TEXT) / 2;
                Vector2 gameoverPosition = new Vector2(RenderConstants.RENDER_WIDTH / 2, (RenderConstants.RENDER_HEIGHT / 2) - 20);

                // Draw the game over message.
                spriteBatch.DrawString(_font, UIConstants.MENU_TITLE_TEXT, gameoverPosition, Color.White, 0, gameoverPoint, 1f, SpriteEffects.None, 0.5f);

                // Find the middle points.
                Vector2 continuePoint = _font.MeasureString(UIConstants.MENU_START_TEXT) / 2;
                Vector2 continuePosition = new Vector2(RenderConstants.RENDER_WIDTH / 2, (RenderConstants.RENDER_HEIGHT / 2) + 20);
                Vector2 quitPoint = _font.MeasureString(UIConstants.MENU_QUIT_TEXT) / 2;
                Vector2 quitPosition = new Vector2(RenderConstants.RENDER_WIDTH / 2, (RenderConstants.RENDER_HEIGHT / 2) + 40);

                // If they are selected then draw them in blue rather then white.
                if (topSelected)
                {
                    spriteBatch.DrawString(_font, UIConstants.MENU_START_TEXT, continuePosition, Color.Blue, 0, continuePoint, 0.5f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(_font, UIConstants.MENU_QUIT_TEXT, quitPosition, Color.White, 0, quitPoint, 0.5f, SpriteEffects.None, 0.5f);
                }
                else
                {
                    spriteBatch.DrawString(_font, UIConstants.MENU_START_TEXT, continuePosition, Color.White, 0, continuePoint, 0.5f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(_font, UIConstants.MENU_QUIT_TEXT, quitPosition, Color.Blue, 0, quitPoint, 0.5f, SpriteEffects.None, 0.5f);
                }

            }


            if (CurrentGameState == GameState.PAUSED)
            {

                // Find the middle point for the gameover text, so that it is centred.
                Vector2 gameoverPoint = _font.MeasureString(UIConstants.PAUSE_TITLE_TEXT) / 2;
                Vector2 gameoverPosition = new Vector2(RenderConstants.RENDER_WIDTH / 2, (RenderConstants.RENDER_HEIGHT / 2) - 20);

                // Draw the game over message.
                spriteBatch.DrawString(_font, UIConstants.PAUSE_TITLE_TEXT, gameoverPosition, Color.White, 0, gameoverPoint, 1f, SpriteEffects.None, 0.5f);

                // Find the middle points.
                Vector2 continuePoint = _font.MeasureString(UIConstants.PAUSE_START_TEXT) / 2;
                Vector2 continuePosition = new Vector2(RenderConstants.RENDER_WIDTH / 2, (RenderConstants.RENDER_HEIGHT / 2) + 20);
                Vector2 quitPoint = _font.MeasureString(UIConstants.PAUSE_QUIT_TEXT) / 2;
                Vector2 quitPosition = new Vector2(RenderConstants.RENDER_WIDTH / 2, (RenderConstants.RENDER_HEIGHT / 2) + 40);

                // If they are selected then draw them in blue rather then white.
                if (topSelected)
                {
                    spriteBatch.DrawString(_font, UIConstants.PAUSE_START_TEXT, continuePosition, Color.Blue, 0, continuePoint, 0.5f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(_font, UIConstants.PAUSE_QUIT_TEXT, quitPosition, Color.White, 0, quitPoint, 0.5f, SpriteEffects.None, 0.5f);
                }
                else
                {
                    spriteBatch.DrawString(_font, UIConstants.PAUSE_START_TEXT, continuePosition, Color.White, 0, continuePoint, 0.5f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(_font, UIConstants.PAUSE_QUIT_TEXT, quitPosition, Color.Blue, 0, quitPoint, 0.5f, SpriteEffects.None, 0.5f);
                }

            }

            if (CurrentGameState == GameState.INGAME)
            {
                // The rectangle behind the hearts. Size is adjusted based on health value.
                Rectangle healthRectangle = new Rectangle();

                // If the players health is equal to zero then make sure the rectangle is not x = 0 or else it will error.
                if (Health != 0)
                {
                    // Width will be 14 times the health value, to correctly fill the hearts.
                    healthRectangle = new Rectangle(52, 3, 14 * Health, 9);
                }
                else
                {
                    healthRectangle = new Rectangle(52, 3, 1, 9);
                }

                // Creates the color data for the rectangle.
                Color[] healthData = new Color[healthRectangle.Width * healthRectangle.Height];

                // Creates the rectangle texture to draw the rectangle
                Texture2D healthTexture = new Texture2D(graphicsDevice, healthRectangle.Width, healthRectangle.Height);

                // Makes the rectangle red.
                for (int i = 0; i < healthData.Length; ++i)
                    healthData[i] = Color.Red;
                healthTexture.SetData(healthData);


                // The rectangle behind the key. Only appears if the player has a key.
                Rectangle keyRectangle = new Rectangle();

                // Define the rectangle.
                keyRectangle = new Rectangle(236, 4, 16, 8);

                // Creates the color data for the rectangle.
                Color[] keyData = new Color[keyRectangle.Width * keyRectangle.Height];

                // Creates the rectangle texture to draw the rectangle
                Texture2D keyTexture = new Texture2D(graphicsDevice, keyRectangle.Width, keyRectangle.Height);

                // Makes the rectangle white.
                for (int i = 0; i < keyData.Length; ++i)
                    keyData[i] = Color.White;
                keyTexture.SetData(keyData);

                // Background Rectangle
                Rectangle bkRectangle = new Rectangle(0, 0, 256, 16);
                Color[] bkData = new Color[bkRectangle.Width * bkRectangle.Height];
                Texture2D bkTexture = new Texture2D(graphicsDevice, bkRectangle.Width, bkRectangle.Height);
                for (int i = 0; i < bkData.Length; ++i)
                    bkData[i] = new Color(40, 8, 48);
                bkTexture.SetData(bkData);

                // Draw the rectangle and then the image over the top of it.
                spriteBatch.Draw(bkTexture, new Vector2(0, 0), Color.White);
                spriteBatch.Draw(healthTexture, new Vector2(healthRectangle.Left, healthRectangle.Top), Color.White);
                if (Key)
                    spriteBatch.Draw(keyTexture, new Vector2(keyRectangle.Left, keyRectangle.Top), Color.White);
                _healthImage.Draw(spriteBatch, 0, 0);
            }


        }

    }
}
