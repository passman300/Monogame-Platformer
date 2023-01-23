using Animation2D;
using Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PASS3
{
    class Goblin : Enemy
    {
        // goblin states
        private const byte DEAD = 0;
        private const byte HURT = 1;
        private const byte SHOT = 2;

        // store the force of gravity
        private const float GRAV_ACCEL = 3f;

        // animtion scale
        private const float SCALE = 1.5f;

        // runner reward
        private const int REWARD = 250;

        // local content manger and Graphics Device
        ContentManager content;
        GraphicsDevice graphicsDevice;

        // goblin damage
        private float atkDamage = 25;

        // goblin health
        private float health;

        // goblin animation arrays
        private Texture2D[] goblinImgs = new Texture2D[3];
        private Animation[] goblinAnim = new Animation[3];

        // create goblin's sub hitboxes
        private Rectangle[] recs = new Rectangle[5];

        // fireBall animtion variables
        private Texture2D fireBallImg;

        // store goblin fireBalls
        private List<FireBall> fireBalls = new List<FireBall>();

        // fireBall shoting timer delay
        private Timer fireBallDelay = new Timer(1000, true);

        // flag for if the goblin is leveled with the ground
        private bool isGround = false;

        // get collsion damage
        public float GetDamage
        {
            get { return atkDamage; }
        }

        // returns a list of fireballs that the goblin owns
        public List<FireBall> GetFireBalls
        {
            get
            {
                if (fireBalls != null)
                {
                    return fireBalls;
                }

                else
                {
                    return new List<FireBall>();
                }
            }
        }


        public Goblin(ContentManager content, GraphicsDevice graphicsDevice, EnemyState state, Vector2 spawn,
                      FaceDirection dir, float maxHealth, Vector2 spd) :
                      base(content, graphicsDevice, state, spawn, dir, maxHealth, spd)
        {
            this.content = content;

            this.graphicsDevice = graphicsDevice;

            this.state = state;

            startPos = spawn;

            enemySpd = spd;

        }

        public void LoadGoblin()
        {
            // load the goblin's spawn point
            pos = startPos;

            // load the goblin's images
            goblinImgs[SHOT] = content.Load<Texture2D>("Animations/Enemies/Goblin/Idle");
            goblinImgs[DEAD] = content.Load<Texture2D>("Animations/Enemies/Goblin/Dead");
            goblinImgs[HURT] = content.Load<Texture2D>("Animations/Enemies/Goblin/Hurt");

            goblinAnim[SHOT] = new Animation(goblinImgs[SHOT], 2, 2, 3, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 5, pos, SCALE, true);
            goblinAnim[DEAD] = new Animation(goblinImgs[DEAD], 2, 3, 7, 0, Animation.NO_IDLE, Animation.ANIMATE_ONCE, 10, pos, SCALE, true);
            goblinAnim[HURT] = new Animation(goblinImgs[HURT], 2, 2, 3, 0, Animation.NO_IDLE, Animation.ANIMATE_ONCE, 2, pos, SCALE, true);

            // load fireBall sprite sheet
            fireBallImg = content.Load<Texture2D>("Animations/Enemies/Goblin/Fireball");

            // set rec size to animtion dest rec size
            rec = goblinAnim[SHOT].destRec;

            //rec = new Rectangle(animRec.Bottom, animRec.Left, SCALE * 72, SCALE * 72);
            rec.X = (int)pos.X;
            rec.Y = (int)pos.Y;

            //EnemyRec = rec;
        }

        public void Update(GameTime gameTime, Tile[,] tiles)
        {
            if (!IsDead)
            {
                UpdateAnim(gameTime);

                // update fireBall timer
                fireBallDelay.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
                //Console.WriteLine(fireBallDelay.GetTimePassedAsString(Timer.FORMAT_SEC_MIL));

                // if timer is done, add new fireBall
                if (fireBallDelay.IsFinished() && fireBalls.Count() < 10)
                {
                    fireBalls.Add(new FireBall(new Animation(fireBallImg, 2, 3, 5, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 3,
                                                new Vector2(rec.Center.X, rec.Center.Y - FireBall.size.Y / 2), 0.5f, true),
                                                new Vector2(rec.Center.X, rec.Center.Y - FireBall.size.Y / 2), dir));
                    fireBallDelay.ResetTimer(true);
                }

                // apply gravity until goblin is on a tile
                if (!isGround)
                {
                    pos.Y += GRAV_ACCEL;
                    rec.Y = (int)pos.Y;
                }
            }

            // always update fireballs until he is dead and no more fireballs to update
            //if (!IsDead && fireBalls.Count > 0)
            UpdateFireBalls(gameTime);

            UpdateCollision(gameTime, tiles);
        }

        // update collision between tiles and goblins and tiles and fireball
        private void UpdateCollision(GameTime gameTime, Tile[,] tiles)
        {
            // store the size of the 2d array tiles
            int height = tiles.GetLength(0);
            int width = tiles.GetLength(1);

            // loop over every tile position
            for (int col = 0; col < width; col++)
            {
                for (int row = 0; row < height; row++)
                {
                    if (tiles[row, col].GetCollision != TileCollision.Passable)
                    {
                        // check if there is collision with fireballs
                        for (int i = 0; i < fireBalls.Count() && i > -1; i++)
                        {
                            // remove fireball if it has a collision with tile
                            if (fireBalls[i].GetRec.Intersects(tiles[row, col].GetRec))
                            {
                                fireBalls.RemoveAt(i);

                                i--;
                            }
                        }
                        
                        if (!IsDead)
                        {
                            // check if there is collsion with goblin
                            if (rec.Intersects(tiles[row, col].GetRec) && !isGround)
                            {
                                // level the rec and set flag to true
                                rec.Y = tiles[row, col].GetRec.Y - rec.Height;
                                isGround = true;
                            }
                        }
                        

                    }
                }
            }
        }

        // update fire balls
        private void UpdateFireBalls(GameTime gameTime)
        {
            // update the pos of the fireBall (i is an fireball)
            for (int i = 0; i < fireBalls.Count(); i++)
            {
                fireBalls[i].Update(gameTime);
            }
        }

        // updates goblins animation
        private void UpdateAnim(GameTime gameTime)
        {
            if ((int)state == HURT)
            {
                Console.WriteLine(rec);
            }

            // have to update all state because of a tp dest rec error
            goblinAnim[DEAD].destRec.X = (int)pos.X;
            goblinAnim[DEAD].destRec.Y = (int)pos.Y;
            goblinAnim[HURT].destRec.X = (int)pos.X;
            goblinAnim[HURT].destRec.Y = (int)pos.Y;
            goblinAnim[SHOT].destRec.X = (int)pos.X;
            goblinAnim[SHOT].destRec.Y = (int)pos.Y;

            goblinAnim[(int)state].Update(gameTime);
        }

        // draw everything related to globlin, like itselfs and it's fireballs
        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawGoblin(spriteBatch);
            DrawFireBalls(spriteBatch);
        }

        // only draw the goblins animations
        private void DrawGoblin(SpriteBatch spriteBatch)
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

            // draw each stat
            switch (state)
            {
                case EnemyState.Active:
                    goblinAnim[SHOT].Draw(spriteBatch, Color.White, animFlip);
                    break;
                case EnemyState.Dead:
                    goblinAnim[DEAD].Draw(spriteBatch, Color.White, animFlip);

                    if (!goblinAnim[DEAD].isAnimating)
                    {
                        IsDead = true;

                        MakeDead();
                    }

                    break;
                case EnemyState.Hurt:
                    goblinAnim[HURT].Draw(spriteBatch, Color.White, animFlip);

                    if (!goblinAnim[HURT].isAnimating)
                    {
                        state = defaultState;

                        goblinAnim[HURT].curFrame = 0;
                        goblinAnim[HURT].isAnimating = true;
                    }
                    break;
            }

            GameRectangle temp = new GameRectangle(graphicsDevice, goblinAnim[(int)defaultState].destRec);
            temp.Draw(spriteBatch, Color.AliceBlue * 0.25f, true);
        }

        // draw each fireball that the goblin owns
        private void DrawFireBalls(SpriteBatch spriteBatch)
        {
            if (fireBalls != null)
            {
                for (int i = 0; i < fireBalls.Count(); i++)
                {
                    fireBalls[i].Draw(spriteBatch);
                }
            }

        }

        // returns the reward for killing
        public override int GetReward()
        {
            return REWARD;
        }
    }
}
