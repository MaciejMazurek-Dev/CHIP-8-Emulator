using System.Timers;
using Timer = System.Timers.Timer;

namespace Chip8_Library
{
    internal class Timers
    {
        public byte delayTime = 0;
        public byte soundTime = 0;

        private Timer _delayTimer = new Timer(1000);
        private Timer _soundTimer = new Timer(1000);

        public void SetDelayTimer(byte seconds)
        {
            delayTime = seconds;
            _delayTimer.Elapsed += OnDelayTimedEvent;
            _delayTimer.Enabled = true;
        }
        public void SetSoundTimer(byte second)
        {
            soundTime = second;
            _soundTimer.Elapsed += OnSoundTimedEvent;
            _soundTimer.Enabled = true;
        }
        private void OnDelayTimedEvent(object source, ElapsedEventArgs e)
        {
            if(delayTime > 1)
            {
                delayTime--;
            }
            else
            {
                _delayTimer.Enabled = false;
                delayTime = 0;
            }
        }
        private void OnSoundTimedEvent(object source, ElapsedEventArgs e)
        {
            if(soundTime > 1)
            {
                soundTime--;
            }
            else
            {
                _soundTimer.Enabled = false;
                soundTime = 0;
            }
        }
    }
}
