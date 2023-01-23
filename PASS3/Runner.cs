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
    
    // inheritance the enemy class
    class Runner : Enemy
    {
        // runner states
        private const byte DEAD = 0;
        private const byte HURT = 1;
        private const byte WALK = 2;

        // enemy body parts
        private const int FEET = 0;
        private const int LEFT = 1;
        private const int RIGHT = 2;
        private const int LEFT_GROUND_CHECKER = 3;
        private const int RIGHT_GROUDN_CHECKER = 4;

        // store the force of gravity
        private const float GRAV_ACCEL = 3f;

        // animtion scale
        private const int SCALE = 1;

        // death animation ofeset
        private readonly Vector2 OFSET = new Vector2(15, 0);

        // runner reward
        private const int REWARD = 100;

        // local content manger and Graphics Device
        ContentManager content;
        GraphicsDevice graphicsDevice;

        // runner current state
        //private EnemyState state;

        // runner damage
        private float atkDamage = 25;

        // runner health
        private float maxHealth;
        private float health;

        // runner animation vars
        private Texture2D[] runnerImgs = new Texture2D[3];
        private Animation[] runnerAnim = new Animation[3];

        // create runner's sub hitboxes
        private Rectangle[] recs = new Rectangle[5];

        // create runner's debug sub hitboxes
        private GameRectangle[] visiableRecs = new GameRectangle[5];

        // create animation rec
        private Rectangle animRec;

        // create runner's speed
        private Vector2 spd;

        // hitbox debug swtich
        private bool showCollisionvisibleRecs = false;

        // store if the runner is on the ground
        private bool isGround;

        // store if the runner is a faller (fall off platforms)
        private bool isFaller = false;

        // change the isFaller var
        public bool SetFaller
        {
            set { isFaller = value; }
        }

        // get the damage of the mob
        public float GetDamage
        {
            get { return atkDamage; }
        }

        public Runner(ContentManager content, GraphicsDevice graphicsDevice, EnemyState state, Vector2 spawn, 
                      FaceDirection dir, float maxHealth, Vector2 spd): 
                      base(content, graphicsDevice, state, spawn, dir, maxHealth, spd)
        {
            this.content = content;

            this.graphicsDevice = graphicsDevice;
            
            this.state = state;
             
            startPos = spawn;

            if (dir == FaceDirection.Right)
            {
                enemySpd = spd;
            }
            else
            {
                enemySpd = new Vector2(-1 * spd.X, spd.Y);
            }
        }

        public void LoadRunner()
        {
            // load the runner's images
            runnerImgs[WALK] = content.Load<Texture2D>("Animations/Enemies/Runner/Walk");
            runnerImgs[DEAD] = content.Load<Texture2D>("Animations/Enemies/Runner/Death");
            runnerImgs[HURT] = content.Load<Texture2D>("Animations/Enemies/Runner/Hurt");

            runnerAnim[WALK] = new Animation(runnerImgs[WALK], 4, 1, 4, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 5, pos, SCALE, true);
            runnerAnim[DEAD] = new Animation(runnerImgs[DEAD], 1, 4, 4, 0, Animation.NO_IDLE, Animation.ANIMATE_ONCE, 10, pos, SCALE, true);
            runnerAnim[HURT] = new Animation(runnerImgs[HURT], 1, 2, 2, 0, Animation.NO_IDLE, Animation.ANIMATE_ONCE, 6, pos, SCALE, true);

            // load the runner's spawn point
            pos = startPos;

            // set rec size to animtion dest rec size
            rec = runnerAnim[WALK].destRec;

            //rec = new Rectangle(animRec.Bottom, animRec.Left, SCALE * 72, SCALE * 72);
            rec.X = (int)pos.X;
            rec.Y = (int)pos.Y;
        }

        public void LoadSubRecs()
        {
            // Loc (x, y): top right of general rectangle
            // Size (width, height): 50% of width, 90% of height
            recs[LEFT] = new Rectangle(rec.X, rec.Y, (int)(rec.Width * 0.5f), (int)(rec.Height * 0.9f));

            // Loc (x, y): RIGHT of LEFT rectangle
            // Size (width, height): same as LEFT rectangle
            recs[RIGHT] = new Rectangle(recs[LEFT].X + recs[LEFT].Width, rec.Y, recs[LEFT].Width, recs[LEFT].Height);

            // Loc (x, y): under LEFT rectangle, and in middle of general rec
            // Size (width, height): 90% of width, rest of space (height)
            recs[FEET] = new Rectangle(rec.X + rec.Width / 2 - (int)(rec.Width * 0.9f / 2), recs[LEFT].Y + recs[LEFT].Height,
                                (int)(rec.Width * 0.9f), rec.Height - recs[LEFT].Height);

            // load ground checker
            if (isFaller)
            {
                recs[LEFT_GROUND_CHECKER] = new Rectangle(rec.X, rec.Y, recs[LEFT].Width, rec.Height);

                recs[RIGHT_GROUDN_CHECKER] = new Rectangle(recs[LEFT].X, rec.Y, recs[RIGHT].Width, rec.Height);
            }
        }

        public void Update(GameTime gameTime, Tile[,] tiles)
        {

            pos.X += enemySpd.X;

            pos.Y += GRAV_ACCEL;

            rec.X = (int)pos.X;
            rec.Y = (int)pos.Y;

            // if dead do the ofset
            if (state == DEAD)
            {
                pos.X = recs[(int)defaultState].X - OFSET.X;
                pos.Y = recs[(int)defaultState].Y - OFSET.Y;
            }
            else
            {
                UpdateTileCollision(tiles);

            }

            UpdateAnimRec(gameTime);

            if (showCollisionvisibleRecs)
            {
                visiableRecs[FEET] = new GameRectangle(graphicsDevice, recs[FEET]);
                visiableRecs[LEFT] = new GameRectangle(graphicsDevice, recs[LEFT]);
                visiableRecs[RIGHT] = new GameRectangle(graphicsDevice, recs[RIGHT]);
            }
        }

        // check if runner has collision with tiles
        public void UpdateTileCollision(Tile[,] tiles)
        {
            // store the size of the 2D array tile
            int width = tiles.GetLength(0);
            int height = tiles.GetLength(1);

            isGround = false;
            // Loop over every tile position
            for (int col = 0; col < width; col++)
            {
                for (int row = 0; row < height; row++)
                {
                    bool collision = false;

                    if (tiles[col, row].GetCollision != TileCollision.Passable)
                    {
                        Rectangle tileRec = tiles[col, row].GetRec;

                        // stage 1: check collision with general rectangle
                        if (rec.Intersects(tileRec))
                        {
                            LoadSubRecs();

                            // stage 2: check collision with sub hitboxes
                            if (recs[LEFT].Intersects(tileRec))
                            {
                                // runner turns around
                                TurnArround();

                                // move the enemy to the edge of the tile
                                rec.X = tileRec.X + tileRec.Width;
                                pos.X = rec.X;

                                // change state
                                if (state != EnemyState.Hurt)
                                {
                                    state = (EnemyState)WALK;
                                }

                            }

                            if (recs[RIGHT].Intersects(tileRec))
                            {
                                // runner turns around
                                TurnArround();

                                // move the enemy to the edge of the tile
                                rec.X = tileRec.X - rec.Width;
                                pos.X = rec.X;

                                // change state
                                if (state != EnemyState.Hurt )
                                {
                                    state = (EnemyState)WALK;
                                }
                            }

                            if (recs[FEET].Intersects(tileRec))
                            {
                                // change isGround to true
                                isGround = true;


                                rec.Y = tileRec.Y - rec.Height;
                                pos.Y = rec.Y;
                                enemySpd.Y = 0f;
                            }
                        }
                    }
                }
            }
        }

        // update the runner animation
        public void UpdateAnimRec(GameTime gameTime)
        {
            // update the player's animation draw location
            // NOTE: not the most effective, however if did anim[(int)state], there was a destrec glitch
            runnerAnim[DEAD].destRec.X = (int)pos.X;
            runnerAnim[DEAD].destRec.Y = (int)pos.Y;
            runnerAnim[HURT].destRec.X = (int)pos.X;
            runnerAnim[HURT].destRec.Y = (int)pos.Y;
            runnerAnim[WALK].destRec.X = (int)pos.X;
            runnerAnim[WALK].destRec.Y = (int)pos.Y;

            // update animtion
            runnerAnim[(int)state].Update(gameTime);

            if (state != DEAD)
            {
                runnerAnim[(int)state].destRec.Width = rec.Width;
                runnerAnim[(int)state].destRec.Height = rec.Height;
            }
         
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // temporary flip variable
            SpriteEffects animFlip;
            if (dir == FaceDirection.Right)
            {
                animFlip = SpriteEffects.FlipHorizontally;
            }
            else
            {
                animFlip = SpriteEffects.None;
            }

            switch (state)
            {
                case EnemyState.Active:
                    runnerAnim[WALK].Draw(spriteBatch, Color.White, animFlip);
                    break;
                case EnemyState.Dead:
                    runnerAnim[DEAD].Draw(spriteBatch, Color.White, animFlip);

                    if (!runnerAnim[DEAD].isAnimating)
                    {
                        IsDead = true;

                        MakeDead();
                    }

                    break;
                case EnemyState.Hurt:
                    runnerAnim[HURT].Draw(spriteBatch, Color.White, animFlip);

                    // reset the hurt animation
                    if (!runnerAnim[HURT].isAnimating)
                    {
                        state = defaultState;

                        runnerAnim[HURT].isAnimating = true;
                    }
                    break;
            }

            if (showCollisionvisibleRecs)
            {
                visiableRecs[LEFT].Draw(spriteBatch, Color.Red * 0.5f, true);
                visiableRecs[RIGHT].Draw(spriteBatch, Color.Blue * 0.5f, true);
                visiableRecs[FEET].Draw(spriteBatch, Color.Green * 0.5f, true);
            }
        }
        

        // returns teh reward for killing
        public override int GetReward()
        {
            return REWARD;
        }
    }
}
