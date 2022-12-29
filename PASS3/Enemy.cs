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
    public class Enemy
    {
        // enum on where the enemy is facing
        public enum FaceDirection
        {
            Left = -1,
            Right = 1,
        }

        // used to view hitboxes
        GraphicsDevice graphicsDevice;

        // enemy sprites
        private Dictionary<byte, Animation> aniDict;

        // enemy state
        private byte defaultState;
        private byte state;

        // enemy location
        private Vector2 startPos;
        private Vector2 pos;

        // enemy rectangles
        private Rectangle rec;

        // enemy spd
        private Vector2 enemySpd;

        // enemy direction
        private FaceDirection startDir;
        private FaceDirection dir;

        // enemy health variables
        private float maxHealth;
        private float currHealth;


        // set and get enemy state
        public byte EnemyState
        {
            set { state = value; }
            get { return state; }
        }

        // get location property
        public Vector2 GetLoc
        {
            get { return pos; }
        }

        // get enemy rectangle
        public Rectangle GetRec
        {
            get { return rec; }
        }


        // constructor for enemy
        public Enemy(byte state, Dictionary<byte, Animation> aniDict, Rectangle rec, FaceDirection dir, float maxHealth, Vector2 spd)
        {
            // default state
            defaultState = state;
            this.state = state;

            // store the enemy texture
            this.aniDict = aniDict;

            // store the enemys rectangle
            this.rec = rec;

            // store the start location
            startPos = new Vector2(rec.X, rec.Y);

            // store the enemy direction
            startDir = dir;
            this.dir = dir;

            // set up spd (should be velocity)
            if (dir == FaceDirection.Right)
            {
                enemySpd = spd;
            }
            else
            {
                enemySpd = new Vector2(-1f * spd.X, spd.Y);
            }

            // store the enemy health
            this.maxHealth = maxHealth;
            currHealth = maxHealth;
        }

        // turn the enemy around
        public void TurnArround()
        {
            enemySpd.X *= -1f;
            if (dir == FaceDirection.Left)
            {
                dir = FaceDirection.Right;
            }
            else
            {
                dir = FaceDirection.Left;
            }
        }

        // resets the enemy
        public void Reset()
        {
            // reset enemy state
            state = defaultState;

            // reset all animtions
            foreach (KeyValuePair<byte, Animation> ele in aniDict)
            {
                Animation ani = ele.Value;

                // reset the animation frame, and isAnimating
                ani.isAnimating = true;
                ani.curFrame = 0;
            }

            // reset the enemy location
            pos = startPos;

            // reset the enemy health
            currHealth = maxHealth;

            // reset the facing direction
            dir = startDir;
        }

        // update the enemy
        public void UpdatePos()
        {
            pos += enemySpd;
        }

        public void UpdateAnim(GameTime gameTime)
        {
            aniDict[state].Update(gameTime);
        }

        // draw the enemy
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // temporary flip variable
            SpriteEffects animFlip;
            if (dir == FaceDirection.Left)
            {
                animFlip = SpriteEffects.FlipHorizontally;
            }
            else
            {
                animFlip = SpriteEffects.None;
            }

            // draw the enemy
            aniDict[state].Draw(spriteBatch, Color.White, animFlip);

            spriteBatch.End();
        }


    }
}
