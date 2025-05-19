using Chip8_Library;
using Xunit;
using FluentAssertions;
using Microsoft.Win32;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System;

namespace Chip8_Library_Test_Unit
{
    public class CpuTests
    {
        private readonly Chip8 _sut = new();

        [Fact]
        public void ExecuteInstruction_00E0_ClearScreen()
        {
            //Arrange
            _sut.display.screen[10, 10] = true;
            _sut.Load(new byte[]
            {
                0x00,
                0xE0
            });
            _sut.Run();

            //Act
            bool[,] screen = _sut.Screen;

            //Assert
            for (int y = 0; y < _sut.Screen.GetLength(1); y++)
            {
                for(int x = 0; x < _sut.Screen.GetLength(0); x++)
                {
                    Assert.False(screen[x, y]); 
                }
            }
        }
        [Theory]
        [InlineData(0x01AD)]
        [InlineData(0x044F)]
        [InlineData(0x03BD)]
        public void ExecuteInstruction_00EE_ReturnFromSubroutine(ushort expected)
        {
            //Arrange
            _sut.memory.StackPUSH(_sut.cpu.SP, expected);
            _sut.cpu.SP++;
            _sut.Load(new byte[]
            {
                0x00,
                0xEE
            });
            _sut.Run();

            //Act
            ushort registerPC = _sut.cpu.PC;

            //Assert
            registerPC.Should().Be(expected);
        }
        [Fact]
        public void ExecuteInstruction_1NNN_JumpToAddressNNN()
        {
            //Arrange
            _sut.Load(new byte[]
            {
                0x10,
                0xB7
            });
            _sut.Run();

            //Act
            ushort registerPC = _sut.cpu.PC;

            //Assert
            registerPC.Should().Be(0x00B7);
        }
        [Fact]
        public void ExecuteInstruction_2NNN_CallSubroutineAtNNN()
        {
            //Arrange
            _sut.Load(new byte[]
            {
                0x20,
                0x44
            });
            _sut.Run();

            //Act
            ushort registerPC = _sut.cpu.PC;

            //Assert
            registerPC.Should().Be(0x044);
        }
        [Fact]
        public void ExecuteInstruction_3XNN_SkipNextInstructionIfRegisterXEqualsNN()
        {
            //Arrange
            ushort snapshotRegisterPC = _sut.cpu.PC;
            _sut.cpu.V4 = 0xEA;
            _sut.Load(new byte[] {
                0x34,
                0xEA
            });
            _sut.Run();

            //Act
            ushort registerPC = _sut.cpu.PC;

            //Assert
            ushort expected = (ushort)(snapshotRegisterPC + 4);
            registerPC.Should().Be(expected);
        }
        [Fact]
        public void ExecuteInstruction_4XNN_SkipNextInstructionIfRegisterXNotEqualNN()
        {
            //Arrange
            ushort snapshotRegisterPC = _sut.cpu.PC;
            _sut.cpu.V2 = 0xAB;
            _sut.Load(new byte[]
            {
                0x42,
                0x4C
            });
            _sut.Run();

            //Act
            ushort registerPC = _sut.cpu.PC;

            //Assert
            ushort expected = (ushort)(snapshotRegisterPC + 4);
            registerPC.Should().Be(expected);


        }
        [Fact]
        public void ExecuteInstruction_5XY0_SkipNextInstructionIfRegisterXEqualsRegisterY()
        {
            //Arrange
            ushort snapshotRegisterPC = _sut.cpu.PC;
            _sut.Load(new byte[]
            {
                0x51,
                0x10
            });
            _sut.Run();

            //Act
            ushort registerPC = _sut.cpu.PC;

            //Assert
            ushort expected = (ushort)(snapshotRegisterPC + 4);
            registerPC.Should().Be(expected);
        }
        [Fact]
        public void ExecuteInstruction_6XNN_SetRegisterXToNN()
        {
            //Arrange
            byte nnValue = 0xBB;
            _sut.Load(new byte[]
            {
                0x6A,
                nnValue
            });
            _sut.Run();

            //Act
            byte registerX = _sut.cpu.VA;

            //Assert
            registerX.Should().Be(nnValue);
        }
        [Fact]
        public void ExecuteInstruction_7XNN_AddNNToRegisterX()
        {
            //Arrange
            byte nnValue = 0x1A;
            byte registerValue = 0x15;
            _sut.cpu.V2 = registerValue;
            _sut.Load(new byte[]
            {
                0x72,
                nnValue
            });
            _sut.Run();

            //Act
            byte registerX = _sut.cpu.V2;

            //Assert
            registerX.Should().Be((byte)(registerValue + nnValue));
        }
        [Fact]
        public void ExecuteInstruction_8XY0_SetRegisterXToTheValueOfRegisterY()
        {
            //Arrage 
            byte registerXValue = 0xEA;
            byte registerYValue = 0x04;
            _sut.cpu.V0 = registerXValue;
            _sut.cpu.V2 = registerYValue;
            _sut.Load(new byte[]
            {
                0x80,
                0x20
            });
            _sut.Run();

            //Act
            byte expected = _sut.cpu.V0;

            //Assert
            expected.Should().Be(registerYValue);
        }
        [Fact]
        public void ExecuteInstruction_8XY1_PerformBitwiseOROnRegisterXAndYAndStoresResultInRegisterX()
        {
            //Arrange
            byte registerXValue = 0x55;
            byte registerYValue = 0xAA;
            _sut.cpu.V0 = registerXValue;
            _sut.cpu.V2 = registerYValue;
            _sut.Load(new byte[]
            {
                0x80,
                0x21
            });
            _sut.Run();

            //Act
            byte expected = _sut.cpu.V0;

            //Assert
            expected.Should().Be(((byte)(registerXValue | registerYValue)));
        }
        [Fact]
        public void ExecuteInstruction_8XY2_PerformBitwiseANDOnRegisterXAndYAndStoresResultInRegisterX()
        {
            //Arrange
            byte registerXValue = 0x55;
            byte registerYValue = 0xAA;
            _sut.cpu.V0 = registerXValue;
            _sut.cpu.V2 = registerYValue;
            _sut.Load(new byte[]
            {
                0x80,
                0x22
            });
            _sut.Run();

            //Act
            byte expected = _sut.cpu.V0;

            //Assert
            expected.Should().Be(((byte)(registerXValue & registerYValue)));
        }
        [Fact]
        public void ExecuteInstruction_8XY3_PerformBitwiseXOROnRegisterXAndYAndStoresResultInRegisterX()
        {
            //Arrange
            byte registerXValue = 0x55;
            byte registerYValue = 0xAA;
            _sut.cpu.V0 = registerXValue;
            _sut.cpu.V2 = registerYValue;
            _sut.Load(new byte[]
            {
                0x80,
                0x23
            });
            _sut.Run();

            //Act
            byte expected = _sut.cpu.V0;

            //Assert
            expected.Should().Be(((byte)(registerXValue ^ registerYValue)));
        }
        [Fact]
        public void ExecuteInstruction_8XY4_AddRegisterXToRegisterYToGetOverflowAndSetRegisterVFTo1()
        {
            //Arrange
            byte registerXValue = 0xEE;
            byte registerYValue = 0xEE;
            _sut.cpu.V0 = registerXValue;
            _sut.cpu.V2 = registerYValue;
            _sut.Load(new byte[]
            {
                0x80,
                0x24
            });
            _sut.Run();

            //Act
            byte expectedRegisterX = _sut.cpu.V0;
            byte expectedRegisterVF = _sut.cpu.VF;

            //Assert
            expectedRegisterX.Should().Be((byte)(registerXValue + registerYValue));
            expectedRegisterVF.Should().Be(0b_0000_0001);
        }
        [Fact]
        public void ExecuteInstruction_8XY4_AddRegisterXToRegisterYWithoutOverflowAndSetRegisterVFTo0()
        {
            //Arrange
            byte registerXValue = 0x02;
            byte registerYValue = 0x05;
            _sut.cpu.V0 = registerXValue;
            _sut.cpu.V2 = registerYValue;
            _sut.Load(new byte[]
            {
                0x80,
                0x24
            });
            _sut.Run();

            //Act
            byte expectedRegisterX = _sut.cpu.V0;
            byte expectedRegisterVF = _sut.cpu.VF;

            //Assert
            expectedRegisterX.Should().Be((byte)(registerXValue + registerYValue));
            expectedRegisterVF.Should().Be(0b_0000_0000);
        }
        [Fact]
        public void ExecuteInstruction_8XY5_SetRegisterXToTheResultOfRegisterXMinusRegisterY()
        {
            //Arrange
            byte registerXValue = 0xEE;
            byte registerYValue = 0xEF;
            _sut.cpu.V0 = registerXValue;
            _sut.cpu.V2 = registerYValue;
            _sut.Load(new byte[]
            {
                0x80,
                0x25
            });
            _sut.Run();

            //Act
            byte expectedRegisterX = _sut.cpu.V0;

            //Assert
            expectedRegisterX.Should().Be((byte)(registerXValue - registerYValue));
        }
        [Fact]
        public void ExecuteInstruction_8XY5_SetRegisterVFTo1IfRegisterXIsLargerThanRegisterY()
        {
            //Arrange
            byte registerXValue = 0xFF;
            byte registerYValue = 0xEF;
            _sut.cpu.V0 = registerXValue;
            _sut.cpu.V2 = registerYValue;
            _sut.Load(new byte[]
            {
                0x80,
                0x25
            });
            _sut.Run();

            //Act
            byte expectedRegisterX = _sut.cpu.V0;
            byte expectedRegisterVF = _sut.cpu.VF;

            //Assert
            expectedRegisterVF.Should().Be(0b_0000_0001);
        }
        [Fact]
        public void ExecuteInstruction_8XY5_SetRegisterVFTo0IfRegisterXIsSmallerThanRegisterY()
        {
            //Arrange
            byte registerXValue = 0x01;
            byte registerYValue = 0xAA;
            _sut.cpu.V0 = registerXValue;
            _sut.cpu.V2 = registerYValue;
            _sut.Load(new byte[]
            {
                0x80,
                0x25
            });
            _sut.Run();

            //Act
            byte expectedRegisterX = _sut.cpu.V0;
            byte expectedRegisterVF = _sut.cpu.VF;

            //Assert
            expectedRegisterVF.Should().Be(0b_0000_0000);
        }
        [Fact]
        public void ExecuteInstruction_8XY6_ShiftBitsInRegisterXToRightByOneBit()
        {
            //Arrange
            byte registerXValue = 0x01;
            _sut.cpu.V0 = registerXValue;
            _sut.Load(new byte[]
            {
                0x80,
                0x26
            });
            _sut.Run();

            //Act
            byte expectedRegisterX = _sut.cpu.V0;

            //Assert
            expectedRegisterX.Should().Be((byte)(registerXValue >>> 1));
        }
        [Fact]
        public void ExecuteInstruction_8XY6_SetRegisterVFToTheValueOfLeastSignificantBitInRegisterX()
        {
            //Arrange
            byte registerXValue = 0x01;
            _sut.cpu.V0 = registerXValue;
            _sut.Load(new byte[]
            {
                0x80,
                0x26
            });
            _sut.Run();

            //Act
            byte leastSignificantBitInRegisterX = (byte)(registerXValue & 0b_0000_0001);
            byte expectedRegisterVF = _sut.cpu.VF;

            //Assert
            expectedRegisterVF.Should().Be(leastSignificantBitInRegisterX);
        }
        [Fact]
        public void ExecuteInstruction_8XY7_SetRegisterXToTheResultOfRegisterYMinusRegisterX()
        {
            //Arrange
            byte registerXValue = 0x10;
            byte registerYValue = 0xEF;
            _sut.cpu.V0 = registerXValue;
            _sut.cpu.V2 = registerYValue;
            _sut.Load(new byte[]
            {
                0x80,
                0x27
            });
            _sut.Run();

            //Act
            byte expectedRegisterX = _sut.cpu.V0;

            //Assert
            expectedRegisterX.Should().Be((byte)(registerYValue - registerXValue));
        }
        [Fact]
        public void ExecuteInstruction_8XY7_SetRegisterVFTo1IfRegisterYIsLargerThanRegisterX()
        {
            //Arrange
            byte registerXValue = 0xEF;
            byte registerYValue = 0xFF;
            _sut.cpu.V0 = registerXValue;
            _sut.cpu.V2 = registerYValue;
            _sut.Load(new byte[]
            {
                0x80,
                0x27
            });
            _sut.Run();

            //Act
            byte expectedRegisterVF = _sut.cpu.VF;

            //Assert
            expectedRegisterVF.Should().Be(0b_0000_0001);
        }
        [Fact]
        public void ExecuteInstruction_8XY7_SetRegisterVFTo0IfRegisterYIsSmallerThanRegisterX()
        {
            //Arrange
            byte registerXValue = 0xFF;
            byte registerYValue = 0xEF;
            _sut.cpu.V0 = registerXValue;
            _sut.cpu.V2 = registerYValue;
            _sut.Load(new byte[]
            {
                0x80,
                0x27
            });
            _sut.Run();

            //Act
            byte expectedRegisterVF = _sut.cpu.VF;

            //Assert
            expectedRegisterVF.Should().Be(0b_0000_0000);
        }
        [Fact]
        public void ExecuteInstruction_8XYE_ShiftBitsInRegisterXToLeftByOneBit()
        {
            //Arrange
            byte registerXValue = 0x01;
            _sut.cpu.V0 = registerXValue;
            _sut.Load(new byte[]
            {
                0x80,
                0x2E
            });
            _sut.Run();

            //Act
            byte expectedRegisterX = _sut.cpu.V0;

            //Assert
            expectedRegisterX.Should().Be((byte)(registerXValue << 1));
        }
        [Fact]
        public void ExecuteInstruction_8XYE_SetRegisterVFToTheValueOfMostSignificantBitInRegisterX()
        {
            //Arrange
            byte registerXValue = 0xFF;
            _sut.cpu.V0 = registerXValue;
            _sut.Load(new byte[]
            {
                0x80,
                0x2E
            });
            _sut.Run();

            //Act
            byte mostSignificantBitInRegisterX = (byte)(registerXValue >>> 7);
            byte expectedRegisterVF = _sut.cpu.VF;

            //Assert
            expectedRegisterVF.Should().Be(mostSignificantBitInRegisterX);
        }
        [Theory]
        [InlineData(0xFF, 0xFF)]
        [InlineData(0xAA, 0xBB)]
        [InlineData(0x01, 0xE5)]
        [InlineData(0x00, 0x00)]
        public void ExecuteInstruction_9XY0_IncrementRegisterPCByTwoIfRegistersXAndYAreNotEqual(byte x, byte y)
        {
            //Arrange
            byte registerXValue = x;
            byte registerYValue = y;
            _sut.cpu.V3 = registerXValue;
            _sut.cpu.V6 = registerYValue;
            ushort expectedValue = _sut.cpu.PC;
            _sut.Load(new byte[]
            {
                0x93,
                0x60
            });
            _sut.Run();
            expectedValue += 2;

            //Act
            if (registerXValue != registerYValue)
            {
                expectedValue = (ushort)(expectedValue + 2);
            }
            ushort expectedRegisterPC = _sut.cpu.PC;

            //Assert
            expectedRegisterPC.Should().Be(expectedValue);
        }
        [Theory]
        [InlineData(0x01FF)]
        [InlineData(0x00EA)]
        [InlineData(0x0000)]
        [InlineData(0x0FFF)]
        public void ExecuteInstruction_ANNN_SetRegisterIToValueNNN(ushort nnn)
        {
            //Arrange
            byte nValueInFirstByte = (byte)(nnn >>> 8);
            byte instructionPartOne = (byte)(0xA0 + nValueInFirstByte);
            byte instructionPartTwo = (byte)(nnn & 0xFF);
            _sut.Load(new byte[]
            {
                instructionPartOne,
                instructionPartTwo
            });
            _sut.Run();

            //Act
            ushort expectedRegisterI = _sut.cpu.I;

            //Assert
            expectedRegisterI.Should().Be(nnn);
        }
        [Theory]
        [InlineData(0x0001, 0x10)]
        [InlineData(0x0FFF, 0xFF)]
        [InlineData(0x0101, 0x54)]
        [InlineData(0x0000, 0x00)]
        public void ExecuteInstruction_BNNN_JumpToAddressNNNPlusValueInRegisterV0(ushort nnn, byte v0)
        {
            //Arrange
            byte nValueInFirstByte = (byte)(nnn >>> 8);
            byte instructionPartOne = (byte)(0xB0 + nValueInFirstByte);
            byte instructionPartTwo = (byte)(nnn & 0xFF);
            _sut.Load(new byte[]
            {
                instructionPartOne,
                instructionPartTwo
            });
            _sut.cpu.V0 = v0;
            _sut.Run();

            //Act
            byte registerV0 = _sut.cpu.V0;
            ushort expectedRegisterPC = _sut.cpu.PC;

            //Assert
            expectedRegisterPC.Should().Be((ushort)(nnn + registerV0));
        }
        [Theory]
        [InlineData(0x22)]
        [InlineData(0xFF)]
        [InlineData(0x4C)]
        [InlineData(0x67)]
        public void ExecuteInstruction_CXNN_SetRegisterXToTheResultOfBitwiseANDOperationOnRandomNumberAndNN(byte nn)
        {
            //Arrange
            _sut.Load(new byte[]
            {
                0xC1,
                nn
            });
            _sut.Run();

            //Act
            byte expectedRegister = _sut.cpu.V1;

            //Assert
            expectedRegister.Should().NotBe(nn);
        }



    }
}
