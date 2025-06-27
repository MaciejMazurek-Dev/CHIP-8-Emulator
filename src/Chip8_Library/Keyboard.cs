namespace Chip8_Library
{
    internal class Keyboard
    {
        public bool[] key = new bool[16];
        public Keyboard()
        {
            key[0] = false;
            key[1] = false;
            key[2] = false;
            key[3] = false;
            key[4] = false;
            key[5] = false;
            key[6] = false;
            key[7] = false;
            key[8] = false;
            key[9] = false;
            key[10] = false;
            key[11] = false;
            key[12] = false;
            key[13] = false;
            key[14] = false;
            key[15] = false;
        }

        public void SetKey(byte keyNumber)
        {
            key[keyNumber] = true;

        }
        public void Reset()
        {
            for(int i = 0; i < key.Length; i++)
            {
                if (key[i] == true)
                {
                    key[i] = false;
                }
            }
        }
        public byte? KeyPress()
        {
            throw new NotImplementedException();
        }

    }
}
