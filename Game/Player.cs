﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace PlatformerRemaster
{
    public class Player
    {
        private Texture2D idleTexture;
        private IServiceProvider serviceProvider;

        private float gravity = 5000f;
        private float maxVelocity = 15000f;
        private float speed = 800f;
        private int jumpCountMax = 6;
        private int jumpCount;
        private Vector2 velocity = new Vector2(0, 0);


        static KeyboardState currentKeyState;
        static KeyboardState previousKeyState;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Vector2 position;

        public float X { get => position.X; set => position.X = value; }
        public float Y { get => position.Y; set => position.Y = value; }
        public int Width { get; } = 64;
        public int Height { get; } = 64;

        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;

        public Player(Vector2 position, IServiceProvider serviceProvider)
        {
            Position = position;
            this.serviceProvider = serviceProvider;
            jumpCount = jumpCountMax;
        }       

        public void LoadContent()
        {
            content = new ContentManager(serviceProvider, "Content");
            idleTexture = Content.Load<Texture2D>("cover-256");
        }

        public void Reset(Vector2 position)
        {
            Position = position;
        }

        public void Move(GameTime gameTime)
        {
            previousKeyState = currentKeyState;
            currentKeyState = Keyboard.GetState();

            if (currentKeyState.IsKeyDown(Keys.Space) && !previousKeyState.IsKeyDown(Keys.Space))
                Jump(gameTime);

            if (currentKeyState.IsKeyDown(Keys.S))
            {
                position.Y += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (currentKeyState.IsKeyDown(Keys.A))
            {
                position.X -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (currentKeyState.IsKeyDown(Keys.D))
            {
                position.X += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
                
        }

        public void Jump(GameTime gameTime)
        {
            if (jumpCount <= jumpCountMax)
            {
                velocity.Y = -1750f;
                jumpCount++;
            }
        }

        private void OnGround()
        {
            jumpCount = 0;
        }

        public void Collision(Platform platform)
        {
            
            if (X + Width > platform.X && X < platform.X + platform.Width)
            {
                //above
                if (Y + Height >= platform.Y && Y + Height <= platform.Y + platform.Height / 2)
                {
                    Y = platform.Y - Height;
                    velocity.Y = 0;
                    OnGround();
                }            
                //below
                if (Y <= platform.Y + platform.Height && Y >= platform.Y + platform.Height / 2)
                {
                    Y = platform.Y + platform.Height;
                    velocity.Y = 0;
                }
            }

            if (Y + Height > platform.Y && Y < platform.Y + platform.Height)
            {

                //left
                if (X + Width >= platform.X && X + Width <= platform.X + platform.Width / 2) {
                    X = platform.X - Width - 1;
                    velocity.X = 0;
                }

                //right
                if (X <= platform.X + platform.Width && X >= platform.X + platform.Width / 2)
                {
                    X = platform.X + platform.Width + 1;
                    velocity.X = 0;
                }
            }
        }

        public void UpdatePosition(GameTime gameTime)
        {
            if (position.Y < 1080 - 64)
            {
                if (velocity.Y < maxVelocity)
                {
                    velocity.Y += gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (velocity.Y > maxVelocity)
                {
                    velocity.Y = maxVelocity;
                }
            }
            if (position.Y > 1080 - 64)
            {
                velocity.Y = 0;
                position.Y = 1080 - 64;
                OnGround();
            }

            if (position.X < 0)
            {
                position.X = 0;
            }
            else if (position.X > 1920 - 64)
            {
                position.X = 1920 - 64;
            }

            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(idleTexture, new Rectangle((int) Position.X, (int) Position.Y, 64, 64), Color.White);
        }
    }
}