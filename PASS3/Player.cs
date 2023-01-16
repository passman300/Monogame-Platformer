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
        private const byte FALL = 3;
        private const byte CROUCH = 4;
        private const byte ATK1 = 5;
        private const byte ATK2 = 6;
        private const byte ATK3 = 7;
        private const byte MAGIC = 8;
        private const byte HURT = 9;
        private const byte DEAD = 10;


        // player body parts
        private const int FEET = 0;
        private const int HEAD = 1;
        private const int LEFT = 2;
        private const int RIGHT = 3;

        // store the force of gravity
        private const float GRAV_ACCEL = 0.7f;

        // player's horizontal acceleration
        private const float ACCEL = 1.1f;

        // store the force of ground friction
        private const float FRICTION = ACCEL * 0.2f;

        // minimum speed of player when on ground
        private const float TOLERANCE = FRICTION * 0.9f;

        // player's default maxHealth
        private const int DEFAULT_MAX_HEALTH = 100;

        // animtion scale
        private const int SCALE = 3;

        // local content manger
        ContentManager content;

        // used to view hitboxes
        GraphicsDevice graphicsDevice;

        // player's sprites 
        private Texture2D[] playerImgs = new Texture2D[10];
        private Animation[] playerAnims = new Animation[10];

        private int playerWidth = 45;
        private int playerHeight = 60;

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
        private Vector2 animPos = Vector2.Zero;

        // player speed variables
        private Vector2 playerSpd;
        private float maxSpdX;
        private float maxFallSpx;

        // initial jump speed
        private float jumpSpd;

        // store the vertical vector of the player
        private int verticalVec;

        //player's currHealth variables
        private int maxHealth;
        private float currHealth;
        private float healthFactor;
        private bool isHeal;

        // if player is hit they will be immune for a 1 second
        private bool isImmune = false;
        private Timer immuneTime = new Timer(1000, false);

        // attack combo the player can do
        // the timer if a window which the user, is able to continue the combo
        private int atkCombo;
        private Timer atkComboTimer = new Timer(100, false);

        // player's score
        private int score;

        // player's health bar
        private Texture2D healthBarImg;
        private Vector2 healthBarMaxSize;
        private Rectangle healthBarRec;
        private Texture2D healthBarBoarderImg;
        private Rectangle healthBarBoarderRec;

        // store user input
        KeyboardState prevKb;
        KeyboardState kb;
        MouseState prevMouse;
        MouseState mouse;

        GameRectangle temp;
        GameRectangle temp2;

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
            maxFallSpx = 5.2f;
            jumpSpd = -13.2f;
            verticalVec = 0;

            // load health variables
            healthFactor = 0.2f;
            isHeal = false;
        }

        //load the given content of the player model
        public void LoadPlayer()
        {
            

            // load player animation sheets
            //playerImgs[IDLE] = content.Load<Texture2D>("Animations/Player/Idle");
            //playerImgs[WALK] = content.Load<Texture2D>("Animations/Player/Walk");
            //playerImgs[JUMP] = content.Load<Texture2D>("Animations/Player/Jump");
            //playerImgs[CROUCH] = content.Load<Texture2D>("Animations/Player/Idle");

            playerImgs[IDLE] = content.Load<Texture2D>("Animations/Player/Temp/IdleV2");
            playerImgs[WALK] = content.Load<Texture2D>("Animations/Player/Temp/WalkV2");
            playerImgs[JUMP] = content.Load<Texture2D>("Animations/Player/Temp/JumpV2");
            playerImgs[FALL] = content.Load<Texture2D>("Animations/Player/Temp/FallV2");
            playerImgs[CROUCH] = content.Load<Texture2D>("Animations/Player/Temp/CrouchV2");
            playerImgs[ATK1] = content.Load<Texture2D>("Animations/Player/Temp/Attack1V2");
            playerImgs[ATK2] = content.Load<Texture2D>("Animations/Player/Temp/Attack2V2");
            playerImgs[ATK3] = content.Load<Texture2D>("Animations/Player/Temp/Attack3V2");

           

            playerAnims[IDLE] = new Animation(playerImgs[IDLE], 2, 3, 6, 0, 0, Animation.ANIMATE_FOREVER, 5, animPos, SCALE, true);
            playerAnims[WALK] = new Animation(playerImgs[WALK], 2, 3, 6, 0, 0, Animation.ANIMATE_FOREVER, 5, animPos, SCALE, true);
            playerAnims[JUMP] = new Animation(playerImgs[JUMP], 1, 2, 2, 0, 0, Animation.ANIMATE_FOREVER, 4, animPos, SCALE, true);
            playerAnims[FALL] = new Animation(playerImgs[FALL], 1, 2, 2, 0, 0, Animation.ANIMATE_FOREVER, 4, animPos, SCALE, true);
            playerAnims[CROUCH] = new Animation(playerImgs[CROUCH], 2, 3, 6, 0, 0, Animation.ANIMATE_FOREVER, 5, animPos, SCALE, true);
            playerAnims[ATK1] = new Animation(playerImgs[ATK1], 2, 3, 5, 0, 0, Animation.ANIMATE_FOREVER, 5, animPos, SCALE, true);
            playerAnims[ATK2] = new Animation(playerImgs[ATK2], 2, 3, 5, 0, 0, Animation.ANIMATE_FOREVER, 5, animPos, SCALE, true);
            playerAnims[ATK3] = new Animation(playerImgs[ATK3], 2, 3, 5, 0, 0, Animation.ANIMATE_FOREVER, 5, animPos, SCALE, true);

            // load player's postion
            playerPos = spawnPoint;
            AnimToPlayerPos();

            maxHealth = DEFAULT_MAX_HEALTH;
            currHealth = maxHealth;

            // load player's sub-hitboxes
            LoadPlayerRecs();

            // load player's hud
            LoadPlayerHud();


        }

        // load the player's overall and 2 stage collision of rectangles
        private void LoadPlayerRecs()
        {
            // center player rec of the of the animation dest rec
            playerRec = new Rectangle((int)playerPos.X,
                                      (int)playerPos.Y,
                                      playerWidth, playerHeight);

            // Pos (x, y): horizontally centered and at the top of the player
            // Size (width, height): 60% of width, 86% of height
            playerRecs[HEAD] = new Rectangle(playerRec.X + (int)(playerRec.Width * 0.5f) - (int)(playerRec.Width * 0.43f),
                                            playerRec.Y,
                                            (int)(playerRec.Width * 0.86f),
                                            (int)(playerRec.Height * 0.10f));

            // Pos (x, y): LEFT of players horizontal axis, and under HEAD
            // Size (width, height): 50% of width, 75% of height
            playerRecs[LEFT] = new Rectangle(playerRec.X,
                                            playerRecs[HEAD].Y + playerRecs[HEAD].Height,
                                            (int)(playerRec.Width * 0.5f),
                                            (int)(playerRec.Height * 0.5f));

            // Pos (x, y): RIGHT of LEFT rectangle, and under HEAD
            // Size (width, height): 45% of width, 25% of height
            playerRecs[RIGHT] = new Rectangle(playerRecs[LEFT].X + playerRecs[LEFT].Width,
                                             playerRecs[HEAD].Y + playerRecs[HEAD].Height,
                                             (int)(playerRec.Width * 0.5f),
                                             (int)(playerRec.Height * 0.5f));

            // Pos (x, y): horizontally centered and under LEFT rec
            // Size (width, height): 70% of width, and what space is left of the player rec
            playerRecs[FEET] = new Rectangle(playerRec.X + (int)(playerRec.Width * 0.5f) - (int)(playerRec.Width * 0.3f),
                                            playerRecs[LEFT].Y + playerRecs[LEFT].Height,
                                            (int)(playerRec.Width * 0.6f),
                                            playerRec.Height - (playerRecs[HEAD].Height + playerRecs[LEFT].Height));


            //Now setup the visible recs when necessary
            if (showCollisionRecs)
            {
                temp = new GameRectangle(graphicsDevice, playerRec);
                temp2 = new GameRectangle(graphicsDevice, playerAnims[playerState].destRec);

                playerVisibleRecs[HEAD] = new GameRectangle(graphicsDevice, playerRecs[HEAD]);
                playerVisibleRecs[LEFT] = new GameRectangle(graphicsDevice, playerRecs[LEFT]);
                playerVisibleRecs[RIGHT] = new GameRectangle(graphicsDevice, playerRecs[RIGHT]);
                playerVisibleRecs[FEET] = new GameRectangle(graphicsDevice, playerRecs[FEET]);
            }
        }

        private void LoadPlayerHud()
        {
            // load player's health bar
            healthBarImg = content.Load<Texture2D>("Images/Player/HealthBar");

            healthBarMaxSize = new Vector2(4 * maxHealth, 20);
            healthBarRec = new Rectangle(30, 30, (int)healthBarMaxSize.X, (int)healthBarMaxSize.Y);

            // load player's health bar boarder
            healthBarBoarderImg = content.Load<Texture2D>("Images/Player/HealthBarBoarder");
            healthBarBoarderRec = new Rectangle(30, 30, healthBarRec.Width, healthBarRec.Height);
        }

        // update player's animation rectangles relative to the player true location
        private void UpdatePlayerAnimRec(GameTime gameTime)
        {
            AnimToPlayerPos();
            
            // update the player's animation
            playerAnims[playerState].Update(gameTime);

            // update the player's animation draw location
            playerAnims[playerState].destRec.X = (int)animPos.X + 2;
            playerAnims[playerState].destRec.Y = (int)animPos.Y + 2;

            //playerAnims[playerState].destRec.Width = playerRec.Width;
            //playerAnims[playerState].destRec.Height = playerRec.Height;
        }

        // update all player properties. Like a manager
        public void Update(GameTime gameTime, Tile[,] tiles, Enemy[,] enemies)
        {
            UpdatePlayer(gameTime, tiles, enemies);
            UpdateHealthBar();
        }

        // update player's movement, and states
        private void UpdatePlayer(GameTime gametime, Tile[,] tiles, Enemy[,] enemies)
        {
            
            prevKb = kb;
            kb = Keyboard.GetState();

            prevMouse = mouse;
            mouse = Mouse.GetState();

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

            // player movement region
            #region
            // change user with respect to player input
            if (kb.IsKeyDown(Keys.A))
            {
                // update the player states
                playerState = isGround ? WALK : JUMP;
                playerDir = FaceDirection.Left;
                isIdle = false;

                // update player speed
                playerSpd.X -= ACCEL;
                playerSpd.X = MathHelper.Clamp(playerSpd.X, -maxSpdX, maxSpdX);
            }
            else if (kb.IsKeyDown(Keys.D))
            {
                // update the player states
                playerState = isGround ? WALK : JUMP;
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
            if (kb.IsKeyDown(Keys.S))
            {
                // update player state
                playerState = CROUCH;

                if (!isHeal)
                {
                    // update the player health
                    currHealth *= 1 + healthFactor;

                    // change player flags
                    isHeal = true;
                    isIdle = false;
                }
            }
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {

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
            playerSpd.Y = MathHelper.Clamp(playerSpd.Y, jumpSpd, maxFallSpx);


            // add player's speed to their position
            playerPos.X += playerSpd.X;
            playerPos.Y += playerSpd.Y;

            // update the player's rectangle to the new player position
            playerRec.X = (int)playerPos.X;
            playerRec.Y = (int)playerPos.Y;
            #endregion 

            // check if the player is isImmune
            if (isImmune)
            {
                // if the player is immune then don't check enemy collision
                // instead update immune timer
                immuneTime.Update(gametime.ElapsedGameTime.TotalMilliseconds);

                // check if timer is over
                if (immuneTime.IsFinished())
                {
                    // set immune flag to false and reset timer
                    isImmune = false;
                    immuneTime.ResetTimer(false);
                }
            }
            // if not immune check enemy collsion
            else
            {
                // check enemy collisons
                UpdateEnemyCollision(enemies);
            }
            // check tiles collisions
            UpdateTileCollision(tiles);

            // update the player's animation
            UpdatePlayerAnimRec(gametime);

            LoadPlayerRecs();
        }

        private void UpdateHealthBar()
        {
            healthBarRec.Width = (int)(healthBarMaxSize.X * (currHealth / maxHealth));
        }

        // check if player has collision with tiles
        private void UpdateTileCollision(Tile[,] tiles)
        {
            // store the size of the 2D array tiles
            int height = tiles.GetLength(0);
            int width = tiles.GetLength(1);

            bool tempGround = false;

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

                                        LoadPlayerRecs();
                                    }
                                }
                                // only check head if vertical vector is going up and no ground
                                else if (verticalVec == 1 && !tempGround)
                                {
                                    if (playerRecs[HEAD].Intersects(tileRec))
                                    {
                                        playerRec.Y = tileRec.Y + tileRec.Height;
                                        playerPos.Y = playerRec.Y;
                                        playerSpd.Y = GRAV_ACCEL;
                                        collision = true;

                                        LoadPlayerRecs();
                                    }
                                }

                                // check other sub-hitboxes
                                if (playerRecs[LEFT].Intersects(tileRec))
                                {
                                    playerRec.X = tileRec.X + tileRec.Width;
                                    playerPos.X = playerRec.X;
                                    playerSpd.X = 0f;
                                    collision = true;

                                    LoadPlayerRecs();
                                }
                                else if (playerRecs[RIGHT].Intersects(tileRec))
                                {
                                    playerRec.X = tileRec.X - playerRec.Width;
                                    playerPos.X = playerRec.X;
                                    playerSpd.X = 0f;
                                    collision = true;

                                    LoadPlayerRecs();
                                }
                            }
                        }
                    }
                }
            }
        }

        // check if player has collision with enemy
        private void UpdateEnemyCollision(Enemy[,] enemies)
        {
            // store the size of the 2D array enemies
            int height = enemies.GetLength(0);
            int width = enemies.GetLength(1);

            for (int col = 0; col < width; col++)
            {
                for (int row = 0; row < height; row++)
                {
                    bool collision = false;

                    if (enemies[row, col] != null)
                    {
                        // check if there is intersection with enemy
                        if (playerRec.Intersects(enemies[row, col].EnemyRec))
                        {
                            // check what type of enemy
                            if (enemies[row, col] is Runner)
                            {
                                currHealth -= ((Runner)(enemies[row, col])).GetDamage;

                                isImmune = true;
                                immuneTime.Activate();
                            }
                        }
                    }
                }
            }
        }


        // draw all of player's properties
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            DrawPlayer(spriteBatch);
            DrawHud(spriteBatch);
            
            spriteBatch.End();

        }

        // draw only the player
        private void DrawPlayer(SpriteBatch spriteBatch)
        {
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
                temp.Draw(spriteBatch, Color.Beige * 0.7f, true);
                temp2.Draw(spriteBatch, Color.CornflowerBlue * 0.7f, true);

                playerVisibleRecs[HEAD].Draw(spriteBatch, Color.Yellow * 0.5f, true);
                playerVisibleRecs[LEFT].Draw(spriteBatch, Color.Red * 0.5f, true);
                playerVisibleRecs[RIGHT].Draw(spriteBatch, Color.Blue * 0.5f, true);
                playerVisibleRecs[FEET].Draw(spriteBatch, Color.Green * 0.5f, true);
            }
        }

        // draw only the hud
        private void DrawHud(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(healthBarImg, healthBarRec, Color.White);
            spriteBatch.Draw(healthBarBoarderImg, healthBarBoarderRec, Color.White);
        }


        private void AnimToPlayerPos()
        {
            animPos.X = playerPos.X - ((playerAnims[playerState].destRec.Width) - (playerWidth)) / 2 + 2;
            animPos.Y = playerPos.Y - ((playerAnims[playerState].destRec.Height)- (playerHeight)) / 2 + 2;
        }
    }
}
