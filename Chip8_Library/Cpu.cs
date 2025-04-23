namespace Chip8_Library
{
    internal class Cpu
    {
        //Data Registers
        private byte V0;
        private byte V1;
        private byte V2;
        private byte V3;
        private byte V4;
        private byte V5;
        private byte V6;
        private byte V7;
        private byte V8;
        private byte V9;
        private byte VA;
        private byte VB;
        private byte VC;
        private byte VD;
        private byte VE;

        //FLAGS register
        private byte VF;

        //Address register
        private ushort I;

        //Program Counter
        private ushort PC;

        //Stack Pointer
        private ushort SP;

        //Instruction register
        private ushort IR;



        private Memory _memoryBus;
        private Display _displayBus;


        public Cpu(Memory memoryBus, Display display)
        {
            _memoryBus = memoryBus;
            _displayBus = display;

            PC = 512;
            SP = 0;
        }
        public void Tick()
        {
            Fetch();
            DecodeAndExecute();
        }


        private void Fetch()
        {
            IR = (ushort)(_memoryBus.GetByte(PC) << 8);
            PC++;
            IR = (ushort)(IR & 0xFF00 | _memoryBus.GetByte(PC));
            PC++;
        }

        private void DecodeAndExecute()
        {
            switch(IR)
            {
                //CLS - Clear screen
                case 0x00E0:
                    _displayBus.ClearScreen();
                    break;

                //RET - Return from a subroutine
                case 0x00EE:
                    PC = _memoryBus.PopStack(SP);
                    SP--;
                    break;

                




            }

        }






    }
}
