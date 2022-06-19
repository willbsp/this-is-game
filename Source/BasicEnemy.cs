using System;
using Microsoft.Xna.Framework;

namespace computing_project
{
    class BasicEnemy : Entity
    {

        private static Random _r = new Random();
        private bool _seen = false;
        private bool _dying = false;
        private Timer _deathSoundTimer;

        public BasicEnemy(int x, int y) : base(x, y, "enemy", 0, 0, 16, 17, 500, (float)_r.Next(20, 40) / 1000, 2, new Rectangle(3, 9, 10, 7), 1f) // (float)r.Next(2, 7) / 100
        {
            SetupAnimations();
            PlayAnimation("Idle", true);
            _deathSoundTimer = new Timer();
        }

        private void SetupAnimations()
        {
            AddAnimation(3, 0, 0, "Idle", 16, 17);
            AddAnimation(1, 0, 17, "Death", 16, 17);
        }

        public override void Load(Game game)
        {
            AudioHandler.AddSoundEffect("basicenemydeath", game);
            AudioHandler.AddSoundEffect("takedamage", game);
            base.Load(game);
        }

        public override void DealDamage(int amount, float power, Side side)
        {

            if (_knockBackTimer.Done && _knockBackCooldownTimer.Done) // Cannot be knocked back when already knocked back. Invulnerable during this time.
            {

                AudioHandler.PlaySoundEffect("takedamage");

            }

            base.DealDamage(amount, power, side);
        }

        public override void Update(GameTime gameTime, Vector2 playerPos)
        {
        
            if (!_isKnockback)
            {

                if (_seen)
                {

                    // Move towards the players x coord.
                    if (playerPos.X > X)
                    {
                        _dx = 1;
                    }
                    else if (Math.Abs((int)X - (int)playerPos.X) <= 1)
                    {
                        _dx = 0;
                    }
                    else
                    {
                        _dx = -1;
                    }

                    // Move towards the players y coord.
                    if (playerPos.Y > Y)
                    {
                        _dy = 1;
                    }
                    else if (Math.Abs((int)Y - (int)playerPos.Y) <= 1)
                    {
                        _dy = 0;
                    }
                    else
                    {
                        _dy = -1;
                    }

                }
                else
                {

                    // Find the distance from the enemy to the player using pythagoras theorum.
                    double distanceX = Math.Abs(Math.Abs(X) - Math.Abs(playerPos.X));
                    double distanceY = Math.Abs(Math.Abs(Y) - Math.Abs(playerPos.Y));
                    int distanceFromPlayer = (int)Math.Sqrt((distanceX * distanceX) + (distanceY * distanceY));
                    if (distanceFromPlayer < 70)
                        _seen = true;

                }

            }

            

            if (_health == 0)
            {
                PlayAnimation("Death", false);
                if (!_dying)
                {
                    _dying = true;
                    AudioHandler.PlaySoundEffect("basicenemydeath");
                }
                if (_knockBackCooldownTimer.Done)
                {
                    Dead = true;
                }
            }

            _deathSoundTimer.Update(gameTime);

            base.Update(gameTime);
        }

    }
}
