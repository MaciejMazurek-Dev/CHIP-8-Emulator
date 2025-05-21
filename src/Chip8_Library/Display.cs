using System.Reflection.Metadata.Ecma335;

namespace Chip8_Library
{
    internal class Display
    {
        private const ushort width = 64;
        private const ushort height = 32;
        private Memory _memoryBus;
        internal bool[,] screen;

        public Display(Memory memory)
        {
            _memoryBus = memory;
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
                
                bool currentPixel = screen[xRegsiter + x, yRegister];

                if (currentPixel ^ pixel)
                {
                    screen[xRegsiter + x, yRegister] = pixel;
                    pixelChange = true;
                }
            }
            return pixelChange;
        }
        public bool DrawSprite(ushort xRegister, ushort yRegister, ushort iRegister, byte length)
        {
                for(ushort y = 0; y < length; y++)
                {
                    byte memoryByte = _memoryBus.GetByte((ushort)(iRegister + y));
                
                    ushort moduloY = (ushort)(yRegister + y);
                    if(moduloY >= height)
                    {
                        moduloY %= moduloY;
                    }
                    for (ushort x = 0; x < 8; x++)
                    {
                        ushort moduloX = (ushort)(xRegister + x);
                        if(moduloX >= width)
                        {
                            moduloX %= width;
                        }
                        if((memoryByte & (0b_1000_0000 >>> x)) > 0)
                        {
                            screen[moduloX, moduloY] = true;
                        }
                        else
                        {
                            screen[moduloX, moduloY] = false;
                        }
                    }
                }
            return false;
        }
    }
}
