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
    class Thrower : Enemy
    {
        // Thrower states
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

        // local content manger and Graphics Device
        ContentManager content;
        GraphicsDevice graphicsDevice;

        // runner damage
        private float atkDamage = 25;

        // runner health
        private float maxHealth;
        private float health;

        // runner animation arrays
        private Texture2D[] runnerImgs = new Texture2D[3];
        private Animation[] runnerAnim = new Animation[3];

        // create runner's sub hitboxes
        private Rectangle[] recs = new Rectangle[5];

        // create runner's debug sub hitboxes
        private GameRectangle[] visiableRecs = new GameRectangle[5];

        // hitbox debug swtich
        private bool showCollisionvisibleRecs = false;

        public float GetDamage
        {
            get { return atkDamage; }
        }

        public Thrower(ContentManager content, GraphicsDevice graphicsDevice, EnemyState state, Vector2 spawn,
                      FaceDirection dir, float maxHealth, Vector2 spd) :
                      base(content, graphicsDevice, state, spawn, dir, maxHealth, spd)
        {
            this.content = content;

            this.graphicsDevice = graphicsDevice;

            this.state = state;

            startPos = spawn;

            enemySpd = spd;
        }

        public void LoadThrower()
        {
            // load 
        }
    }
}
