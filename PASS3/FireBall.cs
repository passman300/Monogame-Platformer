using Animation2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PASS3
{
    public class FireBall
    {
        public enum FireBallDirection
        {
            left = 0,
            right = 1
        }

        // basic pos, rectangle each fireBall has
        private Vector2 pos;
        public static readonly Vector2 size = new Vector2(20, 20);
        private Rectangle rec;

        // fireBall direction and spd
        private Enemy.FaceDirection dir;
        private Vector2 spd = new Vector2(4, 0);

        // fireBall damange will deal on player
        private int damage = 30;

        // fireBall animtion
        private Animation anim;

        // returns the damage dealt by each fireBall
        public int GetDamage 
        { 
            get { return damage; }
        }

        // returns the rectangle of the each fireBall
        public Rectangle GetRec
        {
            get { return rec; }
        }

        public FireBall(Animation anim, Vector2 pos, Enemy.FaceDirection dir)
        {
            // store passed variables
            this.anim = anim;
            this.pos = pos;
            this.dir = dir;

            // set up fireball's rectangle and animation size
            rec = new Rectangle((int)this.pos.X, (int)this.pos.Y, (int)size.X, (int)size.Y);
            this.anim.destRec = rec;

        }

        // update the fire balls animations and position
        public void Update(GameTime gameTime)
        {
            // update position and rectangle of the fireball
            if (dir == Enemy.FaceDirection.Left)
            {
                pos.X -= spd.X;
            }
            else
            {
                pos.X += spd.X;
            }

            rec.X = (int)pos.X;
            rec.Y = (int)pos.Y;

            UpdateAnim(gameTime);
        }

        // update the animations of the fireball
        private void UpdateAnim(GameTime gameTime)
        {
            // update location
            anim.destRec.X = (int)pos.X;
            anim.destRec.Y = (int)pos.Y;

            anim.Update(gameTime);
        }

        // draw the fireball
        public void Draw(SpriteBatch spriteBatch)
        {
            // flip the animation if the direction is left
            SpriteEffects animFlip;
            if (dir == Enemy.FaceDirection.Left)
            {
                animFlip = SpriteEffects.FlipHorizontally;
            }
            else
            {
                animFlip = SpriteEffects.None;
            }

            anim.Draw(spriteBatch, Color.White, animFlip);
        }
    }
}
