using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game2
{
    public class PlayerCharacter
    {
        private string _name;
        private int _health;
        public Texture2D _playerCharacter;
        public Vector2 _playerPosition;
        public float movespeed = 2;






        public PlayerCharacter(string name, int health)
        {
            _name = name;
            _health = health;

            //    _isDead = false;
        }





    }
}
