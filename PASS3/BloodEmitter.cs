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
    class BloodEmitter
    {
        // used to release all particles at once;
        // if the value was > 0 it would release particle periodically
        public const int EXPLODE = 0;

        // emitter data
        private Rectangle rec;
        private int numParticles;
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
        private float gravity = 120f;
        private float wind = 0f;

        // particle generation range
        private float lowAngle = 30;
        private float highAngle = 70;
        private float lowSpd = 80;
        private float highSpd = 100;
        private float lowLifeSpan;
        private float highLifeSpan;

        // random variable
        Random rng = new Random();


        // emitter constructor
        public BloodEmitter(Texture2D img, Rectangle rec, Vector2 particleSize, float transparency, 
            int numParticles, float launchFrequency, int particlesPerLaunch, bool cycleParticles)
        {
            // store passed through data of emitter and particles
            particleImg = img;
            this.rec = rec;
            this.particleSize = particleSize;
            this.transparency = Math.Min(1f, Math.Max(0f, transparency)); // max: transparency min: 0  transparency
            this.numParticles = numParticles;
            launchFreq = launchFrequency;
            this.particlesPerLaunch = particlesPerLaunch;
            this.cycleParticles = cycleParticles;
            this.gravity = gravity;
            this.wind = wind;

            // setup basic emitter day
            active = false;
            timePassed = 0;
            particles = new List<BloodParticle>(numParticles);


            // create all particles in the system
            for (int i = 0; i < numParticles; i++)
            {
                // create a new blood particle
                particles.Add(new BloodParticle(img, rec, gravity, wind, rng.Next((int)lowAngle, (int)highAngle + 1), rng.Next((int)lowSpd, (int)highSpd + 1), transparency, rng, 2000));
            }
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
                        if (!cycleParticles || particlesDead + particlesAlive < numParticles && mousePress)
                        {
                            timePassed = 0;

                            // launch the particles
                            LaunchParticles(particlesPerLaunch, mousePress);
                        }
                    }
                }

                // update particle if it's active, kill if it's dead
                for (int i = 0; i < particles.Count; i++)
                {
                    if (!particles[i].Update(GetParticleLaunchPoint(), deltaTime))
                    {
                        // particle has die, reset it
                        particles[i].RandomizeTrajectory(lowSpd, highSpd, lowAngle, highAngle);

                        // increase the amount of particles dead
                        particlesDead++;
                    }
                }

                // check of end of cycling
                if (particlesDead >= numParticles)
                {
                    // reset all particles
                    for (int i = 0; i < particles.Count; i++)
                    {
                        particles[i].ResetParticle(GetParticleLaunchPoint());
                        particlesAlive--;
                    }

                    // after iterating all particles reset all particles
                    Reset();
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
                    if (particles[i].Draw(spriteBatch))
                    {
                        Console.WriteLine(i);
                    }
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

        // launch all particles
        private void LaunchParticles(int numParticles, bool mousePress)
        {
            int numLaunched = 0;

            // only contuies to lauch particles until no more particles to launch or have launched to desired amount
            for (int i = 0; i < particles.Count && numLaunched < numParticles; i++)
            {
                // only launch non - active particles
                if (!particles[i].isActive())
                {
                    if (particles[i].Launch())
                    {
                        // increase particles launched and active
                        numLaunched++;
                        particlesAlive++;
                    }
                }
            }
        }

        public void SetPostion(Vector2 newPos)
        {
            rec.X = (int)newPos.X;
            rec.Y = (int)newPos.Y;

            for (int i = 0; i < particles.Count; i++)
            {
                if (!particles[i].isActive())
                {
                    particles[i].ResetParticle(newPos);
                }
            }
        }


        private Vector2 GetParticleLaunchPoint()
        {
            // store the emittersCenter Pos
            Vector2 emitterCentre = new Vector2(rec.X + rec.Width / 2, rec.Y + rec.Height / 2);

            // return pos, where the particles are centred at such point
            return new Vector2(emitterCentre.X - particleSize.X / 2, emitterCentre.Y - particleSize.Y / 2);
        }
    }
}
