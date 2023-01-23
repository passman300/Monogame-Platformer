//Author: Colin Wang
//File Name: Game1.cs
//Project Name: PASS3, A Dumpster Fire of a game
//Created Date: Dec. 18, 2022
//Modified Date: Jan 22, 2023
//Description: Supposed to be plat-former which the player goal is to "finish" (which they cant), with the highest score by killed mobs and completing levels.
// Variables: stored all types of variables (int, vector2, rectangle...), with different access modifiers (public, private, protected), and as consts. 
// The naming of variables reflects the general purpose of the variable (ex, playerSpd)
// Arrays: used arrays to store similar variables together for instance, animations for the runner. Where the index of each animation in the array was a state.
// If Statements: used to check what types of mobs an enemy was. Else if was checking for priority of hitbox collision  of the player. Also used logic operators (? && ||) in such lines
// Switch: quickly switch between game states
// Methods: used in every class, to help organize common logic (loading assets, updating...)
// Functions: used to return values to there methods and classes (tileManger)
// For loops: used to iterate through txt files to locate certain tiles (chars)

// NOTE: I know this project is unfinished, and it is no where near what I initially promised.
// I know there is code half done, useless variables, classes, horrible organization and poor implementation of logic. 
// But sadly I just can not bring myself to finish the game, due to personal reasons and stress. Or in other words "I have simply gave up"
// Though I do care about the marks, and the mark I expect to get from this is no where near my standards. I lack motivation and reason to complete the game. 
// It has become a chore.
// 
// In some ways, I feel sad that I could not finish the game. In others I felt that I have let down myself, you Mr.Lane, and even some other students who expected me to create something great
// If you were expecting a completed game, and high expectation. I'm Sorry.
//
// It was fun learning in your class, and I truly learned a lot about c#. Though OOP did cause me issues, I have most of it to blame on myself. For biting more then I can chew, and a lack of organization of ideas/time
// Even if I had more time, I would not even want to finish the game. 
//
// Colin Wang

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
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        // game state constant
        const int MENU = 0;
        const int HI_SCORE = 1;
        const int GAMEPLAY = 2;
        const int PAUSE = 3;
        const int GAMEOVER = 4;

        // button constants
        const int PLAY_BTN = 0;
        const int HI_SCORE_BTN = 1;
        const int EXIT_BTN = 2;
        const int MENU_BTN = 3;
        const int BACK_BTN = 4;

        // size of each button
        const int BTN_WIDTH = 245;
        const int BTN_HEIGHT = 130;

        // button pressed offset
        const int BTN_PRESSED_OFFSET_Y = 0;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;


        Player player;

        // level files
        int currLvl;
        Level lvl;
        Level[] lvls = new Level[5];

        int screenWidth;
        int screenHeight;

        MouseState prevMouse;
        MouseState mouse;

        // store the game state
        int gameState = MENU;

        // store the btn variables
        Texture2D[] btnImgs = new Texture2D[5];
        Texture2D[] btnPressedImg = new Texture2D[5];
        Vector2[] btnPos = new Vector2[5];
        Rectangle[] btnRecs = new Rectangle[5];
        Texture2D[] menuBgImgs = new Texture2D[2];


        // high score
        int hiScore = -1;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            // make the cursor visible 
            IsMouseVisible = true;

            //Change the size of the window
            this.graphics.PreferredBackBufferWidth = (int)(47 * Tile.Size.X);
            this.graphics.PreferredBackBufferHeight = 700;
            this.graphics.ApplyChanges();

            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;

            player = new Player(Content, GraphicsDevice);
            player.SetSpawnPoint = new Vector2(500, -1000);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            // load player
            player.LoadPlayer();

            //load menu background images

            // load btn images
            btnImgs[PLAY_BTN] = Content.Load<Texture2D>("Images/Buttons/Play");
            btnPressedImg[PLAY_BTN] = Content.Load<Texture2D>("Images/Buttons/PlayPressed");
            btnImgs[MENU_BTN] = Content.Load<Texture2D>("Images/Buttons/MenuBtn");
            btnPressedImg[MENU_BTN] = Content.Load<Texture2D>("Images/Buttons/MenuPressed");
            btnImgs[HI_SCORE_BTN] = Content.Load<Texture2D>("Images/Buttons/HighScoreBtn");
            btnPressedImg[HI_SCORE_BTN] = Content.Load<Texture2D>("Images/Buttons/HighScorePressed");
            btnImgs[EXIT_BTN] = Content.Load<Texture2D>("Images/Buttons/ExitBtn");
            btnPressedImg[EXIT_BTN] = Content.Load<Texture2D>("Images/Buttons/ExitPressed");
            btnImgs[BACK_BTN] = Content.Load<Texture2D>("Images/Buttons/BackBtn");
            btnPressedImg[BACK_BTN] = Content.Load<Texture2D>("Images/Buttons/BackPressed");

            // load button positions 
            btnPos[PLAY_BTN] = new Vector2(800, 70);
            btnPos[HI_SCORE_BTN] = new Vector2(800, 270);
            btnPos[EXIT_BTN] = new Vector2(800, 470);
            btnPos[MENU_BTN] = new Vector2(800, 540);

            // load button rec
            btnRecs[PLAY_BTN] = new Rectangle((int)btnPos[PLAY_BTN].X, (int)btnPos[PLAY_BTN].Y, BTN_WIDTH, BTN_HEIGHT);
            btnRecs[HI_SCORE_BTN] = new Rectangle((int)btnPos[HI_SCORE_BTN].X, (int)btnPos[HI_SCORE_BTN].Y, BTN_WIDTH, BTN_HEIGHT);
            btnRecs[EXIT_BTN] = new Rectangle((int)btnPos[EXIT_BTN].X, (int)btnPos[EXIT_BTN].Y, BTN_WIDTH, BTN_HEIGHT);
            btnRecs[MENU_BTN] = new Rectangle((int)btnPos[MENU_BTN].X, (int)btnPos[MENU_BTN].Y, BTN_WIDTH, BTN_HEIGHT);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // update mouse postion
            prevMouse = mouse;
            mouse = Mouse.GetState();

            // update game state
            switch(gameState)
            {
                case MENU:
                    UpdateMenu();
                    break;
                case GAMEPLAY:
                    UpdateGamePlay(gameTime);
                    break;
            }



            base.Update(gameTime);
        }


        // update the menu and check for button clicks
        private void UpdateMenu()
        {
            // check if the player has clicked
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                // check if the player had clicked any of the 3 buttons on the menu
                if (CheckRecHover(btnRecs[PLAY_BTN]))
                {
                    gameState = GAMEPLAY;

                    // load starting level
                    currLvl = 0;
                    lvls[currLvl] = new Level(Content, GraphicsDevice, currLvl, player);
                    lvls[currLvl].LoadContent();
                }
                else if (CheckRecHover(btnRecs[HI_SCORE_BTN]))
                {
                    gameState = HI_SCORE;
                }
                else if (CheckRecHover(btnRecs[EXIT_BTN]))
                {
                    Exit();
                }
            }
        }

        // update game play
        private void UpdateGamePlay(GameTime gameTime)
        {
            // TODO: Add your update logic here
            lvls[currLvl].UpdateLevel(gameTime);

            if (lvls[currLvl].NextLvl())
            {
                currLvl += 1;
                lvls[currLvl] = new Level(Content, GraphicsDevice, currLvl, player);
                lvls[currLvl].LoadContent();

                // reset the player
                player.Reset();
            }

            if (lvls[currLvl].IsPlayerDead())
            {
                hiScore = Math.Max(hiScore, player.Score);

                gameState = GAMEOVER;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here           

            spriteBatch.Begin();

            // draw the game state
            switch(gameState)
            {
                case MENU:
                    DrawMenu();
                    break;
                case GAMEPLAY:
                    lvls[currLvl].Draw(spriteBatch);

                    player.Draw(spriteBatch);
                    break;
            }

            // debug mouse position
            spriteBatch.DrawString(Content.Load<SpriteFont>("MouseFont"), mouse.X + " " + mouse.Y, new Vector2(mouse.X + 10, mouse.Y + 10), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        // draw all menu assets
        private void DrawMenu()
        {
            DrawMenuButtons();
        }

        // draw the 3 buttons in the menu option
        private void DrawMenuButtons()
        {
            // check if the mouse is hovering above a rec (could have combine with update button loop, but...)
            for (int i = 0; i < 3; i++)
            {
                if (CheckRecHover(btnRecs[i]))
                {
                    spriteBatch.Draw(btnPressedImg[i], btnRecs[i], Color.White);
                }
                else
                {
                    spriteBatch.Draw(btnImgs[i], btnRecs[i], Color.White);

                }
            }
        }

        // chcek if the mouse if hovering above another rectangle
        private bool CheckRecHover(Rectangle otherRec)
        {
            if (otherRec.Contains(mouse.Position))
            {
                return true;
            }
            else return false;
        }
    }
}
