using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game2
{
    public class Tile
    {
        private Vector2 pos;
        private Texture2D tex;
        private Rectangle sourceRect;


        public Tile(Vector2 a_pos, Texture2D a_tex, Rectangle a_sourceRect)
        {
            pos = a_pos;
            tex = a_tex;
            sourceRect = a_sourceRect;
        }

        public void Draw(SpriteBatch a_sb)
        {
            a_sb.Draw(tex, pos, null, sourceRect, null, 0f, null, null, SpriteEffects.None, 0f);
        }

    }
}
