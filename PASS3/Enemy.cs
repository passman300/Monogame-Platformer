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
        public enum EnemyState
        {
            Dead = 0,
            Hurt = 1,
            Active = 2,
        }

        // store the local content manger
        ContentManager content;

        // used to view hitboxes
        GraphicsDevice graphicsDevice;

        // enemy state
        protected EnemyState defaultState;
        protected EnemyState state;

        // enemy rec
        protected Rectangle rec;

        // enemy location
        protected Vector2 startPos;
        protected Vector2 pos;

        // enemy spd
        protected Vector2 enemySpd;

        // enemy direction
        private FaceDirection startDir;
        protected FaceDirection dir;

        // enemy health variables
        protected float maxHealth;
        protected float currHealth;

        // is dead flag
        private bool isDead = false;
        private bool scoreFlag = false;

        // set and get enemy state
        public EnemyState SetEnemyState
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

        // returns if dead
        public bool IsDead
        {
            set { isDead = value; }
            get { return isDead; }
        }

        // returns if the score has been claimed
        public bool IsReward
        {
            get { return !scoreFlag; }
            set { scoreFlag = value; }
        }

        // constructor for enemy
        public Enemy(ContentManager content, GraphicsDevice graphicsDevice, EnemyState state, Vector2 spawn, FaceDirection dir, float maxHealth, Vector2 spd)
        {
            // pass the local content manger
            this.content = content;

            // pass the local graphics device
            this.graphicsDevice = graphicsDevice;

            // default state
            defaultState = state;
            this.state = state;

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

            // reset the enemy location
            pos = startPos;

            // reset the enemy health
            currHealth = maxHealth;

            // reset the facing direction
            dir = startDir;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }

        public void UpdateHealth(float damage)
        {
            currHealth -= damage;

            state = EnemyState.Hurt;

            if (currHealth <= 0)
            {
                state = EnemyState.Dead;
            }
        }

        // make dead enemy 
        public void MakeDead()
        {
            enemySpd = new Vector2(0, 0);
            pos = new Vector2(-9000, -9000);
            rec.X = (int)pos.X;
            rec.Y = (int)pos.Y;
        }

        // returns the reward of the enemy
        public virtual int GetReward()
        {
            return 0;
        }
    }
}
