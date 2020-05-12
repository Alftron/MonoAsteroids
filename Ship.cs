using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    class Ship
    {
        // Rotation parameters
        private const float rotationVelocityIncrement = 0.10f; // How much to increase rotational velocity by
        private const float maxRotationVelocity = 100f; // Maximum rotational velocity

        // Acceleration parameters
        private const float accelerationIncrement = 3f; // How much to increase acceleration by
        private const float accelerationAirResistance = 1f; // How much braking/natural air resistance
        private const float maxAcceleration = 400f; // Maximum air resistance

        private static int radius = 20;

        private Vector2 position;
        private Vector2 thrustPosition;
        private Vector2 direction;
        private Vector2 thrustDirection;
        private float rotation = 0.00f; // Current rotation

        private float acceleratedRotation = 0.00f; // Rotation when last accelerated at
        private float rotationVelocity = 0.00f; // Speed of rotating in gravity
        private float acceleration = 0.00f; // Acceleration of ship
        private bool accelerating = false;
        private bool thrusting = false;
        private int health = 3;

        private KeyboardState oldKState = Keyboard.GetState();

        public static int Radius
        {
            get { return radius; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector2 ThrustPosition
        {
            get { return thrustPosition; }
            set { thrustPosition = value; }
        }

        public float Rotation
        {
            get { return rotation; }
        }

        public float Acceleration
        {
            get { return acceleration; }
        }

        public bool Thrusting
        {
            get { return thrusting; }
        }

        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        public Ship(Vector2 newPos)
        {
            position = newPos;
            thrustPosition = newPos;
        }

        public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            // Get keyboard state
            KeyboardState kState = Keyboard.GetState();

            // Get delta time
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Handle keyboard input here for now
            if (kState.IsKeyDown(Keys.Up) && kState.IsKeyUp(Keys.Left) && kState.IsKeyUp(Keys.Right))
            {
                // Up is pressed so accelerate but taking "physics" into account
                bool oppositeDir = false;
                float newRot = Math.Abs(MathHelper.ToDegrees(rotation));
                float prevRot = Math.Abs(MathHelper.ToDegrees(acceleratedRotation));
                float diff = Math.Abs(newRot - prevRot);
                if (diff >= 35f) oppositeDir = true;

                if (oppositeDir && acceleration > 0)
                {
                    acceleration /= 3;
                }
                // Increase acceleration
                if (acceleration < maxAcceleration) acceleration += accelerationIncrement;
                accelerating = true;
                thrusting = true;
                direction = new Vector2((float)Math.Cos(MathHelper.ToRadians(90) - rotation), -(float)Math.Sin(MathHelper.ToRadians(90) - rotation));
                // Set the accelerated rotation
                acceleratedRotation = rotation;
            }
            if (kState.IsKeyDown(Keys.Down))
            {
                // Down is pressed but I don't think down does anything in asteroids
            }
            if (kState.IsKeyDown(Keys.Left))
            {
                // Change rotation
                if (rotationVelocity < maxRotationVelocity) rotation -= rotationVelocityIncrement;
            }
            if (kState.IsKeyDown(Keys.Right))
            {
                // Change rotation
                if (rotationVelocity < maxRotationVelocity) rotation += rotationVelocityIncrement;
            }
            if (kState.IsKeyUp(Keys.Up)) thrusting = false;

            // Handle deceleration here
            if (!accelerating)
            {
                // We're not accelerating but could be floating still, so slow down in the air
                if (acceleration > 0) acceleration -= accelerationAirResistance;
                // Adjust acceleration in case we're going in a different direction
            }
            // Change position of ship and thrust
            position += direction * acceleration * dt;
            thrustPosition += direction * acceleration * dt;
            // Reset the accelerating flag
            accelerating = false;
            // Handle being off the screen (Should reappear on opposite screen at same X or same Y dependent on side loss)
            if (position.X > graphics.PreferredBackBufferWidth + radius)
            {
                position.X = 0 - radius;
                thrustPosition.X = position.X;
            }
            else if (position.X  < 0 - radius)
            {
                position.X = graphics.PreferredBackBufferWidth + radius;
                thrustPosition.X = position.X;
            }
            else if (position.Y > graphics.PreferredBackBufferHeight + radius)
            {
                position.Y = 0 - radius;
                thrustPosition.Y = position.Y;
            }
            else if (position.Y < 0 - radius)
            {
                position.Y = graphics.PreferredBackBufferHeight + radius;
                thrustPosition.Y = position.Y;
            }

            // Fire projectile on space but no holding and reduce amount on screen at once
            if (kState.IsKeyDown(Keys.Space) && oldKState.IsKeyUp(Keys.Space) && Bullet.bullets.Count < 5)
            {
                // Fire projectile
                GameSounds.bulletFire.Play();
                Bullet proj = new Bullet(position, rotation);
                Console.WriteLine(proj.Speed.ToString());
                Bullet.bullets.Add(proj);
                //Bullet.bullets.Add(new Bullet(position, rotation));
            }
            // Update the old keyboard state
            oldKState = kState;
        }

        public void ResetState()
        {
            // Resets the ship stats (speed, rotation, etc);
            rotation = 0.00f; // Current rotation
            acceleratedRotation = 0.00f; // Rotation when last accelerated at
            rotationVelocity = 0.00f; // Speed of rotating in gravity
            acceleration = 0.00f; // Acceleration of ship
            accelerating = false;
    }
    }
}
