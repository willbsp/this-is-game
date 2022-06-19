using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace computing_project
{

    public class Main : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        RenderTarget2D _renderTarget;

        Level _level;
        UI _ui;

        // Start the game when set true.
        public static bool StartGame = false;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = RenderConstants.RENDER_WIDTH * RenderConstants.VIEWPORT_SCALE,
                PreferredBackBufferHeight = RenderConstants.RENDER_HEIGHT * RenderConstants.VIEWPORT_SCALE
            };
            Content.RootDirectory = "Content";

        }

        protected override void Initialize()
        {
            Window.Title = "Computing Project";
            IsMouseVisible = true;

            if(UI.CurrentGameState == GameState.INGAME)
                _level = new Level();

            _ui = new UI();
            AudioHandler.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            if (UI.CurrentGameState == GameState.INGAME)
                _level.LoadLevel(this, "newmap1");

            _ui.Load(this);
            AudioHandler.AddSoundEffect("start", this);
            AudioHandler.LoadSong("thegreatsea", this);

            // Create the render target.
            _renderTarget = new RenderTarget2D(GraphicsDevice, RenderConstants.RENDER_WIDTH, RenderConstants.RENDER_HEIGHT);

        }



        protected override void UnloadContent()
        {
            Content.Unload();
        }

        public void PlayGame()
        {
            UI.CurrentGameState = GameState.INGAME;
            Initialize();
            UI.CurrentGameState = GameState.INGAME;
            LoadContent();
            AudioHandler.PlaySoundEffect("start");
            AudioHandler.PlaySong(true);
        }

        protected override void Update(GameTime gameTime)
        {
        
            InputHandler.GetState();
            InputHandler.GetGamepadState();
            _ui.Update(gameTime, this);

            if (StartGame)
            {
                StartGame = false;
                PlayGame();
            }

            // Do not continue the update when paused.
            if (UI.CurrentGameState == GameState.PAUSED || UI.Died)
                return;
                
            if (UI.CurrentGameState == GameState.INGAME)
                _level.Update(gameTime, this);

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {

            // A new render target is created  so that the game elements themselves are rendered at the render resolution.
            GraphicsDevice.SetRenderTarget(_renderTarget);
            GraphicsDevice.Clear(new Color(40, 8, 48)); // The background colour for the game!
            _spriteBatch.Begin();

            // Anything in the scene is drawn below here before spritebatch end (in the render order!).

            if (UI.CurrentGameState == GameState.INGAME)
                _level.Draw(_spriteBatch, GraphicsDevice, RenderConstants.RENDER_DEBUG);
            _ui.Draw(_spriteBatch, GraphicsDevice);

            _spriteBatch.End();

            // The final result is then scaled to the window size, this ensures consistent rendering and proper sprite scaling.
            GraphicsDevice.SetRenderTarget(null);
            SpriteBatch targetBatch = new SpriteBatch(GraphicsDevice);
            targetBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp);
            targetBatch.Draw(_renderTarget, new Rectangle(0, 0, RenderConstants.RENDER_WIDTH * RenderConstants.VIEWPORT_SCALE, RenderConstants.RENDER_HEIGHT * RenderConstants.VIEWPORT_SCALE), Color.White);
            targetBatch.End();

            base.Draw(gameTime);
        }
    }
}
