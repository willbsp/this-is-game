using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace computing_project
{
    // --------------------------------------------------------------------
    // Sprite - Holds information for rendering individual static sprites.
    // --------------------------------------------------------------------

    class Sprite
    {

        private readonly string _fileName;
        protected Texture2D _texture;
        protected Rectangle _sourceRect;

        public Sprite(string fileName, int sourceX, int sourceY, int width, int height)
        {
            _fileName = fileName;
            _sourceRect = new Rectangle(sourceX, sourceY, width, height);
        }

        // Load the file from the pipeline.
        public virtual void Load(Game game)
        {
            _texture = game.Content.Load<Texture2D>(_fileName);
        }

        // Draw the sprite in the spritebatch. x and y are where to draw on the game screen.
        public virtual void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            spriteBatch.Draw(_texture, new Vector2(x, y), _sourceRect, Color.White);
        }

    }

    class AnimatedSprite : Sprite
    {
        // Which frame the animation is currently on.
        private int _frameIndex;
        // Duration of each frame.
        private readonly float _timeToUpdate;
        // Time since last frame was shown.
        private double _timeElapsed;
        // Stores a string and list of rectangles, string is an identifer for the animation. The rectangles are each frame on the source spritesheet.
        private Dictionary<string, List<Rectangle>> _animations = new Dictionary<string, List<Rectangle>>();
        // Currently playing animation.
        protected string _currentAnimation;

        private bool _repeat;

        public AnimatedSprite(string fileName, int sourceX, int sourceY, int width, int height, float timeToUpdate) : base(fileName, sourceX, sourceY, width, height)
        {
            _timeToUpdate = timeToUpdate;
            _frameIndex = 0;
        }

        public void AddAnimation(int frames, int x, int y, string name, int width, int height)
        {
            // List of rectangles making up the frames of the animation
            List<Rectangle> rectangles = new List<Rectangle>();
            for (int i = 0; i < frames; i++)
            {
                // Add each rectangle to a list of rectangles
                Rectangle newRect = new Rectangle(x + i * width, y, width, height);
                rectangles.Add(newRect);
            }
            // Add it to the animations dictionary along with the name string to identify it
            _animations.Add(name, rectangles);
        }

        // Play the specified animation, if not already playing.
        public void PlayAnimation(string animation, bool repeat)
        {
            _repeat = repeat;
            if (_currentAnimation != animation)
            {
                _currentAnimation = animation;
                // Start animation from the beginning
                _frameIndex = 0;
            }
        }

        // Return the animation to the first frame.
        private void StopAnimation()
        {
            _frameIndex = 0;
        }

        // Used to change the current frame after specified amount of time.
        public virtual void Update(GameTime gameTime)
        {
            // Add the time since the beginning of the game to time since the last frame
            _timeElapsed += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_timeElapsed > _timeToUpdate)
            {
                // Set back to the elapsed time for the next frame
                _timeElapsed -= _timeToUpdate;
                // Move to the next frame if the last frame has not been reached.
                if (_frameIndex < _animations[_currentAnimation].Count - 1)
                {
                    _frameIndex++;
                }
                // If it has been reached, then return to the first frame.
                else
                {
                    if (_repeat)
                    {
                        StopAnimation();
                    }
                }

            }
        }

        // Override the DrawSprite() procedure to draw the specific animation frame.
        public override void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            // Create a rectangle to specify where the current frame is positioned within the sprite sheet
            Rectangle sourceRect = _animations[_currentAnimation][_frameIndex];
            // Render that specific frame
            spriteBatch.Draw(_texture, new Vector2(x, y), sourceRect, Color.White);
        }

    }

}
