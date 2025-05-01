namespace Chip8_Library
{
    public class Chip8
    {
        private readonly Memory _memory;
        private Cpu _cpu;
        private readonly Display _display;
        private Keyboard _keyboard;
        private Timers _timers;
        

        public Chip8()
        {
            _memory = new();
            _display = new();
        }
        public bool Load(byte[] data)
        {
            return _memory.Load(data);
        }
        public void Run()
        {
            _cpu = new(_memory, _display, _keyboard, _timers);
            while(true)
            {
                _cpu.Tick();

            }
        }
    }
}
