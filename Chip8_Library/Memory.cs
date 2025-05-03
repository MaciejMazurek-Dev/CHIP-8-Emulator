namespace Chip8_Library
{
    internal class Memory
    {
        private const ushort programStartAddress = 512;
        private const ushort fontsStartAddress = 80;
        private const ushort memorySize = 4096;
        private const ushort stackSize = 16;
        private byte[] _memory = new byte[memorySize];
        private ushort[] _stack = new ushort[stackSize];

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

        public void LoadFonts(byte[] fonts)
        {
            for(int i = 0; i < fonts.Length; i++)
            {
                _memory[fontsStartAddress + i] = fonts[i];
            }
        }

        public byte GetByte(ushort address)
        {
            return _memory[address];
        }

        public void SetByte(ushort address, byte value)
        {
            _memory[address] = value;
        }

        public void SetWord(ushort address, ushort value)
        {
            _stack[address] = value;
        }

        public ushort GetWord(ushort address)
        {
            return _stack[address];
        }
    }
}
