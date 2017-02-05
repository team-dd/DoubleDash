using System;
using System.Collections.Generic;
using System.Text;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace DoubleDash
{
    public class RainManager
    {
        private GraphicsDeviceManager graphics;
        private List<Line> rain;
        private List<Sound> rainSounds;

        public RainManager(GraphicsDeviceManager graphics, List<SoundEffect> sounds)
        {
            this.graphics = graphics;
            rainSounds = new List<Sound>();
            rain = new List<Line>();
            foreach (var sound in sounds)
            {
                rainSounds.Add(new Sound(sound));
            }
        }

        public void Spawn(int screenWidth)
        {
            int color = World.random.Next(0, 2);
            Line line = new Line(graphics);
            if (color == 0)
            {
                line.color = Color.White;
            }
            else
            {
                line.color = Color.Black;
            }
            Vector2 vector = new Vector2(0, 1);
            float magnitude = (float)World.random.NextDouble(500, 750);
            line.point1 = new Vector2(World.random.Next(0, screenWidth), 0);
            line.point2 = line.point1 - vector * magnitude;
            line.velocity = vector * (float)World.random.NextDouble(40, 50);
            rain.Add(line);
        }

        public void Update(int screenBottom)
        {
            for (int i = 0; i < rain.Count; i++)
            {
                rain[i].Update();
                if (rain[i].point2.Y > screenBottom)
                {
                    //rainSounds[World.random.Next(0, rainSounds.Count)].Play();
                    rain.RemoveAt(i);
                    i--;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var line in rain)
            {
                line.Draw(spriteBatch);
            }
        }
    }
}
