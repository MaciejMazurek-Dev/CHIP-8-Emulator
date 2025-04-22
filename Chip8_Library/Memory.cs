using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8_Library
{
    internal class Memory
    {
        private const int programStartAddress = 512;
        private const int memorySize = 4096;
        private ushort[] Stack = new ushort[16];

        private readonly byte[] _memory = new byte[memorySize];

        public bool Load(byte[] data)
        {
            if (data.Length > (memorySize - programStartAddress))
            {
                return false;
            }
            for (int i = 0; i < data.Length; i++)
            {
                _memory[programStartAddress + i] = data[i];
            }
            return true;
        }
    }
}
