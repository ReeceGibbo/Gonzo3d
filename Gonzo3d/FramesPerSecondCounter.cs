using System;
using System.Diagnostics;

namespace Gonzo3d
{
    public class FramesPerSecondCounter
    {

        private readonly TimeSpan _oneSecondTimeSpan;
        private int _framesCounter;
        private readonly Stopwatch _stopwatch;
        private TimeSpan _timer;

        public int FramesPerSecond { get; private set; }

        public FramesPerSecondCounter()
        {
            _oneSecondTimeSpan = new TimeSpan(0, 0, 1);
            _framesCounter = 0;
            _stopwatch = new Stopwatch();
            _timer = _oneSecondTimeSpan;
        }
        
        public void Draw()
        {
            _framesCounter++;

            _timer += _stopwatch.Elapsed;
            _stopwatch.Reset();
            _stopwatch.Start();

            if (_timer <= _oneSecondTimeSpan)
                return;

            FramesPerSecond = _framesCounter;
            _framesCounter = 0;
            _timer -= _oneSecondTimeSpan;
        }
        
    }
}