using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game2
{
    class GameObject
    {

        private Texture2D _image;
        public bool _exists;
        private Vector2 _centerOrigin;



        public Texture2D Image
        {
            get { return _image; }
            set { _image = value; }
        }

        public GameObject(Texture2D image)
        {
            _image = image;
            _exists = false;
            _centerOrigin = new Vector2(image.Width / 2, image.Height / 2);
        }



    

    }
}
