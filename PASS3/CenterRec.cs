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
    class CenterRec
    {
        Rectangle rec1;
        Rectangle rec2;

        public CenterRec(Rectangle rec1, Rectangle rec2)
        {
            this.rec1 = rec1;
            this.rec2 = rec2;
        }

        // returns the location where rec 2 is at horizontal and vertical center of rec 1
        public Vector2 CenterAll()
        {
            // create temporary vector
            Vector2 centerLoc;
            centerLoc.X = rec2.X;
            centerLoc.Y = rec2.Y;

            centerLoc.X = CenterWidth().X;
            centerLoc.Y = CenterHeight().Y;

            return centerLoc;
        }


        // returns the location where rec 2 is at the horizontal center of rec 1
        public Vector2 CenterWidth()
        {
            // create temporary vector
            Vector2 centerLoc;
            centerLoc.X = rec2.X;
            centerLoc.Y = rec2.Y;

            // calculate the horizontal center
            centerLoc.X = rec1.X + (int)(rec1.Width * 0.5f) - (int)(rec2.Width * 0.5f);
            return centerLoc;
        }

        // returns the location where rec 2 is at the vertical center of rec 1
        public Vector2 CenterHeight()
        {
            // create temporary vector
            Vector2 centerLoc;
            centerLoc.X = rec2.X;
            centerLoc.Y = rec2.Y;

            // calculate the vertical center
            centerLoc.Y = rec1.X + (int)(rec1.Height * 0.5f) - (int)(rec2.Height * 0.5f);
            return centerLoc;
        }
    }
}
