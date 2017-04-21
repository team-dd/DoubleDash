using System;
using System.Collections.Generic;
using System.Text;

namespace DoubleDash
{
    public class TimeManager
    {
        private List<GameTimeManager> times;

        public TimeManager()
        {
            times = new List<GameTimeManager>();
        }

        public void AddTime(GameTimeManager time)
        {
            times.Add(time);
        }

        public void AddTimes(params GameTimeManager[] times)
        {
            AddTimes(new List<GameTimeManager>(times));
        }

        public void AddTimes(List<GameTimeManager> times)
        {
            this.times.AddRange(times);
        }

        public void Play()
        {
            foreach (var time in times)
            {
                time.Play();
            }
        }

        public void Pause()
        {
            foreach (var time in times)
            {
                time.Pause();
            }
        }

        public void Reset()
        {
            foreach (var time in times)
            {
                time.Reset();
            }
        }
    }
}
