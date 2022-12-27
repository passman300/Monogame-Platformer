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
    class Enemy
    {
        // enum on where the enemy is facing
        public enum FaceDirection
        {
            Left = -1,
            Right = 1,
        }

        // type of enemy
        public enum EnemyType
        {
            Walker = 1,
            Ghast = 2,
        }

        // local content manger
        ContentManager content;

        // used to view hitboxes
        GraphicsDevice graphicsDevice;

        // enemy type
        private byte type;

        // enemy sprite
        private Texture2D enemyImg;

        // enemy location
        private Vector2 startPos;
        private Vector2 pos;

        // enemy rectangles
        private Rectangle animRec;
        private Rectangle rec;

        // enemy spd
        private Vector2 enemySpd;

        // enemy direction
        private FaceDirection startDir;
        private FaceDirection dir;

        // enemy health variables
        private float maxHealth;
        private float currHealth;

        // constructor for enemy
        public Enemy(Texture2D texture, Vector2 location, FaceDirection dir, float maxHealth, Vector2 spd)
        {
            // pass the enemy texture
            enemyImg = texture;

            // pass the start location
            startPos = location;
            
            // pass the enemy direction
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

            // pass the enemy health
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
            // reset the enemy location
            pos = startPos;

            // reset the enemy health
            currHealth = maxHealth;

            // reset the facing direction
            dir = startDir;
        }

        // update the enemy
        public void Update()
        {
            pos += enemySpd;
        }

        // draw the enemy
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();


            spriteBatch.Draw(enemyImg, rec, Color.White);
        }


    }
}
