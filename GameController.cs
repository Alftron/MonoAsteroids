using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Asteroids
{
    class GameController
    {
        private double timer = 300D;
        private double maxTimer = 300D;
        private double difficultyTimer = 15D;
        private double maxDifficultyTimer = 15D;
        private double gameTimer = 0D;

        private bool inGame = false;
        private bool gameOver = false;

        private int score = 0;

        public bool InGame
        {
            get { return inGame; }
            set { inGame = value; }
        }

        public bool GameOver
        {
            get { return gameOver; }
            set { gameOver = value; }
        }

        public int Score
        {
            get { return score; }
            set { score = value; }
        }

        public double GameTimer
        {
            get { return gameTimer; }
            set { gameTimer = value; }
        }

        public void Update (GameTime gameTime)
        {
            timer -= gameTime.ElapsedGameTime.TotalMilliseconds;
            difficultyTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            gameTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (timer < 0)
            {
                // Get all the asteroid sprites
                Texture2D[] sprites = AsteroidsGame.asteroidSprites;
                // Choose a sprite at random
                Random rand = new Random();
                Texture2D spriteToUse = sprites[rand.Next(sprites.Length)];
                // Add a new asteroid
                Asteroid.asteroids.Add(new Asteroid(spriteToUse));
                // Reset the timer
                timer = maxTimer;
            }
            // Difficulty increase here
            if (difficultyTimer < 0)
            {
                // Every 30 seconds increase the spawn rate but only up to a certain amount and add to score
                if (maxTimer > 100D) maxTimer -= 25D;
                difficultyTimer = maxDifficultyTimer;
                score += 100;
            }
        }
    }
}
