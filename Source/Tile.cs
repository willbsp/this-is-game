using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace computing_project
{
    class Tile : Sprite
    {
    
        private int _x, _y; // Position within the level

        public Tile(string fileName, int width, int height, int sourceX, int sourceY, int x, int y) : base(fileName, sourceX, sourceY, width, height)
        {
            _x = x; _y = y;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // The constant x and y coords are passed back into sprites draw function
            base.Draw(spriteBatch, _x, _y);
        }

    }
}
