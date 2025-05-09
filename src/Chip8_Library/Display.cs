namespace Chip8_Library
{
    internal class Display
    {
        private const ushort width = 64;
        private const ushort height = 32;
        internal bool[,] screen;

        public Display()
        {
            screen = new bool[width, height];
        }
        public void ClearScreen()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    screen[x, y] = false;
                }
            }
        }
        public bool DrawSpriteBatch(ushort xRegsiter, ushort yRegister, byte spriteBatch)
        {
            bool pixelChange = false;
            for(byte x = 0; x < 8;x++)
            {
                byte bytePixel = (byte)(spriteBatch & (0b_1000_0000 >>> x));
                bool pixel = ( bytePixel > 0);
                if(screen[xRegsiter + x, yRegister] ^ pixel)
                {
                    screen[xRegsiter + x, yRegister] = pixel;
                    pixelChange = true;
                }
            }
            return pixelChange;
        }
    }
}
