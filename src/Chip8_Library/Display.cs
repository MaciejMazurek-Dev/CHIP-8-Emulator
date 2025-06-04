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
            for (byte x = 0; x < 8; x++)
            {
                byte bytePixel = (byte)(spriteBatch & (0b_1000_0000 >>> x));
                bool pixel = (bytePixel > 0);

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
            bool shouldSetRegisterVF = false;
            ushort wrappedY = WrapPosition(yRegister, height);
            ushort wrappedX = WrapPosition(xRegister, width);

            for (ushort y = 0; y < length; y++)
            {
                byte spriteByte = _memoryBus.GetByte((ushort)(iRegister + y));
                ushort positionY = (ushort)(wrappedY + y);
                for (ushort x = 0; x < 8; x++)
                {
                    ushort positionX = (ushort)(wrappedX + x);
                    bool clipSprite = ClipSprite(positionX, positionY);
                    if (clipSprite)
                    {
                        continue;
                    }

                    bool memoryPixel = GetCurrentPixel(spriteByte, x);
                    bool screenPixel = screen[positionX, positionY];
                    bool pixelState = FlipPixel(memoryPixel, screenPixel);
                    if (pixelState)
                    {
                        screen[positionX, positionY] = !pixelState;
                        shouldSetRegisterVF = true;
                    }
                    else
                    {
                        //XOR pixels
                        screen[positionX, positionY] = memoryPixel ^ screenPixel;
                    }
                }
            }
            return shouldSetRegisterVF;
        }
        private ushort WrapPosition(ushort position, ushort divisor)
        {
            return (ushort)(position % divisor);
        }
        private bool FlipPixel(bool memoryPixel, bool screenPixel)
        {
            if (screenPixel == true && memoryPixel == true)
            {
                return true;
            }
            return false;
        }
        private bool GetCurrentPixel(byte spriteByte, ushort offset)
        {
            byte pixelPosition = (byte)(0b_1000_0000 >>> offset);
            byte currentPixel = (byte)(spriteByte & pixelPosition);
            if(currentPixel > 0)
            {
                return true;
            }
            return false;
        }
        private bool ClipSprite(ushort positionX, ushort positionY)
        {
            if(positionX >= width)
            {
                return true;
            }
            if(positionY >= height)
            {
                return true;
            }
            return false;
        }
    }
}
