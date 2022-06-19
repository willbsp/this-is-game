using System;
using Microsoft.Xna.Framework;

namespace computing_project
{
    public class Timer
    {

        private float _delay = 0; // The amount of time to wait for.
        private bool _trigger = false; // Triggers the start of the timer.
        public bool Done = true; // Is true when there is no timer, or a timer has finished.

        // Wait for a specific amount of time.
        public void WaitTime(float amountOfTime)
        {
            // Sets the delay
            _delay = amountOfTime;

            // Will start the timer in update.
            _trigger = true;
            Done = false;

        }

        public void Update(GameTime gameTime)
        {
            // Find the amount of time since the delay started.
            if (_delay > 0) _delay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            // When it is less than zero (it has finished) the timer is done.
            if (_delay <= 0 && _trigger)
            {
                _trigger = false;
                Done = true;
            }

        }

    }
}
