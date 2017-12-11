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
        private int _scoreE;
        private int _scoreE2;
        private Rectangle _chickenrectangle;
        private Rectangle _playerRectangle;
        private Rectangle _enemyRectangle;
        private Rectangle _enemy2Rectangle;
        Color[] playerTextureData;
        Color[] chickenTextureData;
        Color[] enemyTextureData;
        Color[] enemy2TextureData;
        protected float scale = 1f;
        private Tile[,] tileset;
        private Vector2 _targetPos;
        private Vector2 _targetPosE;
        private PlayerCharacter player;
        private PlayerCharacter enemy;
        private PlayerCharacter enemy2;
        private Vector2 x2y2;
        private float movespeed_x;
        private float movespeed_y;
        private float movespeed_xE;
        private float movespeed_yE;
        private Vector2 winScreen;

        public Vector2 TargetPos
        {
            get { return _targetPos; }
            set { _targetPos = value; }
        }
        public Vector2 TargetPosE
        {
            get { return _targetPosE; }
            set { _targetPosE = value; }
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
            enemy2 = new PlayerCharacter("bigfaggot", 100);
            enemy = new PlayerCharacter("evilfaggot", 100);
            enemy2._playerCharacter = Content.Load<Texture2D>("playerChar");
            player._playerCharacter = Content.Load<Texture2D>("playerChar");
            enemy._playerCharacter = Content.Load<Texture2D>("playerChar");
            _moveDirection = new Vector2(0,0);
            _chicken = new GameObject(Content.Load<Texture2D>("chicken"));
            _random = new Random();
            keyState = new KeyboardState();
            _yPosition = new Vector2(0,15);
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
            _chickenrectangle = new Rectangle(0, 0, 16, 16);
            winScreen = new Vector2(600, 400);
            _chickenPosition = new Vector2(_random.Next(0, _graphics.GraphicsDevice.Viewport.Width - 50), _random.Next(0, _graphics.GraphicsDevice.Viewport.Height - 50));
            TargetPos = new Vector2(0,0);
            TargetPosE = new Vector2(0, 0);
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
        //void PlayerUpdate(GameTime gameTime)
        //{
        //    float dT = (float)gameTime.ElapsedGameTime.TotalSeconds;
        //    player._playerPosition.X += player._playerVelocity.X * dT;
        //    player._playerPosition.Y += player._playerVelocity.Y * dT;
                
        //    player.CheckDest(x2y2, player._playerPosition);


        //    if ((MouseInput.MouseState.RightButton == ButtonState.Pressed) && (player.moving == false))
        //    {
        //        x2y2.X = MouseInput.MouseState.X;
        //        x2y2.Y = MouseInput.MouseState.Y;

        //        player.Move(x2y2, player._playerPosition);

        //    }
        //}
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }



            keyState = Keyboard.GetState();

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                player._playerPosition.X -= enemy.movespeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                player._playerPosition.X += enemy.movespeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                player._playerPosition.Y -= enemy.movespeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                player._playerPosition.Y += enemy.movespeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.G))
            {
                enemy2._playerPosition.X -= enemy.movespeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.J))
            {
                enemy2._playerPosition.X += enemy.movespeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Y))
            {
                enemy2._playerPosition.Y -= enemy.movespeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.H))
            {
                enemy2._playerPosition.Y += enemy.movespeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                enemy._playerPosition.X -= enemy.movespeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                enemy._playerPosition.X += enemy.movespeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                enemy._playerPosition.Y -= enemy.movespeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                enemy._playerPosition.Y += player.movespeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }



            //if (Keyboard.GetState().IsKeyDown(Keys.A))
            //{
            //    GeneratePos();
            //}



            ////Enemy move logic
            //float speed_per_tickE = 5;
            //float delta_x1E = TargetPosE.X - enemy._playerPosition.X;
            //float delta_x2E = enemy._playerPosition.X - TargetPosE.X;
            //float delta_y1E = TargetPosE.Y - enemy._playerPosition.Y;
            //float delta_y2E = enemy._playerPosition.Y - TargetPosE.Y;
            //float goal_distE = (float)Math.Sqrt((delta_x1E * delta_x1E) + (delta_y1E * delta_y1E));
            //float ratioE;

            //if (enemy.moving == false)
            //{
            //    if (delta_x1E < delta_x2E)
            //    {
            //        _targetPosE.X += 5;
            //        enemy.moving = true;
            //    }
            //    else
            //    {
            //        _targetPosE.X -= 5;
            //        enemy.moving = true;
            //    }

            //}
            //else
            //{
            //    _targetPosE.X = _chickenPosition.X - 5;
            //    _targetPosE.Y = _chickenPosition.Y - 5;

            //}


            //if (goal_distE > speed_per_tickE)
            //{
            //    ratioE = speed_per_tickE / goal_distE;
            //    movespeed_xE = (ratioE * delta_x1E);
            //    movespeed_yE = (ratioE * delta_y1E);
            //    enemy._playerPosition.X = movespeed_xE + enemy._playerPosition.X + 1;
            //    enemy._playerPosition.Y = movespeed_yE + enemy._playerPosition.Y + 1;
            //}
            //else //if (Math.Abs(delta_x2) < player.movespeed && Math.Abs(delta_y2) < player.movespeed)
            //{
            //    //handle the case where we're very close and would over shoot the position by moving
            //    //player._playerPosition.X = TargetPos.X;
            //    //player._playerPosition.Y = TargetPos.Y;
            //    enemy.movespeed = 0;
            //    enemy._playerPosition.X -= 2; //this makes it go faster down right but no stuck
            //    enemy._playerPosition.Y -= 2;
            //}


            ////Click to move

            //MouseInput.LastMouseState = MouseInput.MouseState;
            //MouseInput.MouseState = Mouse.GetState();
            //float speed_per_tick = 5;
            //float delta_x1 = TargetPos.X - player._playerPosition.X;
            //float delta_x2 = player._playerPosition.X - TargetPos.X;
            //float delta_y1 = TargetPos.Y - player._playerPosition.Y;
            //float delta_y2 = player._playerPosition.Y - TargetPos.Y;
            //float goal_dist = (float)Math.Sqrt((delta_x1 * delta_x1) + (delta_y1 * delta_y1));
            //float ratio;

            //if (MouseInput.LastMouseState.LeftButton == ButtonState.Released && MouseInput.MouseState.LeftButton == ButtonState.Pressed || MouseInput.MouseState.LeftButton == ButtonState.Pressed)

            //    {
            //        _targetPos.X = (MouseInput.getMouseX() - 40);
            //    _targetPos.Y = (MouseInput.getMouseY() - 40);
            //}

            //if (goal_dist > speed_per_tick)
            //{
            //    ratio = speed_per_tick / goal_dist;
            //    movespeed_x = ratio * delta_x1;
            //    movespeed_y = ratio * delta_y1;
            //    player._playerPosition.X = movespeed_x + player._playerPosition.X;
            //    player._playerPosition.Y = movespeed_y + player._playerPosition.Y;
            //}
            //else //if (Math.Abs(delta_x2) < player.movespeed && Math.Abs(delta_y2) < player.movespeed)
            //{
            //    //handle the case where we're very close and would over shoot the position by moving
            //    //player._playerPosition.X = TargetPos.X;
            //    //player._playerPosition.Y = TargetPos.Y;
            //    player.movespeed = 0;
            //}




            //if (Math.Abs(delta_x2) < player.movespeed)
            //{
            //    //handle the case where we're very close and would over shoot the position by moving
            //    player._playerPosition.X = TargetPos.X;
            //}
            //else if (player._playerPosition.X < TargetPos.X)
            //{
            //    if (delta_x1 > delta_y1)
            //    {
            //        movespeed_x = 4;
            //    }
            //    else
            //    {
            //        movespeed_x = 2;
            //    }
            //    player._playerPosition.X += movespeed_x;
            //    //we're at a position less than the target, add to our position

            //}
            //else if (player._playerPosition.X > TargetPos.X)
            //{
            //    if (delta_x2 > delta_y2)
            //    {
            //        movespeed_x = 4;
            //    }
            //    else
            //    {
            //        //we're at a position greater than the target, subtract from our position
            //        movespeed_x = 2 ;
            //    }
            //    player._playerPosition.X -= movespeed_x;
            //}

            //if (Math.Abs(delta_y2) < player.movespeed)
            //{
            //    //handle the case where we're very close and would over shoot the position by moving
            //    player._playerPosition.Y = TargetPos.Y;
            //}
            //else if (player._playerPosition.Y < TargetPos.Y)
            //{
            //    if (delta_y1 > delta_x1)
            //    {
            //        movespeed_y = 4;
            //    }
            //    else
            //    {
            //        movespeed_y = 2;
            //    }
            //    player._playerPosition.Y += movespeed_y;
            //}
            //else if (player._playerPosition.Y > TargetPos.Y)
            //{
            //    if (delta_y2 > delta_x2)
            //    {
            //        movespeed_y = 4;
            //    }
            //    else
            //    {
            //        //we're at a position greater than the target, subtract from our position
            //        movespeed_y = 2;
            //    }
            //    player._playerPosition.Y -= movespeed_y;
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

            if (enemy._playerPosition.X < 0)
            {
                enemy._playerPosition.X = 0;
            }
            else if (enemy._playerPosition.X > _graphics.GraphicsDevice.Viewport.Width - enemy._playerCharacter.Width)
            {
                enemy._playerPosition.X = (_graphics.GraphicsDevice.Viewport.Width - enemy._playerCharacter.Width);
            }

            if (enemy._playerPosition.Y < 0)
            {
                enemy._playerPosition.Y = 0;
            }
            else if (enemy._playerPosition.Y > _graphics.GraphicsDevice.Viewport.Height - enemy._playerCharacter.Height)
            {
                enemy._playerPosition.Y = (_graphics.GraphicsDevice.Viewport.Height - enemy._playerCharacter.Height);
            }

            if (enemy2._playerPosition.X < 0)
            {
                enemy2._playerPosition.X = 0;
            }
            else if (enemy2._playerPosition.X > _graphics.GraphicsDevice.Viewport.Width - enemy2._playerCharacter.Width)
            {
                enemy2._playerPosition.X = (_graphics.GraphicsDevice.Viewport.Width - enemy2._playerCharacter.Width);
            }

            if (enemy2._playerPosition.Y < 0)
            {
                enemy2._playerPosition.Y = 0;
            }
            else if (enemy2._playerPosition.Y > _graphics.GraphicsDevice.Viewport.Height - enemy2._playerCharacter.Height)
            {
                enemy2._playerPosition.Y = (_graphics.GraphicsDevice.Viewport.Height - enemy2._playerCharacter.Height);
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
            enemyTextureData = new Color[enemy._playerCharacter.Width * enemy._playerCharacter.Height];
            enemy._playerCharacter.GetData(enemyTextureData);
            enemy2TextureData = new Color[enemy2._playerCharacter.Width * enemy2._playerCharacter.Height];
            enemy2._playerCharacter.GetData(enemy2TextureData);


            Rectangle _playerRectangle = new Rectangle((int)player._playerPosition.X, (int)player._playerPosition.Y, player._playerCharacter.Width, player._playerCharacter.Height);
            Rectangle _enemyRectangle = new Rectangle((int)enemy._playerPosition.X, (int)enemy._playerPosition.Y, enemy._playerCharacter.Width, enemy._playerCharacter.Height);
            Rectangle _enemy2Rectangle = new Rectangle((int)enemy2._playerPosition.X, (int)enemy2._playerPosition.Y, enemy2._playerCharacter.Width, enemy2._playerCharacter.Height);


            _chickenrectangle.X = (int) _chickenPosition.X;
            _chickenrectangle.Y = (int) _chickenPosition.Y;

            if (IntersectPixels(_playerRectangle, playerTextureData, _chickenrectangle, chickenTextureData) == true)
            {
                _chicken._exists = false;
                _score++;


            }
            if (IntersectPixels(_enemy2Rectangle, enemy2TextureData, _chickenrectangle, chickenTextureData) == true)
            {
                _chicken._exists = false;
                _scoreE2++;


            }
            if (IntersectPixels(_enemyRectangle, enemyTextureData, _chickenrectangle, chickenTextureData) == true)
            {
                _chicken._exists = false;
                _scoreE++;

            }

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // TODO: Add your update logic here
            //PlayerUpdate(gameTime);

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
            _spriteBatch.Draw(enemy._playerCharacter, enemy._playerPosition, Color.Blue);
            _spriteBatch.Draw(enemy2._playerCharacter, enemy2._playerPosition, Color.Red);
            _spriteBatch.DrawString(theFont,$"Position: X:{player._playerPosition.X}", Vector2.Zero, Color.White);
            _spriteBatch.DrawString(theFont, $"Position: Y:{player._playerPosition.Y}", _yPosition, Color.White);
            _spriteBatch.DrawString(theFont,$"Chicken position: {_chickenPosition}", _yPosition + _yPosition, Color.White);
            _spriteBatch.DrawString(theFont,$"Score: {_score}", _yPosition + (_yPosition * 2), Color.White);
            _spriteBatch.DrawString(theFont, $"Enemy Score: {_scoreE}", _yPosition + (_yPosition * 3), Color.White);
            _spriteBatch.DrawString(theFont, $"player3 Score: {_scoreE2}", _yPosition + (_yPosition * 4), Color.White);
            _spriteBatch.End();

            if (_score >= 15)
            {
                GraphicsDevice.Clear(Color.Black);
                _spriteBatch.Begin();
                _spriteBatch.DrawString(theFont, "player1 Win! Press Y to restart!", winScreen, Color.White);
                _spriteBatch.End();
            }
            else if (_scoreE >= 15)
            {
                GraphicsDevice.Clear(Color.Black);
                _spriteBatch.Begin();
                _spriteBatch.DrawString(theFont, "player2 win! Press Y to restart!", winScreen, Color.White);
                _spriteBatch.End();
            }
            else if (_scoreE2 >= 15)
            {
                GraphicsDevice.Clear(Color.Black);
                _spriteBatch.Begin();
                _spriteBatch.DrawString(theFont, "player3 win! Press Y to restart!", winScreen, Color.White);
                _spriteBatch.End();
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.B))
            {
                GeneratePos();
                player._playerPosition.X = 100;
                player._playerPosition.Y = 100;
                enemy._playerPosition.X = 100;
                enemy._playerPosition.Y = 100;
                enemy2._playerPosition.X = 100;
                enemy2._playerPosition.Y = 100;
                _scoreE2 = 0;
                _scoreE = 0;
                _score = 0;
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
                _spriteBatch.Draw(enemy2._playerCharacter, enemy2._playerPosition, Color.Red);
                _spriteBatch.Draw(enemy._playerCharacter, enemy._playerPosition, Color.Blue);
                _spriteBatch.DrawString(theFont, $"Position: X:{player._playerPosition.X}", Vector2.Zero, Color.White);
                _spriteBatch.DrawString(theFont, $"Position: Y:{player._playerPosition.Y}", _yPosition, Color.White);
                _spriteBatch.DrawString(theFont, $"Chicken position: {_chickenPosition}", _yPosition + _yPosition, Color.White);
                _spriteBatch.DrawString(theFont, $"Score: {_score}", _yPosition + (_yPosition * 2), Color.White);
                _spriteBatch.DrawString(theFont, $"Enemy Score: {_scoreE}", _yPosition + (_yPosition * 3), Color.White);
                _spriteBatch.DrawString(theFont, $"player3 Score: {_scoreE2}", _yPosition + (_yPosition * 4), Color.White);
                _spriteBatch.End();
            }



            base.Draw(gameTime);


        }

    }
}
