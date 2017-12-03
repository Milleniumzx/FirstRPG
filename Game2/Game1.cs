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
        Vector2 _moveDirection;
        private Vector2 _yPosition;
        private SpriteFont theFont;
        private float movespeed = 300;
        private Effect _customEffect;

        private KeyboardState keyState;
        

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
            _yPosition = new Vector2(0,15);
            this.IsMouseVisible = true;
            IsFixedTimeStep = false;

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

            //Debug.WriteLine($"Position: {_playerPosition.X}.{_playerPosition.Y}");

            //boundary logic
            /*
            if (playerPosition.X < 0)
            {
                playerPosition.X = 0;
            }
            else if (playerPosition.X > graphics.GraphicsDevice.Viewport.Width - playerCharacter.Width)
            {
                playerPosition.X = (graphics.GraphicsDevice.Viewport.Width - playerCharacter.Width);
            }

            if (playerPosition.Y < 0)
            {
                playerPosition.Y = 0;
            }
            else if (playerPosition.Y > graphics.GraphicsDevice.Viewport.Height - playerCharacter.Height)
            {
                playerPosition.Y = (graphics.GraphicsDevice.Viewport.Height - playerCharacter.Height);
            }
            */
            // TODO: Add your update logic here

            base.Update(gameTime);
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
            _spriteBatch.Draw(_playerCharacter, _playerPosition, Color.White);
            _spriteBatch.DrawString(theFont,$"Position: X:{_playerPosition.X}", Vector2.Zero, Color.White);
            _spriteBatch.DrawString(theFont, $"Position: Y:{_playerPosition.Y}", _yPosition, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);


        }

    }
}
