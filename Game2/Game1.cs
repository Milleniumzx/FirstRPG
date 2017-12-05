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
using System.Xml.Linq;

namespace Game2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        private Vector2 _chickenPosition;
        Vector2 _moveDirection;
        private Vector2 _yPosition;
        private SpriteFont theFont;
        private Effect _customEffect;
        private KeyboardState keyState;
        private GameObject _chicken;
        private Random _random;
        private int _score;
        private Rectangle _chickenrectangle;
        private Rectangle _playerRectangle;
        Color[] playerTextureData;
        Color[] chickenTextureData;
        protected float scale = 1f;
        private Tile[,] tileset;
        private Vector2 _targetPos;
        private PlayerCharacter player;
        private Vector2 x2y2;

        public Vector2 TargetPos
        {
            get { return _targetPos; }
            set { _targetPos = value; }
        }


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
            player = new PlayerCharacter("faggot", 100);
            player._playerCharacter = Content.Load<Texture2D>("playerChar");
            _moveDirection = new Vector2(0,0);
            _chicken = new GameObject(Content.Load<Texture2D>("chicken"));
            _random = new Random();
            _yPosition = new Vector2(0,15);
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
            _chickenrectangle = new Rectangle(0, 0, 16, 16);
            _chickenPosition = new Vector2(_random.Next(0, _graphics.GraphicsDevice.Viewport.Width - 50), _random.Next(0, _graphics.GraphicsDevice.Viewport.Height - 50));
            TargetPos = new Vector2(0,0);
            scale = 2;
            MouseInput.LastMouseState = MouseInput.MouseState;
            MouseInput.MouseState = Mouse.GetState();


            this.IsMouseVisible = true;
            IsFixedTimeStep = false;
            _score = 0;

            base.Initialize();
            tileset = GetTileset();
        }

        public Tile[,] GetTileset()
        {
            //Load from dungeon01.tmx
            XDocument xDoc = XDocument.Load("Content/dungeon01.tmx");
            int mapwidth = int.Parse(xDoc.Root.Attribute("width").Value);
            int mapheight = int.Parse(xDoc.Root.Attribute("height").Value);
            int tilecount = 105; //int.Parse(xDoc.Root.Element("tileset").Attribute("tilecount").Value);
            int columns = 21; //int.Parse(xDoc.Root.Element("tileset").Attribute("columns").Value);
            //Above commented code returns null for reasons unknown to mankind - manual insertion done temporarily.

            //Make arrays and a split char struct to seperate values
            string IDArray = xDoc.Root.Element("layer").Element("data").Value;
            //something something split the array with the , seperator.
            string[] splitArray = IDArray.Split(',');

            int[,] intIDs = new int[mapwidth, mapheight];

            for (int x = 0; x < mapwidth; x++)
            {
                for (int y = 0; y < mapheight; y++)
                {
                    intIDs[x, y] = int.Parse(splitArray[x + y * mapwidth]);
                }
            }

            //Tile selection - -16 in Y because of a weird offset.
            int key = 0;
            Vector2[] sourcePos = new Vector2[tilecount];
            for (int x = 0; x < tilecount / columns; x++)
            {
                for (int y = 0; y < columns; y++)
                {
                    sourcePos[key] = new Vector2(y * 16 - 16, x * 16);
                    key++;
                }
            }

            //loading the tileset png
            Texture2D sourceTex = Content.Load<Texture2D>("tilesets/tiled_tilesets/dungeon_tiles_compact_and_varied");

            //filling in the right tiles based on tmx map data.
            Tile[,] tiles = new Tile[mapwidth, mapheight];
            for (int x = 0; x < mapwidth; x++)
            {
                for (int y = 0; y < mapheight; y++)
                {
                    tiles[x, y] = new Tile
                    (
                        new Vector2(x * 16, y * 16),
                        sourceTex,
                        new Rectangle((int)sourcePos[intIDs[x, y]].X, (int)sourcePos[intIDs[x, y]].Y, 16, 16)
                    );
                }
            }
            return tiles;
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
        /// 
        /// 
        /// 
        void PlayerUpdate(GameTime gameTime)
        {
            float dT = (float)gameTime.ElapsedGameTime.TotalSeconds;
            player._playerPosition.X += player._playerVelocity.X * dT;
            player._playerPosition.Y += player._playerVelocity.Y * dT;
                
            player.CheckDest(x2y2, player._playerPosition);


            if ((MouseInput.MouseState.RightButton == ButtonState.Pressed) && (player.moving == false))
            {
                x2y2.X = MouseInput.MouseState.X;
                x2y2.Y = MouseInput.MouseState.Y;

                player.Move(x2y2, player._playerPosition);

            }
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }


            /*
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

            */


            //Click to move
            
            //MouseInput.LastMouseState = MouseInput.MouseState;
            //MouseInput.MouseState = Mouse.GetState();
 

            //if (MouseInput.LastMouseState.LeftButton == ButtonState.Released && MouseInput.MouseState.LeftButton == ButtonState.Pressed)
            //{
            //    _targetPos.X = (MouseInput.getMouseX()-40);
            //    _targetPos.Y = (MouseInput.getMouseY()-40);
            //}

            //if (Math.Abs(_playerPosition.X - TargetPos.X) < movespeed)
            //{
            //    //handle the case where we're very close and would over shoot the position by moving
            //    _playerPosition.X = TargetPos.X;
            //}
            //else if (_playerPosition.X < TargetPos.X)
            //{
            //    //we're at a position less than the target, add to our position
            //    movespeed_x = (TargetPos.X - _playerPosition.X)/80;
            //    _playerPosition.X += movespeed_x;
            //}
            //else if (_playerPosition.X > TargetPos.X)
            //{
            //    //we're at a position greater than the target, subtract from our position
            //    movespeed_x = (_playerPosition.X - TargetPos.X)/80;
            //    _playerPosition.X -= movespeed_x;
            //}

            //if (Math.Abs(_playerPosition.Y - TargetPos.Y) < movespeed)
            //{
            //    //handle the case where we're very close and would over shoot the position by moving
            //    _playerPosition.Y = TargetPos.Y;
            //}
            //else if (_playerPosition.Y < TargetPos.Y)
            //{
            //    //we're at a position less than the target, add to our position
            //    movespeed_y = (TargetPos.Y - _playerPosition.Y)/80;
            //    _playerPosition.Y += movespeed_y;
            //}
            //else if (_playerPosition.Y > TargetPos.Y)
            //{
            //    //we're at a position greater than the target, subtract from our position
            //    movespeed_y = (_playerPosition.Y - TargetPos.Y)/80;
            //    _playerPosition.Y -= movespeed_y;
            //}



            //Debug.WriteLine($"Position: {_playerPosition.X}.{_playerPosition.Y}");

            /*boundary logic
            if (_playerPosition.X - _chickenPosition.X < -3 && _playerPosition.Y - _chickenPosition.Y < -3)
            {
                _score++;
                _chicken._exists = false;
            }
            */

            //Boundary logic
            if (player._playerPosition.X < 0)
            {
                player._playerPosition.X = 0;
            }
            else if (player._playerPosition.X > _graphics.GraphicsDevice.Viewport.Width - player._playerCharacter.Width)
            {
                player._playerPosition.X = (_graphics.GraphicsDevice.Viewport.Width - player._playerCharacter.Width);
            }

            if (player._playerPosition.Y < 0)
            {
                player._playerPosition.Y = 0;
            }
            else if (player._playerPosition.Y > _graphics.GraphicsDevice.Viewport.Height - player._playerCharacter.Height)
            {
                player._playerPosition.Y = (_graphics.GraphicsDevice.Viewport.Height - player._playerCharacter.Height);
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
            playerTextureData = new Color[player._playerCharacter.Width * player._playerCharacter.Height];
            player._playerCharacter.GetData(playerTextureData);


            Rectangle _playerRectangle = new Rectangle((int)player._playerPosition.X, (int)player._playerPosition.Y, player._playerCharacter.Width, player._playerCharacter.Height);

            _chickenrectangle.X = (int) _chickenPosition.X;
            _chickenrectangle.Y = (int) _chickenPosition.Y;

            if (IntersectPixels(_playerRectangle, playerTextureData, _chickenrectangle, chickenTextureData) == true)
            {
                _chicken._exists = false;
                _score++;


            }

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // TODO: Add your update logic here
            PlayerUpdate(gameTime);

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
            
            
            //vector2.X = rng.Next(0, 720);
            //vector2.Y = rng.Next(0, 400);
            _chickenPosition.Y = rng.Next(50, _graphics.GraphicsDevice.Viewport.Height-50);
            _chickenPosition.X = rng.Next(50, _graphics.GraphicsDevice.Viewport.Width-50);
            Debug.WriteLine($"New random was made at {_chickenPosition.X}.{_chickenPosition.Y} - Spawning chicken");

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
            //_spriteBatch.Draw(_chicken.Image, _chickenPosition, Color.White);
            foreach (var t in tileset)
            {
                t.Draw(_spriteBatch);
                //Doesn't work as intended, but i'm too tired to fix this shit
            }
            _spriteBatch.Draw(_chicken.Image, _chickenPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            _spriteBatch.Draw(player._playerCharacter, player._playerPosition, Color.White);
            _spriteBatch.DrawString(theFont,$"Position: X:{player._playerPosition.X}", Vector2.Zero, Color.White);
            _spriteBatch.DrawString(theFont, $"Position: Y:{player._playerPosition.Y}", _yPosition, Color.White);
            _spriteBatch.DrawString(theFont,$"Chicken position: {_chickenPosition}", _yPosition + _yPosition, Color.White);
            _spriteBatch.DrawString(theFont,$"Score: {_score}", _yPosition + (_yPosition * 2), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);


        }

    }
}
