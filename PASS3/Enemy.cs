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

        // states of the enemy
        public enum EnemeyState
        {
            Walk = 0,
            Dead = 1,
            Hurt = 2,
        }

        // store the local content manger
        ContentManager content;

        // used to view hitboxes
        GraphicsDevice graphicsDevice;

        // enemy state
        private EnemeyState defaultState;
        private EnemeyState state;

        // enemy rec
        protected Rectangle rec;

        // enemy location
        private Vector2 startPos;
        private Vector2 pos;

        // enemy spawn point
        private Vector2 spawn;

        // enemy spd
        protected Vector2 enemySpd;

        // enemy direction
        private FaceDirection startDir;
        protected FaceDirection dir;

        // enemy health variables
        private float maxHealth;
        private float currHealth;


        // set and get enemy state
        public EnemeyState EnemyState
        {
            set { state = value; }
            get { return state; }
        }

        // get location property
        public Vector2 GetLoc
        {
            get { return pos; }
        }

        // set and get enemy rec
        public Rectangle EnemyRec
        {
            set { rec = value; }
            get { return rec; }
        }


        // constructor for enemy
        public Enemy(ContentManager content, GraphicsDevice graphicsDevice, EnemeyState state, Vector2 spawn, FaceDirection dir, float maxHealth, Vector2 spd)
        {
            // pass the local content manger
            this.content = content;

            // pass the local graphics device
            this.graphicsDevice = graphicsDevice;

            // default state
            defaultState = state;
            this.state = state;

            // store the enemys spawn point
            this.spawn = spawn;

            // store the start location
            startPos = spawn;

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

            //// reset all animtions
            //foreach (KeyValuePair<EnemeyState, Animation> ele in aniDict)
            //{
            //    Animation ani = ele.Value;

            //    // reset the animation frame, and isAnimating
            //    ani.isAnimating = true;
            //    ani.curFrame = 0;
            //}

            // reset the enemy location
            pos = startPos;

            // reset the enemy health
            currHealth = maxHealth;

            // reset the facing direction
            dir = startDir;
        }

        //// update the enemy
        //public void UpdatePos()
        //{
        //    pos += enemySpd;
        //}

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }

    }
}
