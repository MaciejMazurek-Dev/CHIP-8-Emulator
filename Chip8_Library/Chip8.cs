using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8_Library
{
    public class Chip8
    {
        private readonly Memory _memory;
        private readonly Cpu _cpu;
        private readonly Display _display;

        public Chip8()
        {
            _memory = new();
            _cpu = new();
            _display = new();
        }

        public void Load(byte[] data)
        {
            _memory.Load(data);
        }
    }
}
