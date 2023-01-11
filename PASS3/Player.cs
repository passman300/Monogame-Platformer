using Animation2D;
using Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace PASS3
{
    public class Player
    {
        // enum for the player's direction
        enum FaceDirection
        {
            Left = -1,
            Right = 1,
        }

        // player states
        private const byte IDLE = 0;
        private const byte WALK = 1;
        private const byte JUMP = 2;
        private const byte CROUCH = 3;
        private const byte DEAD = 4;

        // player body parts
        private const int FEET = 0;
        private const int HEAD = 1;
        private const int LEFT = 2;
        private const int RIGHT = 3;

        // store the force of gravity
        private const float GRAV_ACCEL = 8f / 60;

        // player's horizontal acceleration
        private const float ACCEL = 1.1f;

        // store the force of ground friction
        private const float FRICTION = ACCEL * 0.8f;

        // minimum speed of player when on ground
        private const float TOLERANCE = FRICTION * 0.9f;

        // local content manger
        ContentManager content;

        // used to view hitboxes
        GraphicsDevice graphicsDevice;

        // player's sprites 
        private Texture2D[] playerImgs = new Texture2D[5];
        private Animation[] playerAnims = new Animation[5];

        // player general hitbox, and sub hitboxes
        private Rectangle playerRec;
        private Rectangle[] playerRecs = new Rectangle[4];

        //Track the 4 visible versions of the player's collision recs (for testing display purposes)
        GameRectangle[] playerVisibleRecs = new GameRectangle[4];

        //Specify whether the collision rectangles should be displayed (for testing purposes)
        bool showCollisionRecs = false;

        // store player states
        private byte playerState;
        private FaceDirection playerDir;
        private bool isGround;
        private bool isIdle;

        // player's location
        private Vector2 spawnPoint;
        private Vector2 playerPos;

        // player speed variables
        private Vector2 playerSpd;
        private float maxSpdX;

        // initial jump speed
        private float jumpSpd;

        // store the vertical vector of the player
        private int verticalVec;

        //player's currHealth variables
        private int maxHealth;
        private float currHealth;
        private float healthFactor;
        private bool isHeal;

        // player's score
        private int score;

        // store user input
        KeyboardState prevKb;
        KeyboardState kb;

        public int SetMaxHealth
        {
            set { maxHealth = value; }
        }

        public Vector2 SetSpawnPoint
        {
            set { spawnPoint = value; }
        }

        // player constructor
        public Player(ContentManager content, GraphicsDevice graphicsDevice)
        {
            // pass content manager
            this.content = content;

            // pass the GraphicsDevice
            this.graphicsDevice = graphicsDevice;

            // load player states
            isGround = false;
            isIdle = true;

            // load speed variables
            playerSpd = Vector2.Zero;
            maxSpdX = 3f;
            jumpSpd = -5.65f;
            verticalVec = 0;

            // load health variables
            healthFactor = 0.2f;
            isHeal = false;
        }

        //load the given content of the player model
        public void LoadPlayer()
        {
            // load player animation sheets
            playerImgs[IDLE] = content.Load<Texture2D>("Animations/Player/Idle");
            playerImgs[WALK] = content.Load<Texture2D>("Animations/Player/Walk");
            playerImgs[JUMP] = content.Load<Texture2D>("Animations/Player/Jump");
            playerImgs[CROUCH] = content.Load<Texture2D>("Animations/Player/Idle");

            // load player animations
            playerAnims[IDLE] = new Animation(playerImgs[IDLE], 2, 3, 5, 0, 0, Animation.ANIMATE_FOREVER, 5, playerPos, 2, true);
            playerAnims[WALK] = new Animation(playerImgs[WALK], 3, 2, 6, 0, 0, Animation.ANIMATE_FOREVER, 5, playerPos, 2, true);
            playerAnims[JUMP] = new Animation(playerImgs[JUMP], 2, 2, 3, 0, 0, Animation.ANIMATE_ONCE, 4, playerPos, 2, true);
            playerAnims[CROUCH] = new Animation(playerImgs[CROUCH], 4, 1, 4, 0, 0, Animation.ANIMATE_FOREVER, 5, playerPos, 2, true);

            playerPos = spawnPoint;

            LoadPlayerRecs();
        }

        // load the player's overall and 2 stage collision of rectangles
        private void LoadPlayerRecs()
        {
            // store the player's width and height temporarily
            int playerWidth;
            int playerHeight;

            playerHeight = playerAnims[playerState].destRec.Height;
            playerWidth = playerAnims[playerState].destRec.Width;
            playerRec = new Rectangle((int)playerPos.X, (int)playerPos.Y, playerWidth, playerHeight);

            // Loc (x, y): horizontally centered and at the top of the player
            // Size (width, height): 60% of width, 86% of height
            playerRecs[HEAD] = new Rectangle(playerRec.X + (int)(playerRec.Width * 0.5f) - (int)(playerRec.Width * 0.43f),
                                            playerRec.Y,
                                            (int)(playerRec.Width * 0.86f), 
                                            (int)(playerRec.Height * 0.10f));

            // Loc (x, y): LEFT of players horizontal axis, and under HEAD
            // Size (width, height): 50% of width, 75% of height
            playerRecs[LEFT] = new Rectangle(playerRec.X,
                                            playerRecs[HEAD].Y + playerRecs[HEAD].Height,
                                            (int)(playerRec.Width * 0.5f), 
                                            (int)(playerRec.Height * 0.75f));

            // Loc (x, y): RIGHT of LEFT rectangle, and under HEAD
            // Size (width, height): 45% of width, 25% of height
            playerRecs[RIGHT] = new Rectangle(playerRecs[LEFT].X + playerRecs[LEFT].Width, 
                                             playerRecs[HEAD].Y + playerRecs[HEAD].Height,
                                             (int)(playerRec.Width * 0.5f), 
                                             (int)(playerRec.Height * 0.75f));

            // Loc (x, y): horizontally centered and under LEFT rec
            // Size (width, height): 70% of width, and what space is left of the player rec
            playerRecs[FEET] = new Rectangle(playerRec.X + (int)(playerRec.Width * 0.5f) - (int)(playerRec.Width * 0.3f),
                                            playerRecs[LEFT].Y + playerRecs[LEFT].Height,
                                            (int)(playerRec.Width * 0.6f),
                                            playerRec.Height - (playerRecs[HEAD].Height + playerRecs[LEFT].Height));


            //Now setup the visible recs when necessary
            if (showCollisionRecs)
            {
                playerVisibleRecs[HEAD] = new GameRectangle(graphicsDevice, playerRecs[HEAD]);
                playerVisibleRecs[LEFT] = new GameRectangle(graphicsDevice, playerRecs[LEFT]);
                playerVisibleRecs[RIGHT] = new GameRectangle(graphicsDevice, playerRecs[RIGHT]);
                playerVisibleRecs[FEET] = new GameRectangle(graphicsDevice, playerRecs[FEET]);
            }
        }

        private void UpdatePlayerAnimRec(GameTime gameTime)
        {
            // update the player's animation
            playerAnims[playerState].Update(gameTime);

            // update the player's animation draw location
            playerAnims[playerState].destRec.X = (int)playerPos.X;
            playerAnims[playerState].destRec.Y = (int)playerPos.Y;

            playerAnims[playerState].destRec.Width = playerRec.Width;
            playerAnims[playerState].destRec.Height = playerRec.Height;
        }

        // update player
        public void Update(GameTime gametime, Tile[,] tiles)
        {
            prevKb = kb;
            kb = Keyboard.GetState();

            //Toggle the visibility of the player's collision rectangles for testing
            if (kb.IsKeyDown(Keys.D1) && !prevKb.IsKeyDown(Keys.D1))
            {
                showCollisionRecs = !showCollisionRecs;
            }

            if (isGround)
            {
                isIdle = true;
                playerState = IDLE;
            }
            

            // change user with respect to player input
            if (kb.IsKeyDown(Keys.A))
            {
                // update the player states
                playerState = WALK;
                playerDir = FaceDirection.Left;
                isIdle = false;

                // update player speed
                playerSpd.X -= ACCEL;
                playerSpd.X = MathHelper.Clamp(playerSpd.X, -maxSpdX, maxSpdX);

                
            }
            else if (kb.IsKeyDown(Keys.D))
            {
                // update the player states
                playerState = WALK;
                playerDir = FaceDirection.Right;
                isIdle = false;

                // update player speed
                playerSpd.X += ACCEL;
                playerSpd.X = MathHelper.Clamp(playerSpd.X, -maxSpdX, maxSpdX);

            }
            
            if (kb.IsKeyDown(Keys.Space) && isGround)
            {
                // update player state
                playerState = JUMP;
                isGround = false;
                isIdle = false;

                // reset jump animation
                playerAnims[JUMP].isAnimating = true;
                playerAnims[JUMP].curFrame = 0;

                verticalVec = 1;

                playerSpd.Y = jumpSpd;
            }
            if (kb.IsKeyDown(Keys.S) && !isHeal)
            {
                // update player state
                playerState = CROUCH;

                // update the player health
                currHealth *= 1 + healthFactor;

                // change player flags
                isHeal = true;
                isIdle = false;
            }
            // adjust players speed by ground effects
            if (isGround)
            {
                // decelerate the players x speed (change by a force of the opposite direction), by the forces of friction
                playerSpd.X += -Math.Sign(playerSpd.X) * FRICTION;

                // reset the players x speed, if bellow the tolerance
                if (Math.Abs(playerSpd.X) <= TOLERANCE)
                {
                    playerSpd.X = 0f;
                }
            }

            // check if the player has peaked in their jump
            if (!isGround && playerSpd.Y > 0)
            {
                // change the vector to face down
                verticalVec = -1;
            }
            else if (!isGround && playerSpd.Y < 0)
            {
                // change the vector to face up
                verticalVec = 1;
            }
            else if (isGround)
            {
                verticalVec = 0;
            }

            //add gravity to the player's y speed
            playerSpd.Y += GRAV_ACCEL;

            // add player's speed to their position
            playerPos.X += playerSpd.X;
            playerPos.Y += playerSpd.Y;

            Console.WriteLine(playerPos.ToString());

            // update the player's rectangle to the new player position
            playerRec.X = (int)playerPos.X;
            playerRec.Y = (int)playerPos.Y;

            // check collisions again
            UpdateCollision(tiles);

            // update the player's animation
            UpdatePlayerAnimRec(gametime);

            LoadPlayerRecs();
            
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // temporary flip variable
            SpriteEffects animFlip;
            if (playerDir == FaceDirection.Left)
            {
                animFlip = SpriteEffects.FlipHorizontally;   
            }
            else
            {
                animFlip = SpriteEffects.None;
            }

            switch (playerState)
            {
                case IDLE:
                    playerAnims[IDLE].Draw(spriteBatch, Color.White, animFlip);
                    break;
                case WALK:
                    playerAnims[WALK].Draw(spriteBatch, Color.White, animFlip);
                    break;
                case JUMP:
                    playerAnims[JUMP].Draw(spriteBatch, Color.White, animFlip);
                    break;
                case CROUCH:
                    playerAnims[CROUCH].Draw(spriteBatch, Color.White, animFlip);
                    break;
                case DEAD:
                    break;
            }


            if (showCollisionRecs)
            {
                playerVisibleRecs[HEAD].Draw(spriteBatch, Color.Yellow * 0.5f, true);
                playerVisibleRecs[LEFT].Draw(spriteBatch, Color.Red * 0.5f, true);
                playerVisibleRecs[RIGHT].Draw(spriteBatch, Color.Blue * 0.5f, true);
                playerVisibleRecs[FEET].Draw(spriteBatch, Color.Green * 0.5f, true);
            }

            spriteBatch.End();
        }

        // check if player has collision
        private void UpdateCollision(Tile[,] tiles)
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

                    if (tiles[row, col] != null)
                    {
                        if (tiles[row, col].GetCollision != TileCollision.Passable)
                        {
                            Rectangle tileRec = tiles[row, col].GetRec;

                            // stage 1: check collision with general rectangle
                            if (playerRec.Intersects(tileRec))
                            {
                                LoadPlayerRecs();

                                // stage 2: check collision with player parts
                                // only check feet collision if vertical vector is going down or isGround
                                if (verticalVec == -1 || verticalVec == 0)
                                {
                                    if (playerRecs[FEET].Intersects(tileRec))
                                    {
                                        playerRec.Y = tileRec.Y - playerRec.Height;
                                        playerPos.Y = playerRec.Y;
                                        playerSpd.Y = 0f;
                                        isGround = true;
                                    }
                                }
                                // only check head if vertical vector is going up and no ground
                                else if (verticalVec == 1 && !isGround)
                                {
                                    if (playerRecs[HEAD].Intersects(tileRec))
                                    {
                                        playerRec.Y = tileRec.Y + tileRec.Height;
                                        playerPos.Y = playerRec.Y;
                                        playerSpd.Y = playerSpd.Y * -1;
                                        collision = true;
                                    }
                                }

                                // check other sub-hitboxes
                                if (playerRecs[LEFT].Intersects(tileRec))
                                {
                                    playerRec.X = tileRec.X + tileRec.Width;
                                    playerPos.X = playerRec.X;
                                    playerSpd.X = 0f; // or multiply by -1 to 
                                    collision = true;
                                }
                                if (playerRecs[RIGHT].Intersects(tileRec))
                                {
                                    playerRec.X = tileRec.X - playerRec.Width;
                                    playerPos.X = playerRec.X;
                                    playerSpd.X = 0f; // or multiply by -1 to bounce
                                    collision = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        // check if 
    }
}
