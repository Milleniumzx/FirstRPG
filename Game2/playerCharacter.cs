using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
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
    public class PlayerCharacter
    {
        private string _name;
        private int _health;
        public Texture2D _playerCharacter;
        public Vector2 _playerPosition;
        public Vector2 _playerVelocity;
        public float movespeed;
        public bool moving;
        private Vector2 dist;
        public Vector2 _centerOrigin;
        public string _sprite;





        public PlayerCharacter(string name, int health, string sprite)
        {
            _name = name;
            _health = health;
            _playerPosition = new Vector2(100, 100);
            _playerVelocity = Vector2.One;
            movespeed = 2;
            moving = false;
            _sprite = sprite;

            //    _isDead = false;
        }

        public Vector2 GetCenterOrigin(int width, int height)
        {
            _centerOrigin = new Vector2(width/2, height/2);


            return _centerOrigin;
        }

        public void Move(Vector2 dest, Vector2 pos)
        {
            dist.X = pos.X - dest.X;
            dist.Y = pos.Y - dest.Y;

            _playerVelocity.X -= dist.X;
            _playerVelocity.Y -= dist.Y;

            moving = true;
        }

        public void CheckDest(Vector2 dest, Vector2 pos)
        {
            if (Vector2.Distance(pos, dest) < 0.1f)
            {
                _playerVelocity.X = 0;
                _playerVelocity.Y = 0;
                moving = false;
            }
        }

    }
}
