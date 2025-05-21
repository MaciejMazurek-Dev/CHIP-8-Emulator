namespace Chip8_Library
{
    internal class Cpu
    {
        //Data Registers 8-bit
        internal byte V0;
        internal byte V1;
        internal byte V2;
        internal byte V3;
        internal byte V4;
        internal byte V5;
        internal byte V6;
        internal byte V7;
        internal byte V8;
        internal byte V9;
        internal byte VA;
        internal byte VB;
        internal byte VC;
        internal byte VD;
        internal byte VE;

        //FLAGS Register 8-bit 
        internal byte VF;

        //Address Register 16-bit
        internal ushort I;

        //Program Counter 16-bit
        internal ushort PC;

        //Stack Pointer 16-bit
        internal ushort SP;

        //Instruction Register 16-bit
        internal ushort IR;


        private Memory _memoryBus;
        private Display _displayBus;
        private Keyboard _keyboardBus;
        private Timers _timers;

        public Cpu(Memory memoryBus, Display display, Keyboard keyboard, Timers timers)
        {
            _memoryBus = memoryBus;
            _displayBus = display;
            _keyboardBus = keyboard;
            _timers = timers;

            PC = 512; //0x200
            SP = 0; 
        }
        public void Tick()
        {
            Fetch();
            DecodeAndExecute();
        }

        private void Fetch()
        {
                IR = (ushort)(((_memoryBus.GetByte(PC) << 8)));
                PC++;
                IR = (ushort)(IR & 0xFF00 | _memoryBus.GetByte(PC));
                PC++;
        }
        private void DecodeAndExecute()
        {
            ref byte registerX = ref GetRegister((byte)((IR & 0x0F00) >> 8));
            ref byte registerY = ref GetRegister((byte)((IR & 0x00F0) >> 4));
            switch (IR & 0xF000)
            {
                case 0x0000:
                    switch (IR)
                    {
                        //00E0 - CLS - Clear screen
                        case 0x00E0:
                            _displayBus.ClearScreen();
                            break;
                        //00EE - RET - Return from a subroutine
                        case 0x00EE:
                            SP--;
                            PC = _memoryBus.StackPOP(SP);
                            break;
                    }
                    break;
                //1NNN - Jump to address NNN
                case 0x1000:
                    PC = (ushort)(IR & 0x0FFF);
                    break;
                //2NNN - Call subroutine at address NNN
                case 0x2000:
                    _memoryBus.StackPUSH(SP, PC);
                    SP++;
                    PC = (ushort)(IR & 0x0FFF);
                    break;
                //3XNN - Skip next instruction if register X equals NN
                case 0x3000:
                    byte value = (byte)(IR & 0x00FF);
                    if (registerX == value)
                    {
                        PC++;
                        PC++;
                    }
                    break;
                //4XNN - Skip next instruction if register X does not equal NN
                case 0x4000:
                    value = (byte)(IR & 0x00FF);
                    if (registerX != value)
                    {
                        PC++;
                        PC++;
                    }
                    break;
                //5XY0 - Skip next instruction if register X equals register Y
                case 0x5000:
                    if (registerX == registerY)
                    {
                        PC++;
                        PC++;
                    }
                    break;
                //6XNN - Sets register X to NN
                case 0x6000:
                    byte registerNumber = (byte)((IR & 0x0F00) >> 8);
                    value = (byte)(IR & 0x00FF);
                    registerX = value;
                    break;
                //7XNN - Adds NN to register X 
                case 0x7000:
                    value = (byte)(IR & 0x00FF);
                    registerX = (byte)(registerX + value);
                    break;
                case 0x8000:
                    switch (IR & 0x000F)
                    {
                        //8XY0 - Sets register X to the value from register Y
                        case 0x0000:
                            registerX = registerY;
                            break;
                        //8XY1 - Performs a bitwise OR on the values of registers X and Y, then stores the result in register X.
                        case 0x0001:
                            registerX = (byte)(registerX | registerY);
                            break;
                        //8XY2 - Performs a bitwise AND on the values of registers X and Y, then stores the result in register X.
                        case 0x0002:
                            registerX = (byte)(registerX & registerY);
                            break;
                        //8XY3 - Performs a bitwise XOR on the values of registers X and Y, then stores the result in register X.
                        case 0x0003:
                            registerX = (byte)(registerX ^ registerY);
                            break;
                        //8XY4 - Adds register X to register Y. FLAGS register VF is set to 1 when there's an overflow, and to 0 when there is not. Only the lowest 8 bits of the result are kept, and stored in register X.
                        case 0x0004:
                            try
                            {
                                registerX = checked((byte)(registerX + registerY));
                            }
                            catch (OverflowException ex)
                            {
                                registerX = (byte)(registerX + registerY);
                                VF = 1;
                                break;
                            }
                            VF = 0;
                            break;
                        //8XY5 - Value in register Y is subtracted from value in register X. FLAGS register VF is set to 0 when there's an underflow, and 1 when there is not.
                        case 0x0005:
                            try
                            {
                                registerX = checked((byte)(registerX - registerY));
                            }
                            catch (OverflowException ex)
                            {
                                registerX = (byte)(registerX - registerY);
                                VF = 0;
                                break;
                            }
                            VF = 1;
                            break;
                        //8XY6 - Shifts bits in register X to the right by 1. If the least-significant bit of register X is 1, then FLAGS register VF is set to 1, otherwise 0.
                        case 0x0006:
                            if ((registerX & 0b_0000_0001) == 0b_0000_0001)
                            {
                                VF = 1;
                            }
                            else
                            {
                                VF = 0;
                            }
                            registerX = (byte)(registerX >>> 1);
                            break;
                        //8XY7 - Sets register X to register Y minus register X. FLAGS register VF is set to 0 when there's an underflow, and 1 when there is not.
                        case 0x0007:
                            try
                            {
                                registerX = checked((byte)(registerY - registerX));
                            }
                            catch (OverflowException ex)
                            {
                                registerX = (byte)(registerY - registerX);
                                VF = 0;
                                break;
                            }
                            VF = 1;
                            break;
                        //8XYE - Shifts bits in register X to the left by 1, then sets VF to 1 if the most significant bit of register X prior to that shift was set, or to 0 if it was unset.
                        case 0x000E:
                            if ((registerX & 0b_1000_0000) == 0b_1000_0000)
                            {
                                VF = 1;
                            }
                            else
                            {
                                VF = 0;
                            }
                            registerX = (byte)(registerX << 1);
                            break;
                    }
                    break;
                //9XY0 - The values of registers X and Y are compared, and if they are not equal, the program counter is increased by 1.
                case 0x9000:
                    if (registerX != registerY)
                    {
                        PC++;
                        PC++;
                    }
                    break;
                //ANNN - Sets register I to NNN
                case 0xA000:
                    I = (ushort)(IR & 0x0FFF);
                    break;
                //BNNN - Jumps to the address NNN plus value in register V0
                case 0xB000:
                    PC = (ushort)((IR & 0x0FFF) + V0);
                    break;
                //CXNN - Sets register X to the result of a bitwise AND operation on a random number (0 to 255) and NN.
                case 0xC000:
                    Random random = new();
                    int randomNumber = random.Next(0, 255);
                    registerX = (byte)((IR & 0x00FF) & randomNumber);
                    break;
                //DXYN - Display N-byte sprite starting at memory location I at (X, Y), set VF = collision.
                case 0xD000:
                    byte nValue =(byte)(IR & 0x000F);
                    _displayBus.DrawSprite(registerX, registerY, I, nValue);
                    break;
                case 0xE000:
                    {
                        switch (IR & 0x00F0)
                        {
                            //EX9E - Skips the next instruction if the key stored in register X is pressed
                            case 0x0090:
                                if (_keyboardBus.key[registerX] == true)
                                {
                                    PC++;
                                    PC++;
                                }
                                break;
                            //EXA1 - Skips the next instruction if the key stored in register X is not pressed
                            case 0x00A0:
                                if (_keyboardBus.key[registerX] == false)
                                {
                                    PC++;
                                    PC++;
                                }
                                break;
                        }
                    }
                    break;
                case 0xF000:
                    {
                        switch(IR & 0x00FF)
                        {
                            //FX07 - Sets register X to the value of the delay timer
                            case 0x0007:
                                registerX = _timers.delayTime;
                                break;
                            //FX0A - A key press is awaited, and then stored in register X 
                            case 0x000A:
                                registerX = _keyboardBus.GetKey();
                                break;
                            //FX15 - Sets the delay timer to register X
                            case 0x0015:
                                _timers.SetDelayTimer(registerX);
                                break;
                            //FX18 - Sets the sound timer to register X
                            case 0x0018:
                                _timers.SetSoundTimer(registerX);
                                break;
                            //FX1E - Adds register X to register I
                            case 0x001E:
                                I += registerX;
                                break;
                        }
                    }
                    break;
            }
        }
        
        private ref byte GetRegister(byte register)
        {
            switch (register)
            {
                case 0x0:
                    return ref V0;
                case 0x1:
                    return ref V1;
                case 0x2:
                    return ref V2;
                case 0x3:
                    return ref V3;
                case 0x4:
                    return ref V4;
                case 0x5:
                    return ref V5;
                case 0x6:
                    return ref V6;
                case 0x7:
                    return ref V7;
                case 0x8:
                    return ref V8;
                case 0x9:
                    return ref V9;
                case 0xA:
                    return ref VA;
                case 0xB:
                    return ref VB;
                case 0xC:
                    return ref VC;
                case 0xD:
                    return ref VD;
                case 0xE:
                    return ref VE;
                case 0xF:
                    return ref VF;
                default:
                    return ref V0;
            }
        }
    }
}
