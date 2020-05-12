using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Asteroids
{
    class Asteroid
    {
        enum Direction
        {
            None,
            Clockwise,
            Anticlockwise
        }

        private Vector2 position;
        public Vector2 flyDirection;
        private int angle;
        private float rotation;
        private float rotationSpeed;
        private float speed;
        private int radius;
        private bool offScreen = false;
        private bool shot = false;
        private Texture2D asteroidSprite;


        // All asteroids get stored here
        public static List<Asteroid> asteroids = new List<Asteroid>();
        public static List<Asteroid> childAsteroids = new List<Asteroid>();

        public Vector2 Position
        {
            get { return position; }
        }

        public int Angle
        {
            get { return angle; }
        }

        public float Rotation
        {
            get { return rotation; }
        }

        public int Radius
        {
            get { return radius; }
        }

        public bool OffScreen
        {
            get { return offScreen; }
            set { offScreen = value; }
        }

        public bool Shot
        {
            get { return shot; }
            set { shot = value; }
        }

        public Texture2D AsteroidSprite
        {
            get { return asteroidSprite; }
        }

        public Asteroid()
        {

        }

        public Asteroid(Texture2D sprite)
        {
            // Give asteroid a sprite
            asteroidSprite = sprite;
            // Spawn an asteroid
            SpawnAsteroid();
        }

        public void Update(GameTime gameTime)
        {
            // Get delta time
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 pos = position;
            if (pos.X < -400 || pos.X > 1680 || pos.Y < -400 || pos.Y > 1120) offScreen = true;

            flyDirection = new Vector2((float)Math.Cos(MathHelper.ToRadians(angle)), -(float)Math.Sin(MathHelper.ToRadians(angle)));
            position += flyDirection * speed * dt;
            rotation += rotationSpeed * dt;
        }

        public void SpawnAsteroid()
        {
            // Spawn the new asteroid
            // Randomly choose an angle for the asteroid to fly in
            Random rand = new Random();
            angle = rand.Next(361);

            // Randomly choose a rotation value for the sprite
            rotationSpeed = (float)rand.NextDouble();
            // Change direction of rotation on a 50% chance
            if (rand.Next(101) < 50) rotationSpeed *= -1;

            // Set "specs" of asteroid depending on type of asteroid we spawn (hard coded for now)
            switch (asteroidSprite.Name)
            {
                case "Enemies/AsteroidXS":
                    radius = 1;
                    speed = rand.Next(200, 400);
                    break;
                case "Enemies/AsteroidS":
                    radius = 5;
                    speed = rand.Next(150, 350);
                    break;
                case "Enemies/AsteroidM":
                    radius = 12;
                    speed = rand.Next(100, 300);
                    break;
                case "Enemies/AsteroidL":
                    radius = 45;
                    speed = 100f;
                    break;
                default:
                    break;
            }

            Vector2 newPos;
            newPos.Y = rand.Next(-200, 920);
            if (newPos.Y < (0 - radius) || newPos.Y > (720 + radius))
            {
                // Spawning above or below the screen
                if (angle <= 180) newPos.X = rand.Next(-100, 640 - radius);
                else newPos.X = rand.Next(640 - radius, 1480);
            }
            else
            {
                // We're spawning inside the screen height so only left or right of the screen is acceptable
                if (angle <= 180)
                {
                    newPos.X = rand.Next(-200, 0 - radius);
                }
                else
                {
                    newPos.X = rand.Next(1280 + radius, 1480);
                }
            }

            // Set position
            position = newPos;
        }

        public void SplitAsteroid()
        {
            // Asteroid has been marked for deletion so spawn smaller versions to fly off in random direction! (Don't split XS sprites)
            Random rand = new Random();
            switch (asteroidSprite.Name)
            {
                case "Enemies/AsteroidS":
                    // Split into 4
                    Asteroid[] xschildAsteroids = new Asteroid[4];
                    for (int i = 0; i < 4; i++)
                    {
                        Asteroid xstempAsteroid = new Asteroid();
                        xstempAsteroid.radius = 1;
                        xstempAsteroid.speed = rand.Next(200, 400);
                        xstempAsteroid.angle = rand.Next(361);
                        xstempAsteroid.position = this.position;
                        xstempAsteroid.offScreen = false;
                        xstempAsteroid.asteroidSprite = AsteroidsGame.asteroidSprites[0];
                        xschildAsteroids[i] = xstempAsteroid;
                    }
                    Asteroid.childAsteroids.AddRange(xschildAsteroids);
                    break;
                case "Enemies/AsteroidM":
                    // Split into 3
                    Asteroid[] schildAsteroids = new Asteroid[3];
                    for (int i = 0; i < 3; i++)
                    {
                        Asteroid stempAsteroid = new Asteroid();
                        stempAsteroid.radius = 5;
                        stempAsteroid.speed = rand.Next(150, 350);
                        stempAsteroid.angle = rand.Next(361);
                        stempAsteroid.position = this.position;
                        stempAsteroid.offScreen = false;
                        stempAsteroid.asteroidSprite = AsteroidsGame.asteroidSprites[1];
                        schildAsteroids[i] = stempAsteroid;
                    }
                    Asteroid.childAsteroids.AddRange(schildAsteroids);
                    break;
                case "Enemies/AsteroidL":
                    // Split into 2
                    Asteroid[] mchildAsteroids = new Asteroid[2];
                    for (int i = 0; i < 2; i++)
                    {
                        Asteroid mtempAsteroid = new Asteroid();
                        mtempAsteroid.radius = 12;
                        mtempAsteroid.speed = rand.Next(100, 300);
                        mtempAsteroid.angle = rand.Next(361);
                        mtempAsteroid.position = this.position;
                        mtempAsteroid.offScreen = false;
                        mtempAsteroid.asteroidSprite = AsteroidsGame.asteroidSprites[2];
                        mchildAsteroids[i] = mtempAsteroid;
                    }
                    Asteroid.childAsteroids.AddRange(mchildAsteroids);
                    break;
                default:
                    break;
            }

        }
    }
}
