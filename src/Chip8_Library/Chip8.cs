namespace Chip8_Library
{
    public class Chip8
    {
        internal readonly Memory memory;
        internal Cpu cpu;
        internal readonly Display display;
        internal Keyboard keyboard;
        internal Timers timers;
        public bool[,] Screen
        {
            get
            {
                return display.screen;
            }
        }
        public void SetKey(byte key)
        {
            keyboard.SetKey(key);
        }

        public Chip8()
        {
            memory = new();
            display = new(memory);
            keyboard = new();
            timers = new();
            memory.LoadFonts(Font.fonts);

            cpu = new(memory, display, keyboard, timers);
        }
        public bool Load(byte[] data)
        {
            return memory.Load(data);
        }
        public void Run()
        {
            cpu.Tick();
            keyboard.Reset();
        }
    }
}
