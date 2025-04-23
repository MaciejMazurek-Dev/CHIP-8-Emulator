namespace Chip8_Library
{
    internal class Display
    {
        private const ushort width = 64;
        private const ushort height = 32;
        private bool[,] _screen;

        public Display()
        {
            _screen = new bool[height, width];
            ClearScreen();
        }
        public void ClearScreen()
        {
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    _screen[h, w] = false;
                }
            }
        }
    }
}
