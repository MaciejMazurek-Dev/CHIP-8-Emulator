namespace Chip8_Library
{
    public class Chip8
    {
        private readonly Memory _memory;
        private Cpu _cpu;
        private readonly Display _display;
        private Keyboard _keyboard;
        private Timers _timers;
        public bool[,] Screen
        {
            get
            {
                return _display._screen;
            }
        }
        public Chip8()
        {
            _memory = new();
            _display = new();
            _keyboard = new();
            _timers = new();
            _memory.LoadFonts(Font.fonts);

            _cpu = new(_memory, _display, _keyboard, _timers);
        }
        public bool Load(byte[] data)
        {
            return _memory.Load(data);
        }
        public void Run()
        {
                _cpu.Tick();
        }
    }
}
