using System;
using System.Collections.Generic;
using System.Text;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoubleDash
{
    public class StarBackground
    {
        private const int MaxStars = 100;
        private GraphicsDeviceManager graphics;
        private List<Sprite> stars;
        private VirtualResolutionRenderer vrr;

        public StarBackground(GraphicsDeviceManager graphics, VirtualResolutionRenderer vrr)
        {
            this.graphics = graphics;
            stars = new List<Sprite>();
            this.vrr = vrr;
        }

        public void GenerateStars(Camera camera)
        {
            while (stars.Count < MaxStars)
            {
                Sprite star = new Sprite(graphics);
                star.position = new Vector2(World.random.Next(0, (int)vrr.VirtualResolution.Width),
                    World.random.Next(0, (int)vrr.VirtualResolution.Height));
                star.DrawSize = new Size(World.random.Next(1, 5));
                star.position = Vector2.Transform(star.position, camera.InverseTransform);
                star.color = Color.Black;
                stars.Add(star);
            }
        }

        public void MoveUp(float distance)
        {
            Move(new Vector2(0, -distance));
        }

        public void MoveDown(float distance)
        {
            Move(new Vector2(0, distance));
        }

        public void MoveLeft(float distance)
        {
            Move(new Vector2(-distance, 0));
        }

        public void MoveRight(float distance)
        {
            Move(new Vector2(distance, 0));
        }

        public void Move(Vector2 vector)
        {
            foreach (var star in stars)
            {
                star.position += vector;
            }
        }

        public void Update(GameTimeWrapper gameTime, Camera camera)
        {
            GenerateStars(camera);
            for (int i = 0; i < stars.Count; i++)
            {
                stars[i].Update(gameTime);
                if (!camera.Contains(stars[i].position))
                {
                    stars.RemoveAt(i);
                    i--;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var star in stars)
            {
                star.Draw(spriteBatch);
            }
        }
    }
}
