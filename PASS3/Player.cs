using Animation2D;
using Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;


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

        // local random
        Random rng;

        // player's blood emitter
        BloodEmitterV2 bloodEmitter;
        Texture2D bloodParticleImg;

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

        // if player is hit they will be immune for a 1.5 second
        private bool isImmune = false;
        private Timer immuneTime = new Timer(1500, false);
        private Timer flashColorTimer = new Timer(150, false); // 2 milliseconds the player flashes 
        private bool flashFlag = false;

        // attack combo the player can do
        // the timer if a window which the user, is able to continue the combo
        private int atkCombo;
        private Timer atkComboTimer = new Timer(100, false);

        // player's score
        private int score;

        // players plood variable
        private int bloodDamage = 10;
        private float bloodFactor = 0.35f;

        // player's health bar
        private Texture2D healthBarImg;
        private Vector2 healthBarMaxSize;
        private Rectangle healthBarDefault;
        private Rectangle healthBarRec;
        private Texture2D healthBarBoarderImg;
        private Rectangle healthBarBoarderRec;

        // player's score counter
        private SpriteFont scoreFont;
        private Vector2 scorePos;

        // store user input
        KeyboardState prevKb;
        KeyboardState kb;
        MouseState prevMouse;
        MouseState mouse;

        private bool isNextLvl = false;

        GameRectangle temp;
        GameRectangle temp2;

        public int SetMaxHealth
        {
            set { maxHealth = value; }
        }

        // change the spawn location of the player
        public Vector2 SetSpawnPoint
        {
            set 
            { 
                spawnPoint = value;

                // to lazy to remake a another method
                LoadPlayer();
            }
        }

        // change the score of the player
        public int Score
        {
            set { score = value; }
            get { return score; }
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
            maxSpdX = 4f;
            maxFallSpx = 8.2f;
            jumpSpd = -13.2f;
            verticalVec = 0;

            // load health variables
            healthFactor = 0.0095f;
            isHeal = false;
        }

        //load the given content of the player model
        public void LoadPlayer()
        {
            // load images and animation of the player
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

            // load blood
            LoadBlood();
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

        // load's the hud
        private void LoadPlayerHud()
        {
            // load score font
            scoreFont = content.Load<SpriteFont>("Fonts/ScoreFont");
            scorePos = new Vector2(30, 70);

            // load player's health bar
            healthBarImg = content.Load<Texture2D>("Images/Player/HealthBar");

            healthBarMaxSize = new Vector2(4 * maxHealth, 20);
            healthBarDefault = new Rectangle(30, 30, (int)healthBarMaxSize.X, (int)healthBarMaxSize.Y);
            healthBarRec = healthBarDefault;

            // load player's health bar boarder
            healthBarBoarderImg = content.Load<Texture2D>("Images/Player/HealthBarBoarder");
            healthBarBoarderRec = new Rectangle(30, 30, healthBarRec.Width, healthBarRec.Height);
        }

        // load blood content and emitter
        private void LoadBlood()
        {
            bloodParticleImg = content.Load<Texture2D>("Images/Player/BloodParticle");

            bloodEmitter = new BloodEmitterV2(bloodParticleImg, new Rectangle(0, 0, 4, 4), new Vector2(4, 4), 1f, 200, 100f, 2);
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
        public void Update(GameTime gameTime, Tile[,] tiles, Enemy[,] enemies, Dictionary<(int x, int y), List<FireBall>> fireBalls)
        {
            UpdatePlayer(gameTime, tiles, enemies, fireBalls);
            UpdateHealthBar();
        }

        // update player's movement, and states
        private void UpdatePlayer(GameTime gametime, Tile[,] tiles, Enemy[,] enemies, Dictionary<(int x, int y), List<FireBall>> fireBalls)
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

                // update player speed if not healing
                if (!isHeal)
                {
                    playerSpd.X -= ACCEL;
                    playerSpd.X = MathHelper.Clamp(playerSpd.X, -maxSpdX, maxSpdX);
                }
            }
            else if (kb.IsKeyDown(Keys.D))
            {
                // update the player states
                playerState = isGround ? WALK : JUMP;
                playerDir = FaceDirection.Right;
                isIdle = false;

                // update player speed if not healing
                if (!isHeal)
                {
                    playerSpd.X += ACCEL;
                    playerSpd.X = MathHelper.Clamp(playerSpd.X, -maxSpdX, maxSpdX);
                }
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
            if (kb.IsKeyDown(Keys.S) && isGround)
            {
                // update player state
                playerState = CROUCH;

                // update the player health
                currHealth = Math.Min(currHealth * (1 + healthFactor), maxHealth);

                // change player flags
                isHeal = true;
                isIdle = false;
            }
            else
            {
                isHeal = false;
            }

            // update blood target angle
            bloodEmitter.ChangeAngle(bloodEmitter.GetParticleLaunchPoint(), ToVector(mouse.Position));

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
                playerState = FALL;
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
                // instead update immune timer and update flash timer
                immuneTime.Update(gametime.ElapsedGameTime.TotalMilliseconds);
                flashColorTimer.Update(gametime.ElapsedGameTime.TotalMilliseconds);

                // check if timer is over
                if (immuneTime.IsFinished())
                {
                    // set immune flag and flashFlag to false
                    // reset immune and flash timer
                    isImmune = false;
                    flashFlag = false;
                    immuneTime.ResetTimer(false);
                    flashColorTimer.ResetTimer(false);
                }
            }

            // check enemy collisons
            UpdateEnemyCollision(enemies, fireBalls);
            // check tiles collisions
            UpdateTileCollision(tiles);

            // update the player's animation
            UpdatePlayerAnimRec(gametime);

            LoadPlayerRecs();

            UpdateBlood(gametime);
        }

        // update blood particles
        private void UpdateBlood(GameTime gametime)
        {
            // store if the mouse if pressed
            bool mouseFlag = false;

            if (mouse.LeftButton == ButtonState.Pressed)
            {
                mouseFlag = true;
                
                bloodEmitter.SetPostion(new Vector2(playerRec.Center.X, playerRec.Center.Y));

                currHealth -= bloodFactor;
            }
            bloodEmitter.Start();
                

            bloodEmitter.Update((float)gametime.ElapsedGameTime.TotalMilliseconds, mouseFlag);
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
                                else if (verticalVec == 1 && !tempGround && tiles[row, col].GetCollision != TileCollision.Platform)
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
                                if (playerRecs[LEFT].Intersects(tileRec) && tiles[row, col].GetCollision != TileCollision.Platform)
                                {
                                    playerRec.X = tileRec.X + tileRec.Width;
                                    playerPos.X = playerRec.X;
                                    playerSpd.X = 0f;
                                    collision = true;

                                    LoadPlayerRecs();
                                }
                                else if (playerRecs[RIGHT].Intersects(tileRec) && tiles[row, col].GetCollision != TileCollision.Platform)
                                {
                                    playerRec.X = tileRec.X - playerRec.Width;
                                    playerPos.X = playerRec.X;
                                    playerSpd.X = 0f;
                                    collision = true;

                                    LoadPlayerRecs();
                                }

                                // update lvl if player reaches exit
                                if (tiles[row, col].GetCollision == TileCollision.Exit)
                                {
                                    isNextLvl = true;
                                }
                                
                            }

                            // check if blood parts hit wall
                            bloodEmitter.ParticleCollision(tileRec);
                        }
                    }
                }
            }
        }

        // check if player has collision with enemy
        private void UpdateEnemyCollision(Enemy[,] enemies, Dictionary<(int x, int y), List<FireBall>> fireBalls)
        {
            // store the size of the 2D array enemies
            int width = enemies.GetLength(0);
            int height = enemies.GetLength(1);

            for (int col = 0; col < width; col++)
            {
                for (int row = 0; row < height; row++)
                {
                    bool collision = false;

                    if (enemies[col, row] != null)
                    {
                        Rectangle enemyRec = enemies[col, row].EnemyRec;


                        // only check if th e player isn't immune
                        if (!isImmune)
                        {
                            // check if there is intersection with enemy
                            if (playerRec.Intersects(enemyRec))
                            {
                                // check what type of enemy
                                if (enemies[col, row] is Runner)
                                {
                                    currHealth -= ((Runner)(enemies[col, row])).GetDamage;

                                    isImmune = true;
                                    immuneTime.ResetTimer(true);
                                    flashColorTimer.ResetTimer(true);
                                }
                            }
                        }

                        

                        // update health of enemies health
                        if (bloodEmitter.ParticleCollision(enemyRec))
                        {
                            enemies[col, row].UpdateHealth(bloodDamage);
                        }

                    }
                }
            }

            if (fireBalls != null)
            {
                // check if the player has collided with any darts
                foreach (List<FireBall> i in fireBalls.Values)
                {
                    for (int j = 0; j < i.Count; j++)
                    {
                        if (!isImmune)
                        {
                            // check if player intersects with a dart
                            if (playerRec.Intersects(i[j].GetRec))
                            {
                            
                                // update player health
                                currHealth -= i[j].GetDamage;

                                // remove dart in list
                                i.RemoveAt(j);
                                j--;

                                // make player immune
                                isImmune = true;
                                immuneTime.ResetTimer(true);
                                flashColorTimer.ResetTimer(true);
                            }
                        }
                    }
                }
            }
        }


        // draw all of player's properties
        public void Draw(SpriteBatch spriteBatch)
        {
            DrawPlayer(spriteBatch);
            DrawHud(spriteBatch);
            DrawBlood(spriteBatch);
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

            // local player color
            Color playerColor = Color.White;


            // check if the player is immune
            if (isImmune)
            {
                if (flashColorTimer.IsFinished())
                {
                    flashFlag = !flashFlag;
                    flashColorTimer.ResetTimer(true);
                }

                if (flashFlag)
                {
                    playerColor = Color.White * 0.3f;
                }
                else
                {
                    playerColor = Color.White;
                }   
            }

            playerAnims[playerState].Draw(spriteBatch, playerColor, animFlip);


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
            // draw the score counter
            spriteBatch.DrawString(scoreFont, "SCORE: " + Convert.ToString(score), scorePos, Color.White);

            // draw the blood gauge
            spriteBatch.Draw(healthBarImg, healthBarRec, Color.White);
            spriteBatch.Draw(healthBarBoarderImg, healthBarBoarderRec, Color.White);
        }

        private void DrawBlood(SpriteBatch spriteBatch)
        {
            bloodEmitter.Draw(spriteBatch);
        }

        private void AnimToPlayerPos()
        {
            animPos.X = playerPos.X - ((playerAnims[playerState].destRec.Width) - (playerWidth)) / 2 + 2;
            animPos.Y = playerPos.Y - ((playerAnims[playerState].destRec.Height)- (playerHeight)) / 2 + 2;
        }

        // returns vector2 value of point
        private Vector2 ToVector(Point point)
        {
            return new Vector2(point.X, point.Y);
        }

        // flags if the player reaches the next level
        public bool PlayerNextLvl()
        {
            return isNextLvl;
        }

        // flags if the player is dead
        public bool IsDead()
        {
            if (currHealth <= 0)
            {
                return true;
            
            }
            return false;
        }

        // reset the player for each level
        public void Reset()
        {
            currHealth = maxHealth;
            isNextLvl = false;
            healthBarRec = healthBarDefault;

            playerPos = spawnPoint;
        }

        // full reset of the player 
        public void FullReset()
        {
            score = 0;
            Reset();
        }
    }
}
