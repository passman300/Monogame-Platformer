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
        private const byte WALK = 0;
        private const byte DEAD = 1;
        private const byte HURT = 2;

        // enemy body parts
        private const int FEET = 0;
        private const int LEFT = 1;
        private const int RIGHT = 2;

        // store the force of gravity
        private const float GRAV_ACCEL = 8f / 60;

        // local content manger and Graphics Device
        ContentManager content;
        GraphicsDevice graphicsDevice;

        // runner current state
        private EnemeyState state;

        // runner direction
        FaceDirection dir;

        // runner animation arrays
        private Texture2D[] runnerImgs = new Texture2D[3];
        private Animation[] runnerAnim = new Animation[3];

        // runner spawn point
        private Vector2 spawnPoint;

        // runner location
        private Vector2 pos;

        // runner's general hitbox
        private Rectangle rec;

        // create runner's sub hitboxes
        private Rectangle[] recs = new Rectangle[3];

        // create runner's debug sub hitboxes
        private GameRectangle[] visiableRecs = new GameRectangle[3];

        // create runner's speed
        private Vector2 spd;

        // hitbox debug swtich
        private bool showCollisionvisibleRecs = false;

        // store if the runner is on the ground
        private bool isGround;

        public Runner(ContentManager content, GraphicsDevice graphicsDevice, EnemeyState state, Vector2 spawn, 
                      FaceDirection dir, float maxHealth, Vector2 spd): 
                      base(content, graphicsDevice, state, spawn, dir, maxHealth, spd)
        {
            this.content = content;

            this.graphicsDevice = graphicsDevice;

            this.state = state;
             
            spawnPoint = spawn;

            this.dir = dir;

            this.spd = spd;
        }

        public void LoadRunner()
        {
            // load the runner's images
            runnerImgs[WALK] = content.Load<Texture2D>("Animations/Enemies/Runner/Walk");
            runnerImgs[DEAD] = content.Load<Texture2D>("Animations/Enemies/Runner/Death");
            runnerImgs[HURT] = content.Load<Texture2D>("Animations/Enemies/Runner/Hurt");

            runnerAnim[WALK] = new Animation(runnerImgs[WALK], 4, 1, 4, 1, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 5, pos, 3, true);
            runnerAnim[DEAD] = new Animation(runnerImgs[DEAD], 4, 1, 4, 1, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 5, pos, 3, true);
            runnerAnim[HURT] = new Animation(runnerImgs[HURT], 2, 1, 2, 1, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 5, pos, 3, true);

            // load the runner's spawn point
            pos = spawnPoint;

            // set rec size to animtion dest rec size
            rec = runnerAnim[WALK].destRec;
        }

        public void LoadvisibleRecs()
        {
            // Loc (x, y): top right of general rectangle
            // Size (width, height): 50% of width, 75% of height
            recs[LEFT] = new Rectangle(rec.X, rec.Y, (int)(rec.Width * 0.5f), (int)(rec.Height * 0.75f));

            // Loc (x, y): RIGHT of LEFT rectangle
            // Size (width, height): same as LEFT rectangle
            recs[RIGHT] = new Rectangle(recs[LEFT].X, rec.Y, recs[LEFT].Width, recs[LEFT].Height);

            // Loc (x, y): under LEFT rectangle, and in middle of general rec
            // Size (width, height): 25% of width, rest of space (height)
            recs[FEET] = new Rectangle(rec.X + rec.Width / 2 - (int)(rec.Width * 0.25f / 2), recs[LEFT].Y,
                                (int)(rec.Width * 0.25f), rec.Height - recs[LEFT].Height);
        }

        public void Update()
        {
            if (showCollisionvisibleRecs)
            {
                visiableRecs[WALK] = new GameRectangle(graphicsDevice, recs[WALK]);
                visiableRecs[LEFT] = new GameRectangle(graphicsDevice, recs[LEFT]);
                visiableRecs[RIGHT] = new GameRectangle(graphicsDevice, recs[RIGHT]);
            }
        }

        // check if runner has collision with tiles
        public void UpdateTileCollision(Tile[,] tiles)
        {
            // store the size of the 2D array tile
            int height = tiles.GetLength(0);
            int width = tiles.GetLength(1);

            isGround = false;
            // Loop over every tile position
            for (int col = 0; col < width; col++)
            {
                for (int row = 0; row < height; row++)
                {
                    bool collision = false;

                    if (tiles[row, col].GetCollision != TileCollision.Passable)
                    {
                        Rectangle tileRec = tiles[row, col].GetRec;

                        // stage 1: check collision with general rectangle
                        if (rec.Intersects(tileRec))
                        {
                            LoadvisibleRecs();

                            // stage 2: check collision with sub hitboxes
                            if (recs[LEFT].Intersects(tileRec) || recs[RIGHT].Intersects(tileRec))
                            {
                                // runner turns around
                                TurnArround();

                                // change state
                                state = WALK;
                            }
                            if (recs[FEET].Intersects(tileRec))
                            {
                                // change isGround to true
                                isGround = true;

                                // change state
                                state = WALK;
                            }
                        }
                    }
                }
            }
        }

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

            switch (state)
            {
                case EnemeyState.Walk:
                    runnerAnim[WALK].Draw(spriteBatch, Color.White, animFlip);
                    break;
                case EnemeyState.Dead:
                    runnerAnim[DEAD].Draw(spriteBatch, Color.White, animFlip);
                    break;
                case EnemeyState.Hurt:
                    runnerAnim[HURT].Draw(spriteBatch, Color.White, animFlip);
                    break;
            }

            if (showCollisionvisibleRecs)
            {
                visiableRecs[LEFT].Draw(spriteBatch, Color.Red * 0.5f, true);
                visiableRecs[RIGHT].Draw(spriteBatch, Color.Blue * 0.5f, true);
                visiableRecs[FEET].Draw(spriteBatch, Color.Green * 0.5f, true);
            }

            spriteBatch.End();
        }

    }
}
