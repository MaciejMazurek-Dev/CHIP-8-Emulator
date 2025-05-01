namespace Chip8_Library
{
    internal class Display
    {
        private const ushort width = 64;
        private const ushort height = 32;
        private bool[,] _screen;

        public Display()
        {
            _screen = new bool[width, height];
            ClearScreen();
        }
        public void ClearScreen()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    _screen[x, y] = false;
                }
            }
        }
        public bool DrawPixels(ushort xRegsiter, ushort yRegister, byte pixelsState)
        {
            bool pixelState = false;
            bool result = true;
            for(byte x = 0; x < 8; x++)
            {
                if(((pixelsState & x) > 0) & _screen[xRegsiter + x, yRegister])
                {
                    pixelState = false;
                    result = true;
                }
                _screen[xRegsiter + x, yRegister] = pixelState;
            }
            return result;
        }
    }
}
