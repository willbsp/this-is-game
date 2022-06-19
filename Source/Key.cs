using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace computing_project
{
    class Key : Sprite
    {

        private Collider _collider;
        private int _x;
        private int _y;
        private string _name;

        public Key(int x, int y, string name) : base("key", 0, 0, 16, 16)
        {
            _collider = new Collider(new Rectangle(x, y, 16, 16));
            _x = x;
            _y = y;
            _name = name;
        }

        public string GetName()
        {
            return _name;
        }

        public Collider GetCollider()
        {
            return _collider;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch, _x, _y);
        }

    }
}
