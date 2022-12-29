using Animation2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PASS3
{
    // inheritance the enemy class
    class Runner : Enemy
    {
        // enemy body parts
        private const int FEET = 0;
        private const int LEFT = 1;
        private const int RIGHT = 2;

        // store the force of gravity
        private const float GRAV_ACCEL = 8f / 60;

        // create runner's sub hitboxes
        private Rectangle[] recs = new Rectangle[3];

        // store if the runner is on the ground
        private bool isGround;

        public Runner(byte state, Dictionary<byte, Animation> aniDict, Rectangle rec, 
                      FaceDirection dir, float maxHealth, Vector2 spd): 
                      base(state, aniDict, rec, dir, maxHealth, spd)
        {
        }

        public void LoadRecs()
        {
            // Loc (x, y): top right of general rectangle
            // Size (width, height): 50% of width, 75% of height
            recs[LEFT] = new Rectangle(GetRec.X, GetRec.Y, (int)(GetRec.Width * 0.5f), (int)(GetRec.Height * 0.75f));

            // Loc (x, y): RIGHT of LEFT rectangle
            // Size (width, height): same as LEFT rectangle
            recs[RIGHT] = new Rectangle(recs[LEFT].X, GetRec.Y, recs[LEFT].Width, recs[LEFT].Height);

            // Loc (x, y): under LEFT rectangle, and in middle of general rec
            // Size (width, height): 25% of width, rest of space (height)
            recs[FEET] = new Rectangle(GetRec.X + GetRec.Width / 2 - (int)(GetRec.Width * 0.25f / 2), recs[LEFT].Y,
                                (int)(GetRec.Width * 0.25f), GetRec.Height - recs[LEFT].Height);
        }

        // check if runner has collision
        public void UpdateCollision(Tile tile)
        {
            isGround = false;

            bool collision = false;

            if (tile.GetCollision != TileCollision.Passable)
            {
                Rectangle tileRec = tile.GetRec;

                // stage 1: check collision with general rectangle
                if (GetRec.Intersects(tileRec))
                {
                    // stage 2: check collision with sub hitboxes
                    if (recs[LEFT].Intersects(tileRec))
                    {
                        
                    }
                    if (recs[FEET].Intersects(tileRec))
                    {
                        isGround = true;
                    }
                    
                }
                


            }
        }

        public void Update(GameTime gameTime)
        {
            if (feet )
        }

    }
}
