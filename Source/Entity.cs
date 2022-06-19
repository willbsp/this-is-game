using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace computing_project
{
    class Entity : AnimatedSprite
    {

        protected readonly float MOVE_SPEED; // The speed at which the entity moves.
        protected readonly Rectangle BOUNDING_BOX; // The collision box that collides with the environment.
        protected readonly int MAX_HEALTH;

        protected float _dx, _dy; // The velocity for each axis.
        protected Collider _collider;
        public float X { get; protected set; } // The position of the entity.
        public float Y { get; protected set; }
        protected int _health;
        protected float _weight;

        public bool Dead { get; protected set; } = false; // If the entity is dead.

        protected Timer _knockBackTimer; // The timer controlling how long the entity remains in a knockback state for.
        protected Timer _knockBackCooldownTimer; // The timer controlling how long the entity has until it can go back into knockback.
        protected bool _isKnockback = false; // Whether or not the entity is currently in a knockback state.

        public Entity(int x, int y, string fileName, int sourceX, int sourceY, int width, int height, float timeToUpdate, float moveSpeed, int maxHealth, Rectangle boundingBox, float weight) : base(fileName, sourceX, sourceY, width, height, timeToUpdate)
        {
            _dy = _dx = 0; // Inital velocity is zero.
            X = x;
            Y = y;
            BOUNDING_BOX = boundingBox;
            _collider = new Collider(BOUNDING_BOX);
            MOVE_SPEED = moveSpeed;
            _knockBackTimer = new Timer();
            _knockBackCooldownTimer = new Timer();
            MAX_HEALTH = maxHealth;
            _health = MAX_HEALTH;
            _weight = weight;
        }

        public Collider GetCollider()
        {
            return _collider;
        }

        public void HandleTileCollisions(List<Collider> others)
        {
            for (int i = 0; i < others.Count; i++)
            {
               Side collisionSide = _collider.GetCollisionSide(others.ElementAt(i));

                if (collisionSide != Side.NONE)
                {
                    switch (collisionSide)
                    {

                        case Side.BOTTOM:
                            Y = others.ElementAt(i).BoundingBox.Top - BOUNDING_BOX.Bottom - 1;
                            _dy = 0;
                            break;
                        case Side.TOP:
                            Y = others.ElementAt(i).BoundingBox.Bottom - BOUNDING_BOX.Top + 1;
                            _dy = 0;
                            break;
                        case Side.LEFT:
                            X = others.ElementAt(i).BoundingBox.Right - BOUNDING_BOX.Left + 1;
                            _dx = 0;
                            break;
                        case Side.RIGHT:
                            X = others.ElementAt(i).BoundingBox.Left - BOUNDING_BOX.Right - 1;
                            _dx = 0;
                            break;

                    }

                }

            }
        }

        public virtual void DealDamage(int amount, float power, Side side)
        {
            if (_knockBackTimer.Done && _knockBackCooldownTimer.Done) // Cannot be knocked back when already knocked back. Invulnerable during this time.
            {
                _health -= amount;
                _knockBackTimer.WaitTime(0.2f);
                _knockBackCooldownTimer.WaitTime(0.35f);
                _isKnockback = true;
                power = (power / 100) * _weight; // Allows for power to be parsed in whole numbers.

                switch (side)
                {
                    case Side.BOTTOM:
                        _dx = 0;
                        _dy = power;
                        break;
                    case Side.TOP:
                        _dx = 0;
                        _dy = -power;
                        break;
                    case Side.LEFT:
                        _dy = 0;
                        _dx = -power;
                        break;
                    case Side.RIGHT:
                        _dy = 0;
                        _dx = power;
                        break;
                }
            }

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime); // Call AnimatedSprite's Update function to animate

            // Clamp the entities health so that it cannot go above max health or below zero.
            if (_health < 0) _health = 0;
            if (_health > MAX_HEALTH) _health = MAX_HEALTH;

            // Normalise the entities movement, this is so that if the entity is moving diagonally they are not faster than normal movement.
            // Do not do this in knockback. As this neuters the effect of the knockback
            if (!_isKnockback)
            {
                Vector2 normal = new Vector2(_dx, _dy);
                normal = Vector2.Normalize(normal);
                _dx = normal.X;
                _dy = normal.Y;
                if (float.IsNaN(_dx)) // When 0, it may not return a value so the value is set to 0 directly.
                {
                    _dx = 0;
                }
                if (float.IsNaN(_dy))
                {
                    _dy = 0;
                }

                // Move the entity by _dy and _dx. Intentional movement so the move speed constant is used.
                X += (float)(_dx * MOVE_SPEED * gameTime.ElapsedGameTime.TotalMilliseconds);
                Y += (float)(_dy * MOVE_SPEED * gameTime.ElapsedGameTime.TotalMilliseconds);

            }
            else
            {
                // Move the entity by _dy and _dx. Unintentional movement so no constant is needed.
                // If this was not here, it would mean knockback would be subject to move speed rather than power.
                X += (float)(_dx * gameTime.ElapsedGameTime.TotalMilliseconds);
                Y += (float)(_dy * gameTime.ElapsedGameTime.TotalMilliseconds);

            }

            // Update the position of the collider with the entites movement.
            _collider.BoundingBox = new Rectangle((int)X + BOUNDING_BOX.Left, (int)Y + BOUNDING_BOX.Top, BOUNDING_BOX.Width, BOUNDING_BOX.Height);

            // Update the knockback timer. Set the velocites back to zero when finished. This does not matter for moving normally as the entity has already been translated above.
            _knockBackTimer.Update(gameTime);
            if (_knockBackTimer.Done)
            {
                _isKnockback = false;
            }
            _knockBackCooldownTimer.Update(gameTime);

        }

        public virtual void Update(GameTime gameTime, Vector2 playerPos)
        {
            Update(gameTime);
        }

        public virtual void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, bool debug)
        {
            base.Draw(spriteBatch, (int)X, (int)Y);
            if(debug)
                _collider.DrawDebug(spriteBatch, graphicsDevice, Color.Red);
        }

    }
}
