using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Asteroids
{

    public static class GameSounds
    {
        public static SoundEffect bulletFire;
        public static SoundEffect shipBoom;
        public static SoundEffect asteroidBlip;
        public static SoundEffect gameOver;
    }

    public class AsteroidsGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Texture2D spaceSprite;
        private Texture2D shipSprite;
        private Texture2D thrustSprite;
        private Texture2D bulletSprite;
        private Texture2D asteroidXSSprite;
        private Texture2D asteroidSSprite;
        private Texture2D asteroidMSprite;
        private Texture2D asteroidLSprite;
        public static Texture2D[] asteroidSprites;
        private Texture2D heartSprite;

        private SpriteFont gameFont;

        private Ship player;
        private GameController controller = new GameController();

        public AsteroidsGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            // Set window size
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            // Have the mouse cursor on for now because it annoys me
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load background sprite
            spaceSprite = Content.Load<Texture2D>("Misc/Background");
            // Load player sprite
            shipSprite = Content.Load<Texture2D>("Player/Ship");
            thrustSprite = Content.Load<Texture2D>("Player/Fire");
            // Load bullet sprite
            bulletSprite = Content.Load<Texture2D>("Player/Bullet");
            // Load asteroid sprite
            asteroidXSSprite = Content.Load<Texture2D>("Enemies/AsteroidXS");
            asteroidSSprite = Content.Load<Texture2D>("Enemies/AsteroidS");
            asteroidMSprite = Content.Load<Texture2D>("Enemies/AsteroidM");
            asteroidLSprite = Content.Load<Texture2D>("Enemies/AsteroidL");
            asteroidSprites = new Texture2D[4] { asteroidXSSprite, asteroidSSprite, asteroidMSprite, asteroidLSprite };
            // Load heart sprite
            heartSprite = Content.Load<Texture2D>("Misc/Heart");

            // Load font
            gameFont = Content.Load<SpriteFont>("Fonts/gameFont");

            // Load sound
            GameSounds.bulletFire = Content.Load<SoundEffect>("Sounds/shoot");
            GameSounds.shipBoom = Content.Load<SoundEffect>("Sounds/boom");
            GameSounds.asteroidBlip = Content.Load<SoundEffect>("Sounds/blip");
            GameSounds.gameOver = Content.Load<SoundEffect>("Sounds/lose");

            // Load in the player
            player = new Ship(new Vector2((graphics.PreferredBackBufferWidth / 2) - shipSprite.Width / 2, (graphics.PreferredBackBufferHeight / 2) - shipSprite.Height / 2));
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (!controller.InGame && Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                controller.InGame = true;
                controller.GameOver = false;
                controller.Score = 0;
                controller.GameTimer = 0D;
                player.Health = 3;
                player.ResetState();
                Asteroid.asteroids.Clear();
                Asteroid.childAsteroids.Clear();
            }

            if (controller.InGame)
            {
                // Update the ship if still alive!
                if (player.Health > 0)
                {
                    // Update the player, movements, shots, etc.
                    player.Update(gameTime, graphics);

                    // Handle the game spawns etc
                    controller.Update(gameTime);

                    // Update asteroids, check for collisions and check for out of bound
                    foreach (Asteroid asteroid in Asteroid.asteroids)
                    {
                        // Update asteroid
                        asteroid.Update(gameTime);
                        // Check for collisions by adding the two radii and check if two positions are less than that
                        int sum = asteroid.Radius + Ship.Radius;
                        if (Vector2.Distance(asteroid.Position, player.Position) < sum)
                        {
                            GameSounds.shipBoom.Play();
                            // Reduce player health
                            player.Health--;
                            // Reset the ship stats
                            player.ResetState();
                            // Clear all asteroids on screen if needed
                            if (player.Health > 0) Asteroid.asteroids.Clear();
                            // Reset the player position
                            player.Position = new Vector2((graphics.PreferredBackBufferWidth / 2) - shipSprite.Width / 2, (graphics.PreferredBackBufferHeight / 2) - shipSprite.Height / 2);
                            player.ThrustPosition = player.Position;
                            // Destroy any existing projectiles
                            Bullet.bullets.Clear();
                            // Break out of foreach so we don't iterate further
                            break;
                        }

                        // Update the bullets and remove if collided or off screen
                        foreach (Bullet bullet in Bullet.bullets)
                        {
                            bullet.Update(gameTime);
                            // Check if any are off the screen
                            Vector2 pos = bullet.Position;
                            if (pos.X < -50 || pos.X > 1330 || pos.Y < -50 || pos.Y > 750) bullet.OffScreen = true;

                            // Check if bullet hit asteroid
                            int projSum = asteroid.Radius + bullet.Radius;
                            if (Vector2.Distance(asteroid.Position, bullet.Position) < projSum)
                            {
                                GameSounds.asteroidBlip.Play();
                                // Increase score depending on asteroid size
                                switch (asteroid.AsteroidSprite.Name)
                                {
                                    case "Enemies/AsteroidXS":
                                        controller.Score += 50;
                                        break;
                                    case "Enemies/AsteroidS":
                                        controller.Score += 30;
                                        break;
                                    case "Enemies/AsteroidM":
                                        controller.Score += 20;
                                        break;
                                    case "Enemies/AsteroidL":
                                        controller.Score += 10;
                                        break;
                                    default:
                                        break;
                                }
                                asteroid.SplitAsteroid();
                                asteroid.Shot = true;
                                bullet.Collided = true;
                            }
                        }
                    }
                    Asteroid.asteroids.AddRange(Asteroid.childAsteroids);
                    Asteroid.childAsteroids.Clear();
                    Asteroid.asteroids.RemoveAll(a => a.OffScreen);
                    Asteroid.asteroids.RemoveAll(a => a.Shot);
                    Bullet.bullets.RemoveAll(a => a.OffScreen);
                    Bullet.bullets.RemoveAll(a => a.Collided);
                }
                else
                {
                    GameSounds.gameOver.Play();
                    controller.InGame = false;
                    controller.GameOver = true;
                }

                base.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            spriteBatch.Begin();

            // Draw background
            DrawBackground();
            if (!controller.InGame && !controller.GameOver)
            {
                // Draw main menu
                DrawMainmenu();
            }

            if (controller.InGame && !controller.GameOver)
            {
                //Draw the in-game stuff!
                // Draw UI
                DrawUI();
                // Draw the player
                DrawShip();
                // Draw asteroids
                DrawAsteroids();
                // Draw bullets
                DrawBullets();
            }
            
            if (!controller.InGame && controller.GameOver)
            {
                // Draw asteroids (I think it looks nice with the asteroids on the game over screen)
                DrawAsteroids();
                // Draw game over screen
                DrawGameOver();
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawMainmenu()
        {
            string mainText = "ASTEROIDS";
            Vector2 mainTextSize = gameFont.MeasureString(mainText);
            string instruction = "Press enter to begin!";
            Vector2 instructTextSize = gameFont.MeasureString(instruction);
            spriteBatch.DrawString(gameFont, mainText, new Vector2((graphics.PreferredBackBufferWidth / 2) - (mainTextSize.X / 2), 200), Color.White);
            spriteBatch.DrawString(gameFont, instruction, new Vector2((graphics.PreferredBackBufferWidth / 2) - (instructTextSize.X / 2), 600), Color.White);
        }

        private void DrawGameOver()
        {
            string overText = "GAME OVER!";
            Vector2 overTextSize = gameFont.MeasureString(overText);
            string scoreText = "Your score was " + controller.Score + " - Press enter to try again!";
            Vector2 scoreTextSize = gameFont.MeasureString(scoreText);
            spriteBatch.DrawString(gameFont, overText, new Vector2((graphics.PreferredBackBufferWidth / 2) - (overTextSize.X / 2), 200), Color.White);
            spriteBatch.DrawString(gameFont, scoreText, new Vector2((graphics.PreferredBackBufferWidth / 2) - (scoreTextSize.X / 2), 600), Color.White);
        }

        private void DrawBackground()
        {
            spriteBatch.Draw(spaceSprite, new Vector2(0, 0), Color.White);
        }

        private void DrawUI()
        {
            // Draw lives
            Rectangle sourceRectangle = new Rectangle(0, 0, heartSprite.Width, heartSprite.Height);
            Vector2 origin = new Vector2(0, 0);
            for (int i = 0; i < player.Health; i++)
            {
                spriteBatch.Draw(heartSprite, new Vector2(3 + (i * 40), 3), sourceRectangle, Color.White, 0.0f, origin, 0.5f, SpriteEffects.None, 1);
            }

            // Draw timer
            spriteBatch.DrawString(gameFont, "Time: " + Math.Floor(controller.GameTimer), new Vector2(3, 650), Color.White);

            // Draw the score
            spriteBatch.DrawString(gameFont, "Score: " + controller.Score, new Vector2(3, 680), Color.White);
        }

        private void DrawShip()
        {
            // Set origin
            Vector2 thrustOrigin = new Vector2(3, 0 - shipSprite.Height / 2);
            Rectangle thrustRectangle = new Rectangle(0, 0, thrustSprite.Width, thrustSprite.Height);
            if (player.Thrusting)
            {
                // Draw the thrust
                spriteBatch.Draw(thrustSprite, player.ThrustPosition, thrustRectangle, Color.White, player.Rotation, thrustOrigin, 1.0f, SpriteEffects.None, 1);
            }
            // Draw the ship
            Vector2 origin = new Vector2(shipSprite.Width / 2, shipSprite.Height / 2);
            Rectangle sourceRectangle = new Rectangle(0, 0, shipSprite.Width, shipSprite.Height);
            spriteBatch.Draw(shipSprite, player.Position, sourceRectangle, Color.White, player.Rotation, origin, 1.0f, SpriteEffects.None, 1);
        }

        public void DrawAsteroids()
        {
            foreach (Asteroid ast in Asteroid.asteroids)
            {
                Rectangle sourceRectangle = new Rectangle(0, 0, ast.AsteroidSprite.Width, ast.AsteroidSprite.Height);
                // Set origin to center of the sprite
                Vector2 origin = new Vector2(ast.AsteroidSprite.Width / 2, ast.AsteroidSprite.Height / 2);
                spriteBatch.Draw(ast.AsteroidSprite, ast.Position, sourceRectangle, Color.White, ast.Rotation, origin, 1.0f, SpriteEffects.None, 1);
            }
        }

        private void DrawBullets()
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, bulletSprite.Width, bulletSprite.Height);
            // Set origin to center of the sprite
            Vector2 origin = new Vector2(bulletSprite.Width / 2, bulletSprite.Height / 2);
            foreach (Bullet b in Bullet.bullets)
            {
                // Rotation/angle of the bullet should be same as the ship
                // Draw it
                spriteBatch.Draw(bulletSprite, b.Position, sourceRectangle, Color.White, b.Rotation, origin, 1.0f, SpriteEffects.None, 1);
            }
        }
    }
}
