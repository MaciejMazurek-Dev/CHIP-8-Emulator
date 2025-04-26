namespace Chip8_Library
{
    public class Chip8
    {
        private readonly Memory _memory;
        private Cpu _cpu;
        private readonly Display _display;
        

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
            _cpu = new(_memory, _display);
            while(true)
            {
                _cpu.Tick();

            }
        }
    }
}
