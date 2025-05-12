using Chip8_Library;
using Xunit;
using FluentAssertions;

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
            ushort registerX = _sut.cpu.VA;

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
            ushort registerX = _sut.cpu.V2;

            //Assert
            registerX.Should().Be((byte)(registerValue + nnValue));
        }
    }
}
