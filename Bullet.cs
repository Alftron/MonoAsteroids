using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Asteroids
{
    class Bullet
    {
        private Vector2 position;
        private Vector2 shootDirection;
        private float speed = 120f;
        private int radius = 15;
        private bool collided = false;
        private float rotation;
        private bool offScreen = false;

        public static List<Bullet> bullets = new List<Bullet>();

        public Vector2 Position
        {
            get { return position; }
        }

        public float Speed
        {
            get { return speed; }
        }

        public int Radius
        {
            get { return radius; }
        }

        public bool Collided
        {
            get { return collided; }
            set { collided = value; }
        }

        public float Rotation
        {
            get { return rotation; }
        }

        public bool OffScreen
        {
            get { return offScreen; }
            set { offScreen = value; }
        }

        public Bullet(Vector2 newPos, float angle)
        {
            position = newPos;
            rotation = angle;
            shootDirection = new Vector2((float)Math.Cos(MathHelper.ToRadians(90) - rotation), -(float)Math.Sin(MathHelper.ToRadians(90) - rotation));
        }

        public void Update(GameTime gametime)
        {
            // Get delta time
            float dt = (float)gametime.ElapsedGameTime.TotalSeconds;

            // Change position based on angle
            position += shootDirection * speed * dt;
        }
    }
}
