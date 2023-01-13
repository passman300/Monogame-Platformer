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
    // store the types of collisions
    public enum TileCollision
    {
        // personal note: enum is similar to CONST variable, but are readable

        // tile collision types
        Passable = 0, // player can pass through the tile
        Impassable = 1, // player can not pass through the tile

        // player is able to move left/right on the tile
        // is only passable when the player is under it
        Platform = 2,

    }

    public class Tile
    {
        
        ContentManager content;

        // store the type of tile
        private Texture2D texture;
        private TileCollision collisionType;

        // store the tile rectangle
        private Rectangle rec;

        // store the tile size
        private const int WIDTH = 20;
        private const int HEIGHT = 20;

        // allow other classes to read tile size
        // can't not do a property since it is need to create a Tile object first, to use get
        public static readonly Vector2 Size = new Vector2(WIDTH, HEIGHT);

        // get location property
        public Vector2 GetLoc
        {
            get { return new Vector2(rec.X, rec.Y); }
        }

        // get tile rectangle
        public Rectangle GetRec
        {
            get { return rec; }
        }

        // get tile collision type property
        public TileCollision GetCollision
        {
            get { return collisionType; }
        }

        // tile constructor
        public Tile(Texture2D img, TileCollision collisionType, Vector2 pos)
        {
            // store the image of the tile
            texture = img;

            // store the collision type of the player
            this.collisionType = collisionType;

            rec = new Rectangle((int)pos.X, (int)pos.Y, WIDTH, HEIGHT);
        }

        // draw the tile
        public void DrawTile(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rec, Color.White);
        }
    }
}
