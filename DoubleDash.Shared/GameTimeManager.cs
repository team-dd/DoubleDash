using System;
using System.Collections.Generic;
using System.Text;
using GLX;
using Microsoft.Xna.Framework;

namespace DoubleDash
{
    public class GameTimeManager
    {
        private GameTimeWrapper gameTimeWrapper;
        private readonly decimal originalSpeed;
        private decimal currentSpeed;

        public GameTimeManager(GameTimeWrapper gameTimeWrapper, decimal originalSpeed)
        {
            this.gameTimeWrapper = gameTimeWrapper;
            this.originalSpeed = originalSpeed;
        }

        public void SetSpeed(decimal speed)
        {
            currentSpeed = speed;
            gameTimeWrapper.GameSpeed = currentSpeed;
        }

        public void MultiplyOriginalSpeed(decimal multiplier)
        {
            currentSpeed = originalSpeed * multiplier;
            gameTimeWrapper.GameSpeed = currentSpeed;
        }

        public void Play()
        {
            gameTimeWrapper.GameSpeed = currentSpeed;
        }

        public void Pause()
        {
            currentSpeed = gameTimeWrapper.GameSpeed;
            gameTimeWrapper.GameSpeed = 0;
        }

        public void Reset()
        {
            gameTimeWrapper.GameSpeed = originalSpeed;
        }
    }
}
