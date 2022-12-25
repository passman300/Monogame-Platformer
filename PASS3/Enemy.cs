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
    class Enemy
    {
        // enum on where the enemy is facing
        enum FaceDirection
        {
            Left = -1,
            Right = 1,
        }

        // enemy sprite
        private Texture2D enemyImg;

        // enemy location
        private Vector2 pos;

        // enemy hitbox
        private Rectangle rec;

        // enemy spd
        private Vector2 spd;

        // enemy health variables
        private int maxHealth;
        private float currHealth;




    }
}
