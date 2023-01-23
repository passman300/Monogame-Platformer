using Animation2D;
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
    class BloodParticle
    {
        private bool active = false;
        private float grav = 0f;
        private float wind = 0f;
        private float angle = 0f;
        private float spd = 0f;

        private Texture2D img;
        private Rectangle rec;
        private float transparency = 1f;
        private float startTransparecny = 1f;

        private Vector2 startPos = Vector2.Zero;
        private Vector2 startTraj = Vector2.Zero;
        private Vector2 startForces = Vector2.Zero;

        private Vector2 pos = Vector2.Zero;
        private Vector2 traj = Vector2.Zero;
        private Vector2 forces = Vector2.Zero;

        private int lifeSpan = 0;
        private int lifeSpent = 0;

        Random rng;

        // change and get the transparecny of each blood particle
        public float Transparency
        {
            set { transparency = Math.Min(1f, Math.Max(0f, value)); }
            get { return transparency; }
        }
        
        // get the life span of the particle
        public int GetLifeSpan
        {
            get { return lifeSpan; }
        }

        // get the life spent of the particle
        public int GetLifeSpent
        {
            get { return lifeSpent; }
        }

        public BloodParticle(Texture2D img, Rectangle rec, float grav, float wind, 
            float angle, float spd, float transparency, Random rng, int lifeSpan)
        {
            this.img = img;
            this.rec = rec;
            this.grav = grav;
            this.wind = wind;
            this.angle = angle;
            this.spd = spd;
            this.transparency = transparency;
            this.lifeSpan = lifeSpan;

            // pass and save random variable
            this.rng = rng;

            startPos = new Vector2(rec.X, rec.Y);
            startTraj = CalcLaunchTrajectory(spd, angle);
            startForces = new Vector2(wind, grav);

            pos = startPos;
            traj = startTraj;
            forces = startForces;
        }

        public bool Update(Vector2 launchPos, float deltaTime)
        {
            if (active)
            {
                // life span
                lifeSpent += (int)deltaTime;
                if (lifeSpent >= lifeSpan)
                {
                    // reset particle
                    ResetParticle(launchPos);

                    return false;
                }

                UpdateProjectille(deltaTime);
            }

            return true;
        }

        // draw blood particle
        public bool Draw(SpriteBatch spriteBatch)
        {
            if (active)
            {
                spriteBatch.Draw(img, rec, Color.Red);

                return true;
            }
            return false;
        }

        // returns if the particle is active
        public bool isActive()
        {
            return active;
        }

        // start the particle lanuch and animtion
        public bool Launch()
        {
            if (!active)
            {
                active = true;

                return true;
            }

            // particle has already lanuched
            return false;
        }

        // reset all particle data
        public void ResetParticle(Vector2 launchPos)
        {
            // reset all data to starting data
            pos = launchPos;
            traj = startTraj;
            forces = startForces;
            rec.X = (int)pos.X;
            rec.Y = (int)pos.Y;

            // reset lifeSpent and transparency
            lifeSpent = 0;
            Transparency = transparency;

            // deactivate particle until net launch
            active = false;
        }


         // turn's speed and angle of the particle into a vector with x, and y components
         private Vector2 CalcLaunchTrajectory(float spd, float angle)
        {
            return new Vector2(spd * (float)Math.Cos(MathHelper.ToRadians(angle)),
                            -(spd) * (float)Math.Sin(MathHelper.ToRadians(angle)));
        }

        // update the projectiles's location by forces which it's exposed to
        private void UpdateProjectille(float deltaTime)
        {
            // covert milliseconds to seconds
            float time = deltaTime * 0.001f;

            // add the forces to the trajitory force
            traj.X += forces.X * time;
            traj.Y += forces.Y * time;

            // change the pos of particle based of the trajitory
            pos.X += traj.X * time;
            pos.Y += traj.Y * time;

            // change the rec pos
            rec.X = (int)pos.X;
            rec.Y = (int)pos.Y;

        }

        // returns if the particle has collised with another rectangle
        public bool isCollison(Rectangle otherRec)
        {
            if (rec.Intersects(otherRec))
            {
                return true;
            }
            return false;
        }

        // randomize the trajrectory of the particle if it's inactive
        public void RandomizeTrajectory(float lowSpd, float highSpd, float lowAngle, float highAngle)
        {
            if (!active)
            {
                // randomize the speed and angle of the particle
                if (lowSpd < highSpd)
                spd = rng.Next((int)lowSpd, (int)highSpd);
                angle = rng.Next((int)lowAngle, (int)highAngle);

                // recalculate the trajectory
                startTraj = CalcLaunchTrajectory(spd, angle);
                traj = startTraj;

                active = true;
            }
        }
    }
}
