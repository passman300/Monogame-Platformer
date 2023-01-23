using Animation2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS3
{
    class EndDoor
    {
        // local content manger
        ContentManager content;

        // store texture of the door
        private Texture2D texture;

        // width and height of the door
        private const int WIDTH = 25;
        private const int HEIGHT = 50;

        public static readonly Vector2 Size = new Vector2(WIDTH, HEIGHT);

        private Rectangle rec;

        public EndDoor(Texture2D img, Vector2 pos)
        {
            texture = img;

            rec = new Rectangle((int)pos.X, (int)pos.Y, (int)Size.X, (int)Size.Y);
        }
    }
}
