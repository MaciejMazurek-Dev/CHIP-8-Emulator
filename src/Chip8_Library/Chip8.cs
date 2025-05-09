namespace Chip8_Library
{
    public class Chip8
    {
        internal readonly Memory memory;
        internal Cpu cpu;
        internal readonly Display display;
        internal Keyboard keyboard;
        private Timers _timers;
        public bool[,] Screen
        {
            get
            {
                return display.screen;
            }
        }
        
        public Chip8()
        {
            memory = new();
            display = new();
            keyboard = new();
            _timers = new();
            memory.LoadFonts(Font.fonts);

            cpu = new(memory, display, keyboard, _timers);
        }
        public bool Load(byte[] data)
        {
            return memory.Load(data);
        }
        public void Run()
        {
                cpu.Tick();
        }
    }
}
