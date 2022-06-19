using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace computing_project
{

    // Used to denote the direction of an object.
    enum Facing
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    class Player : Entity
    {

        private Facing _facing; // The players current facing direction.
        private bool _isAttacking = false; // Whether or not the player is attacking.
        private bool _canAttack = true; // Whether or not the player can attack.

        private Timer _attackTimer; // Timer for how long an attack lasts for.
        private Timer _attackCooldownTimer; // Timer for the amount of time, since the start of the attack, until the player can attack again.
        private Collider _attackCollider; // The collider that will activate for the duration of the attack animation in the specified direction.

        private bool _key = false; // Whether the player is in possession of a key or not.

        public Player(int x, int y) : base(x, y, "linkspritemono", 24, 72, 24, 25, 65, PlayerConstants.MOVE_SPEED, PlayerConstants.MAX_HEALTH, new Rectangle(PlayerConstants.BOUNDING_BOX_TOP_X, PlayerConstants.BOUNDING_BOX_TOP_Y, PlayerConstants.BOUNDING_BOX_TOP_WIDTH, PlayerConstants.BOUNDING_BOX_TOP_HEIGHT), 1f)
        {
            _facing = Facing.DOWN;
            SetupAnimations();
            _attackTimer = new Timer();
            _attackCooldownTimer = new Timer();
            _attackCollider = new Collider(new Rectangle(1, 1, 1, 1))
            {
                Active = false
            };
        }

        public override void Load(Game game)
        {
            AudioHandler.AddSoundEffect("hit", game);
            AudioHandler.AddSoundEffect("getkey", game);
            AudioHandler.AddSoundEffect("dooropen2", game);
            AudioHandler.AddSoundEffect("punch", game);
            base.Load(game);
        }

        // Get the current position of the attack collider if it is active, else nullify.
        public Collider GetAttackCollider()
        {
            if (_attackCollider.Active)
            {
                return _attackCollider;
            }
            return null;
        }

        // Get the direction of the players attack
        public Side GetAttackSide()
        {
            if (_attackCollider.Active)
            {
                switch (_facing)
                {
                    case Facing.UP:
                        return Side.TOP;
                    case Facing.DOWN:
                        return Side.BOTTOM;
                    case Facing.LEFT:
                        return Side.LEFT;
                    case Facing.RIGHT:
                        return Side.RIGHT;
                }
            }
            return Side.NONE;
        }

        public override void DealDamage(int amount, float power, Side side)
        {
            if (!Dead)
            {
                switch (side)
                {
                    case Side.BOTTOM:
                        PlayAnimation("DamageUp", true);
                        _facing = Facing.UP;
                        break;
                    case Side.TOP:
                        PlayAnimation("DamageDown", true);
                        _facing = Facing.DOWN;
                        break;
                    case Side.LEFT:
                        PlayAnimation("DamageRight", true);
                        _facing = Facing.RIGHT;
                        break;
                    case Side.RIGHT:
                        PlayAnimation("DamageLeft", true);
                        _facing = Facing.LEFT;
                        break;
                }

                AudioHandler.PlaySoundEffect("hit");
                base.DealDamage(amount, power, side);
            }

        }

        void SetupAnimations()
        {
            // Run animations
            AddAnimation(8, 0, 0, "RunLeft", 24, 25);
            AddAnimation(8, 0, 25, "RunRight", 24, 25);
            AddAnimation(8, 0, 50, "RunUp", 24, 25);
            AddAnimation(8, 0, 75, "RunDown", 24, 25);
            // Idle animations (single frame)
            AddAnimation(1, 96, 0, "IdleLeft", 24, 25);
            AddAnimation(1, 96, 25, "IdleRight", 24, 25);
            AddAnimation(1, 120, 100, "IdleUp", 24, 25);
            AddAnimation(1, 96, 100, "IdleDown", 24, 25);
            // Attack animations (single frame)
            AddAnimation(1, 0, 100, "AttackLeft", 24, 25);
            AddAnimation(1, 24, 100, "AttackRight", 24, 25);
            AddAnimation(1, 48, 100, "AttackUp", 24, 25);
            AddAnimation(1, 72, 100, "AttackDown", 24, 25);
            // Death animation (single frame)
            AddAnimation(1, 144, 100, "Death", 24, 25);
            // Take damage animations (single frame)
            AddAnimation(1, 0, 125, "DamageLeft", 24, 25);
            AddAnimation(1, 24, 125, "DamageRight", 24, 25);
            AddAnimation(1, 48, 125, "DamageUp", 24, 25);
            AddAnimation(1, 72, 125, "DamageDown", 24, 25);
        }

        public void MoveLeft(bool diag)
        {
            _dx = -MOVE_SPEED;
            if (!diag)
            {
                PlayAnimation("RunLeft", true);
                _facing = Facing.LEFT;
            }
        }

        public void MoveRight(bool diag)
        {
            _dx = MOVE_SPEED;
            if (!diag)
            {
                PlayAnimation("RunRight", true);
                _facing = Facing.RIGHT;
            }
        }

        public void MoveUp()
        {
            _dy = -MOVE_SPEED;
            PlayAnimation("RunUp", true);
            _facing = Facing.UP;
        }

        public void MoveDown()
        {
            _dy = MOVE_SPEED;
            PlayAnimation("RunDown", true);
            _facing = Facing.DOWN;
        }

        public void Attack()
        {

            // Cannot move intentionally whilst attacking.
            _isAttacking = true;

            // Cannot attack during or directly after an attack.
            _canAttack = false;

            // Play correct attack animation for direction.
            switch (_facing)
            {
                case Facing.LEFT:
                    PlayAnimation("AttackLeft", true);
                    _attackCollider.BoundingBox = new Rectangle((int)X + PlayerConstants.ATTACK_BOX_LEFT_X, (int)Y + PlayerConstants.ATTACK_BOX_LEFT_Y, PlayerConstants.ATTACK_BOX_LEFT_WIDTH, PlayerConstants.ATTACK_BOX_LEFT_HEIGHT);
                    break;
                case Facing.RIGHT:
                    PlayAnimation("AttackRight", true);
                    _attackCollider.BoundingBox = new Rectangle((int)X + PlayerConstants.ATTACK_BOX_RIGHT_X, (int)Y + PlayerConstants.ATTACK_BOX_RIGHT_Y, PlayerConstants.ATTACK_BOX_RIGHT_WIDTH, PlayerConstants.ATTACK_BOX_RIGHT_HEIGHT);
                    break;
                case Facing.UP:
                    PlayAnimation("AttackUp", true);
                    _attackCollider.BoundingBox = new Rectangle((int)X + PlayerConstants.ATTACK_BOX_TOP_X, (int)Y + PlayerConstants.ATTACK_BOX_TOP_Y, PlayerConstants.ATTACK_BOX_TOP_WIDTH, PlayerConstants.ATTACK_BOX_TOP_HEIGHT);
                    break;
                case Facing.DOWN:
                    PlayAnimation("AttackDown", true );
                    _attackCollider.BoundingBox = new Rectangle((int)X + PlayerConstants.ATTACK_BOX_BOTTOM_X, (int)Y + PlayerConstants.ATTACK_BOX_BOTTOM_Y, PlayerConstants.ATTACK_BOX_BOTTOM_WIDTH, PlayerConstants.ATTACK_BOX_BOTTOM_HEIGHT);
                    break;
            }

            // Enable the attack collider.
            _attackCollider.Active = true;

            // Play the attack sound
            AudioHandler.PlaySoundEffect("punch");

            // Start the timers, ensures a delay between when the attack finishes and when the player can attack again.
            _attackTimer.WaitTime(PlayerConstants.ATTACK_DURATION);
            _attackCooldownTimer.WaitTime(PlayerConstants.ATTACK_COOLDOWN);

        }

        // Stop any player movement, when bool verical is true it stops movement in the y axis only etc.
        public void StopMoving(bool vertical, bool animate)
        {
            if (!vertical)
            {
                _dx = 0.0f;
                if (animate)
                {
                    if (_facing == Facing.RIGHT)
                    {
                        PlayAnimation("IdleRight", true);
                    }
                    else if (_facing == Facing.LEFT)
                    {
                        PlayAnimation("IdleLeft", true);
                    }
                }
            }
            if (vertical)
            {
                _dy = 0.0f;
                if (animate)
                {
                    if (_facing == Facing.UP)
                    {
                        PlayAnimation("IdleUp", true);
                    }
                    else if (_facing == Facing.DOWN)
                    {
                        PlayAnimation("IdleDown", true);
                    }
                }
            }
        }

        // Move the player instantaniously to the specified point.
        public void Spawn(Vector2 spawnPoint)
        {
            X = (int)spawnPoint.X;
            Y = (int)spawnPoint.Y;
        }

        public void GetKey()
        {
            _key = true;
            AudioHandler.PlaySoundEffect("getkey");
        }

        public void UseKey()
        {
            _key = false;
            AudioHandler.PlaySoundEffect("dooropen2");
        }

        private void HandleInput()
        {

            if (!_isAttacking && !Dead)
            {

                // Move Up / Down
                if (InputHandler.IsKeyPressed(InputConstants.UP, false) || InputHandler.IsKeyPressed(InputConstants.UP_ALT, false) || InputHandler.IsButtonPressed(InputConstants.GAMEPAD_UP, false))
                    MoveUp();
                if (InputHandler.IsKeyPressed(InputConstants.DOWN, false) || InputHandler.IsKeyPressed(InputConstants.DOWN_ALT, false) || InputHandler.IsButtonPressed(InputConstants.GAMEPAD_DOWN, false))
                    MoveDown();

                // Move left
                if (InputHandler.IsKeyPressed(InputConstants.LEFT, false) || InputHandler.IsKeyPressed(InputConstants.LEFT_ALT, false) || InputHandler.IsButtonPressed(InputConstants.GAMEPAD_LEFT, false))
                    if (InputHandler.IsKeyPressed(InputConstants.DOWN, false) || InputHandler.IsKeyPressed(InputConstants.DOWN_ALT, false) || InputHandler.IsKeyPressed(InputConstants.UP, false) || InputHandler.IsKeyPressed(InputConstants.UP_ALT, false) || InputHandler.IsButtonPressed(InputConstants.GAMEPAD_DOWN, false) || InputHandler.IsButtonPressed(InputConstants.GAMEPAD_UP, false))
                        MoveLeft(true);
                    else
                        MoveLeft(false);

                // Move right
                if (InputHandler.IsKeyPressed(InputConstants.RIGHT, false) || InputHandler.IsKeyPressed(InputConstants.RIGHT_ALT, false) || InputHandler.IsButtonPressed(InputConstants.GAMEPAD_RIGHT, false))
                    if (InputHandler.IsKeyPressed(InputConstants.DOWN, false) || InputHandler.IsKeyPressed(InputConstants.DOWN_ALT, false) || InputHandler.IsKeyPressed(InputConstants.UP, false) || InputHandler.IsKeyPressed(InputConstants.UP_ALT, false) || InputHandler.IsButtonPressed(InputConstants.GAMEPAD_DOWN, false) || InputHandler.IsButtonPressed(InputConstants.GAMEPAD_UP, false))
                        MoveRight(true);
                    else
                        MoveRight(false);

                // Stop moving
                if ((InputHandler.IsKeyUp(InputConstants.UP) && InputHandler.IsKeyUp(InputConstants.DOWN)) && InputHandler.IsKeyUp(InputConstants.UP_ALT) && InputHandler.IsKeyUp(InputConstants.DOWN_ALT) && InputHandler.IsButtonUp(InputConstants.GAMEPAD_DOWN) && InputHandler.IsButtonUp(InputConstants.GAMEPAD_UP))
                    StopMoving(true, true);
                if ((InputHandler.IsKeyUp(InputConstants.LEFT) && InputHandler.IsKeyUp(InputConstants.RIGHT)) && InputHandler.IsKeyUp(InputConstants.LEFT_ALT) && InputHandler.IsKeyUp(InputConstants.RIGHT_ALT) && InputHandler.IsButtonUp(InputConstants.GAMEPAD_LEFT) && InputHandler.IsButtonUp(InputConstants.GAMEPAD_RIGHT))
                    StopMoving(false, true);

                // Attack
                if ((InputHandler.IsKeyPressed(InputConstants.ATTACK, true) || InputHandler.IsKeyPressed(InputConstants.ATTACK_ALT, true) || InputHandler.IsButtonPressed(InputConstants.GAMEPAD_ATTACK, true)) && _canAttack)
                    Attack();
            }
            else
            {
                StopMoving(true, false);
                StopMoving(false, false);
            }

        }

        public override void Update(GameTime gameTime)
        {

            // Player input
            if (!_isKnockback)
                HandleInput();

            // Clamp the players health so it stays within bounds.
            _health = Math.Min(Math.Max(_health, 0), 6);
            UI.Health = _health;
            UI.Key = _key;

            // Update the timer, when it finishes - the player can move again.
            _attackTimer.Update(gameTime);
            if (_attackTimer.Done)
            {
                _isAttacking = false;
                _attackCollider.Active = false;
            }

            // When this timer finishes, the player can attack again.
            _attackCooldownTimer.Update(gameTime);
            if (_attackCooldownTimer.Done)
                _canAttack = true;

            // The death animation will play when the players health get to zero.
            if (_health == 0)
            {
                Dead = true;
                PlayAnimation("Death", true);
            }

            // The game will pause upon the death of the player.
            if (_knockBackCooldownTimer.Done && Dead)
            {
                UI.Died = true;
            }

            base.Update(gameTime);

        }

        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, bool debug)
        {

            if (debug)
            {
                _attackCollider.DrawDebug(spriteBatch, graphicsDevice, Color.Orange);
            }

            base.Draw(spriteBatch, graphicsDevice, debug);

        }

    }
}
