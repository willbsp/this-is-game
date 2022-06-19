using System;
using Microsoft.Xna.Framework;

namespace computing_project
{
    class BossEnemy : Entity
    {

        private bool _dying = false;
        private bool _seen = false; // Seen the player.
        private Timer _deathTimer; // Times the time between when the player kills the boss and when its death animation finishes.
        private Timer _entryTimer; // Timer for the enterance animation.

        public BossEnemy(int x, int y) : base(x, y, "boss", 0, 0, 32, 34, 500, 0.02f, 10, new Rectangle(2, 8, 28, 21), 0.7f)
        {
            SetupAnimations();
            PlayAnimation("Waiting", false);
            _deathTimer = new Timer();
            _entryTimer = new Timer();
        }

        private void SetupAnimations()
        {
            AddAnimation(4, 0, 0, "Idle", 32, 34);
            AddAnimation(3, 0, 34, "Death", 32, 34);
            // A blank animation for when the player enters the room.
            AddAnimation(1, 128, 0, "Waiting", 32, 34);
            // The enterance animation when the player approaches.
            AddAnimation(5, 0, 68, "Grow", 32, 34);
        }

        public override void DealDamage(int amount, float power, Side side)
        {

            if (_knockBackTimer.Done && _knockBackCooldownTimer.Done) // Cannot be knocked back when already knocked back. Invulnerable during this time.
            {

                AudioHandler.PlaySoundEffect("takedamage");

            }

            base.DealDamage(amount, power, side);
        }

        // For loading sounds...
        public override void Load(Game game)
        {
            // Stop the currently playing song when the player enters the room with the boss.
            AudioHandler.StopSong();

            AudioHandler.LoadSong("bossmusic", game);
            AudioHandler.AddSoundEffect("takedamage", game);
            AudioHandler.AddSoundEffect("bossdead", game);
            base.Load(game);
        }

        public override void Update(GameTime gameTime, Vector2 playerPos)
        {

            // If the player has been seen and the entry animation has finished.
            if (_seen && _entryTimer.Done)
            {
                if (!_isKnockback && !_dying)
                {

                    PlayAnimation("Idle", true);

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
                else if (!_isKnockback && _dying)
                {
                    _dy = 0; _dx = 0;
                }
            }
            else if (!_seen)
            {

                // Find the distance from the enemy to the player using pythagoras theorum.
                double distanceX = Math.Abs(Math.Abs(X) - Math.Abs(playerPos.X));
                double distanceY = Math.Abs(Math.Abs(Y) - Math.Abs(playerPos.Y));
                int distanceFromPlayer = (int)Math.Sqrt((distanceX * distanceX) + (distanceY * distanceY));
                if (distanceFromPlayer < 70)
                {

                    // Start the timer for the grow animation.
                    _entryTimer.WaitTime(2f);
                    // Seen the player.
                    _seen = true;
                    // Play the boss music
                    AudioHandler.PlaySong(true);
                    PlayAnimation("Grow", false);

                }

            }

            if (_health == 0)
            {
                if (!_dying)
                {
                    _dying = true;
                    PlayAnimation("Death", false);
                    AudioHandler.PlaySoundEffect("bossdead");
                    _deathTimer.WaitTime(1.4f);
                }
                else if (_deathTimer.Done)
                {
                    // Signal to show the win screen in the UI.
                    UI.Won = true;
                    // Remove from the level.
                    Dead = true;
                }
            }

            _deathTimer.Update(gameTime);
            _entryTimer.Update(gameTime);
            base.Update(gameTime);
        }

    }
}
