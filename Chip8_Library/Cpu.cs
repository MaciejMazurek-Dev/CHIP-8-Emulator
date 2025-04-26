using System.Runtime.Versioning;

namespace Chip8_Library
{
    internal class Cpu
    {
        //Data Registers 8-bit
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

        //FLAGS Register 8-bit 
        private byte VF;

        //Address Register 16-bit
        private ushort I;

        //Program Counter 16-bit
        private ushort PC;

        //Stack Pointer 16-bit
        private ushort SP;

        //Instruction Register 16-bit
        private ushort IR;


        private Memory _memoryBus;
        private Display _displayBus;


        public Cpu(Memory memoryBus, Display display)
        {
            _memoryBus = memoryBus;
            _displayBus = display;

            PC = 512; //0x200
            SP = 4000;//0xFA0 
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
            ref byte registerX = ref V0;
            ref byte registerY = ref V0;
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
                            PC = StackPOP();
                            break;
                    }
                    break;
                //1NNN - Jump to address NNN
                case 0x1000:
                    PC = (ushort)(IR & 0x0FFF);
                    break;
                //2NNN - Call subroutine at address NNN
                case 0x2000:
                    StackPUSH(PC);
                    PC = (ushort)(IR & 0x0FFF);
                    break;
                //3XNN - Skip next instruction if register X equals NN
                case 0x3000:
                    byte value = (byte)(IR & 0x00FF);
                    registerX = ref GetRegister((byte)(IR & 0x0F00));
                    if(registerX == value)
                    {
                        PC++;
                    }
                    break;
                //4XNN - Skip next instruction if register X does not equal NN
                case 0x4000:
                    value = (byte)(IR & 0x00FF);
                    registerX = ref GetRegister((byte)(IR & 0x0F00));
                    if( registerX != value)
                    {
                        PC++;
                    }
                    break;
                //5XY0 - Skip next instruction if register X equals register Y
                case 0x5000:
                    registerX = ref GetRegister((byte)(IR & 0x0F00));
                    registerY = ref GetRegister((byte)(IR & 0x00F0));
                    if(registerX == registerY)
                    {
                        PC++;
                    }
                    break;
                //6XNN - Sets register X to NN
                case 0x6000:
                    registerX = ref GetRegister((byte)(IR & 0x0F00));
                    value = (byte)(IR & 0x00FF);
                    registerX = value;
                    break;
                //7XNN - Adds NN to register X 
                case 0x7000:
                    registerX = ref GetRegister((byte)(IR & 0x0F00));
                    value = (byte)(IR & 0x00FF);
                    registerX = (byte)(registerX + value);
                    break;
                case 0x8000:
                    switch(IR & 0x000F)
                    {
                        //8XY0 - Sets register X to the value from register Y
                        case 0x0000:
                            registerX = ref GetRegister((byte)(IR & 0x0F00));
                            registerY = ref GetRegister((byte)(IR & 0x00F0));
                            registerX = registerY;
                            break;
                        //8XY1 - Performs a bitwise OR on the values of registers X and Y, then stores the result in register X.
                        case 0x0001:
                            registerX = ref GetRegister((byte)(IR & 0x0F00));
                            registerY = ref GetRegister((byte)(IR & 0x00F0));
                            registerX = (byte)(registerX | registerY);
                            break;
                        //8XY2 - Performs a bitwise AND on the values of registers X and Y, then stores the result in register X.
                        case 0x0002:
                            registerX = ref GetRegister((byte)(IR & 0x0F00));
                            registerY = ref GetRegister((byte)(IR & 0x00F0));
                            registerX = (byte)(registerX & registerY);
                            break;
                        //8XY3 - Performs a bitwise XOR on the values of registers X and Y, then stores the result in register X.
                        case 0x0003:
                            registerX = ref GetRegister((byte)(IR & 0x0F00));
                            registerY = ref GetRegister((byte)(IR & 0x00F0));
                            registerX = (byte)(registerX ^ registerY);
                            break;
                        //8XY4 - Adds register X to register Y. FLAGS register VF is set to 1 when there's an overflow, and to 0 when there is not. Only the lowest 8 bits of the result are kept, and stored in register X.
                        case 0x0004:
                            registerX = ref GetRegister((byte)(IR & 0x0F00));
                            registerY = ref GetRegister((byte)(IR & 0x00F0));
                            try
                            {
                                registerX = checked((byte)(registerX + registerY));
                            }
                            catch(OverflowException ex)
                            {
                                registerX = (byte)(registerX + registerY);
                                VF = 1;
                                break;
                            }
                            VF = 0;
                            break;
                        //8XY5 - Value in register Y is subtracted from value in register X. FLAGS register VF is set to 0 when there's an underflow, and 1 when there is not.
                        case 0x0005:
                            registerX = ref GetRegister((byte)(IR & 0x0F00));
                            registerY = ref GetRegister((byte)(IR & 0x00F0));
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
                            registerX = ref GetRegister((byte)(IR & 0x0F00));
                            if ((registerX & 0b0000_0001) == 1)
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
                            registerX = ref GetRegister((byte)(IR & 0x0F00));
                            registerY = ref GetRegister((byte)(IR & 0x00F0));
                            if(registerY > registerX)
                            {
                                VF = 0;
                            }
                            else
                            {
                                VF = 1;
                            }
                            registerX = (byte)(registerY - registerX);
                            break;
                        //8XYE - Shifts bits in register X to the left by 1, then sets VF to 1 if the most significant bit of register X prior to that shift was set, or to 0 if it was unset.
                        case 0x000E:
                            registerX = ref GetRegister((byte)(IR & 0x0F00));
                            if((registerX & 0b1000_0000) == 1)
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
            }

        }

        private ushort StackPOP()
        {
            ushort result =  _memoryBus.GetWord(SP);
            SP -= 2;
            return result;
        }
        private void StackPUSH(ushort value)
        {
            _memoryBus.SetWord(SP, value);
            SP += 2;
        }
        private ref byte GetRegister(byte register)
        {
            switch(register)
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
