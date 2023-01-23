using Animation2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS3
{
    class BloodEmitterV2
    {
        // emitter data
        private Rectangle rec;
        private int maxParticles;
        private float launchFreq;
        private int particlesPerLaunch;
        private bool cycleParticles;

        private bool active = false;
        private bool lauched = false;
        private float timePassed = 0;
        private int particlesDead = 0;
        private int particlesAlive = 0;
        private List<BloodParticle> particles;

        // particle data
        private Texture2D particleImg;
        private Vector2 particleSize;
        private float transparency;
        private float gravity = 300f;
        private float wind = 0f;

        // particle generation range
        private float targetAngle = 50;
        private float range = 5f;
        private float lowAngle;
        private float highAngle;
        private int dir;
        private float lowSpd = 300;
        private float highSpd = 310;
        private float lowLifeSpan;
        private float highLifeSpan;

        // random variable
        Random rng = new Random();


        // emitter constructor
        public BloodEmitterV2(Texture2D img, Rectangle rec, Vector2 particleSize, float transparency,
            int maxParticles, float launchFrequency, int particlesPerLaunch)
        {
            // store passed through data of emitter and particles
            particleImg = img;
            this.rec = rec;
            this.particleSize = particleSize;
            this.transparency = Math.Min(1f, Math.Max(0f, transparency)); // max: transparency min: 0  transparency
            this.maxParticles = maxParticles;
            launchFreq = launchFrequency;
            this.particlesPerLaunch = particlesPerLaunch;
            lowAngle = targetAngle - range;
            highAngle = targetAngle + range;

            // setup basic emitter day
            active = false;
            timePassed = 0;
            particles = new List<BloodParticle>();

        }

        // update the emitter
        public void Update(float deltaTime, bool mousePress)
        {
            // if emitter is active (first check)
            if (active)
            {
                // if the launch freq > 0 meaning that it would launch particle not all at once
                if (launchFreq > 0)
                {
                    timePassed += deltaTime;

                    // if delay time has passed launch more particles
                    if (timePassed >= launchFreq)
                    {
                        // always launch if the emitter cycles, or launch if there are no more particle to launch
                        if (mousePress)
                        {
                            // create new particle
                            CreateParticle();

                            timePassed = 0;
                        }
                    }
                }

                // update particle if it's active, kill if it's dead
                int i = 0;
                while (i < particles.Count)
                {
                    if (!particles[i].Update(GetParticleLaunchPoint(), deltaTime))
                    {
                        // remove particle

                        RemoveParticle(i);

                        i--;
                    }
                    i++;
                }
            }
        }

        // draw all particles
        public void Draw(SpriteBatch spriteBatch)
        {
            if (active)
            {
                // iterate and draw each particle
                for (int i = 0; i < particles.Count; i++)
                {
                    particles[i].Draw(spriteBatch);
                }
            }
        }

        // start the emitting particles
        public void Start()
        {
            // only start if haven't started yet
            if (!active)
            {
                // reset all values and particles
                Reset();

                // change to active emitter
                active = true;
            }
        }

        // reset emitter
        private void Reset()
        {
            timePassed = 0;
            particlesAlive = 0;
            particlesDead = 0;

            // emitter is inactive
            active = false;

            // reset launch status
            lauched = false;
        }

        // create a new particle
        private void CreateParticle()
        {
            // add a new particle to the list of particles
            for (int i = 0; i < particlesPerLaunch; i++)
            {
                particles.Add(new BloodParticle(particleImg, rec, gravity, wind, rng.Next((int)lowAngle, (int)highAngle + 1), rng.Next((int)lowSpd, (int)highSpd + 1), transparency, rng, 2000));

                particles[particles.Count - 1].RandomizeTrajectory(lowSpd, highSpd, lowAngle, highAngle);
            }
        }

        // change the starting postion of the emmiter
        public void SetPostion(Vector2 newPos)
        {
            rec.X = (int)newPos.X;
            rec.Y = (int)newPos.Y;
        }

        // remove particle
        public void RemoveParticle(int index)
        {
            particles.RemoveAt(index);
        }

        // returns the lauch point of the particle, used in update
        public Vector2 GetParticleLaunchPoint()
        {
            // store the emittersCenter Pos
            Vector2 emitterCentre = new Vector2(rec.X + rec.Width / 2, rec.Y + rec.Height / 2);

            // return pos, where the particles are centred at such point
            return new Vector2(emitterCentre.X - particleSize.X / 2, emitterCentre.Y - particleSize.Y / 2);
        }

        public bool ParticleCollision(Rectangle otherRec)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                if (particles[i].isCollison(otherRec))
                {
                    RemoveParticle(i);
                    i--;
                    return true;
                }
            }
            return false;
        }

        public void ChangeAngle(Vector2 orgin, Vector2 mousePoint)
        {
            // use the mouse to control the target angle
            double xValue = orgin.X - mousePoint.X;
            double yValue = orgin.Y - mousePoint.Y;

            targetAngle = (float)(180 - Math.Atan2(yValue, xValue) * 180/Math.PI);
            //Console.WriteLine(targetAngle);

            // update low and high angle
            lowAngle = Math.Min(targetAngle + range, targetAngle - range);
            highAngle = Math.Max(targetAngle + range, targetAngle - range);
        }
    }
}
