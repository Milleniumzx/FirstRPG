using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Graphics.Effects;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
using MonoGame.Extended.Tiled.Graphics.Effects;
using MonoGame.Extended.ViewportAdapters;

namespace Game2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        Texture2D _playerCharacter;
        Vector2 _playerPosition;
        private Vector2 _chickenPosition;
        Vector2 _moveDirection;
        private Vector2 _yPosition;
        private SpriteFont theFont;
        private float movespeed = 600;
        private Effect _customEffect;
        private KeyboardState keyState;
        private GameObject _chicken;
        private Random _random;
        private int _score;
        private Rectangle _chickenrectangle;
        private Rectangle _playerRectangle;
        Color[] playerTextureData;
        Color[] chickenTextureData;




        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _playerPosition = new Vector2(100, 100);
            
            _playerCharacter = Content.Load<Texture2D>("playerChar");
            _moveDirection = new Vector2(0,0);
            _chicken = new GameObject(Content.Load<Texture2D>("chicken"));
            
            _random = new Random();
            _yPosition = new Vector2(0,15);
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
            _chickenrectangle = new Rectangle(0, 0, 16, 16);


            this.IsMouseVisible = true;
            IsFixedTimeStep = false;
            _score = 0;





            _chickenPosition = new Vector2(_random.Next(0, _graphics.GraphicsDevice.Viewport.Width-50), _random.Next(0, _graphics.GraphicsDevice.Viewport.Height - 50));


            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            theFont = Content.Load<SpriteFont>("theFont");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            keyState = Keyboard.GetState();

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                _playerPosition.X -= movespeed * (float) gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                _playerPosition.X += movespeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                _playerPosition.Y -= movespeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                _playerPosition.Y += movespeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                GeneratePos();
            }

            //Debug.WriteLine($"Position: {_playerPosition.X}.{_playerPosition.Y}");

            /*boundary logic
            if (_playerPosition.X - _chickenPosition.X < -3 && _playerPosition.Y - _chickenPosition.Y < -3)
            {
                _score++;
                _chicken._exists = false;
            }
            */
            
            if (_playerPosition.X < 0)
            {
                _playerPosition.X = 0;
            }
            else if (_playerPosition.X > _graphics.GraphicsDevice.Viewport.Width - _playerCharacter.Width)
            {
                _playerPosition.X = (_graphics.GraphicsDevice.Viewport.Width - _playerCharacter.Width);
            }

            if (_playerPosition.Y < 0)
            {
                _playerPosition.Y = 0;
            }
            else if (_playerPosition.Y > _graphics.GraphicsDevice.Viewport.Height - _playerCharacter.Height)
            {
                _playerPosition.Y = (_graphics.GraphicsDevice.Viewport.Height - _playerCharacter.Height);
            }

            //chicken logic

            if (_chicken._exists == false)
            {
                GeneratePos();
                _chicken._exists = true;
            }




            //collision
            chickenTextureData = new Color[_chicken.Image.Width * _chicken.Image.Height];
            _chicken.Image.GetData(chickenTextureData);
            playerTextureData = new Color[_playerCharacter.Width * _playerCharacter.Height];
            _playerCharacter.GetData(playerTextureData);


            Rectangle _playerRectangle = new Rectangle((int)_playerPosition.X, (int)_playerPosition.Y, _playerCharacter.Width, _playerCharacter.Height);

            _chickenrectangle.X = (int) _chickenPosition.X;
            _chickenrectangle.Y = (int) _chickenPosition.Y;

            if (IntersectPixels(_playerRectangle, playerTextureData, _chickenrectangle, chickenTextureData) == true)
            {
                _chicken._exists = false;
                _score++;


            }
            


            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        public static bool IntersectPixels(Rectangle rectangleA, Color[] dataA,
            Rectangle rectangleB, Color[] dataB)
        {
            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }

            // No intersection found
            return false;
        }





        private Random rng;
        public void GeneratePos()
        {
            rng = new Random();
            Debug.WriteLine("new random was made, proceeding...");
            
            //vector2.X = rng.Next(0, 720);
            //vector2.Y = rng.Next(0, 400);
            _chickenPosition.Y = rng.Next(50, _graphics.GraphicsDevice.Viewport.Height-50);
            _chickenPosition.X = rng.Next(50, _graphics.GraphicsDevice.Viewport.Width-50);

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            _spriteBatch.Draw(_chicken.Image, _chickenPosition, Color.White);
            _spriteBatch.Draw(_playerCharacter, _playerPosition, Color.White);
            _spriteBatch.DrawString(theFont,$"Position: X:{_playerPosition.X}", Vector2.Zero, Color.White);
            _spriteBatch.DrawString(theFont, $"Position: Y:{_playerPosition.Y}", _yPosition, Color.White);
            _spriteBatch.DrawString(theFont,$"Chicken position: {_chickenPosition}", _yPosition + _yPosition, Color.White);
            _spriteBatch.DrawString(theFont,$"Score: {_score}", _yPosition + (_yPosition * 2), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);


        }

    }
}
