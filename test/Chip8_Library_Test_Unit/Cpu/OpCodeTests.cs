using Chip8_Library;
using Xunit;

namespace Chip8_Library_Test_Unit.Cpu
{
    public class OpCodeTests
    {
        Chip8 chip8 = new();

        [Fact]
        public void ExecuteInstruction_00E0_ClearScreen()
        {
            //Arrange
            chip8.display.screen[10, 10] = true;
            chip8.Load(new byte[]
            {
                0x00,
                0xE0
            });

            //Act
            chip8.Run();

            //Assert
            bool[,] screen = chip8.Screen;
            for (int y = 0; y < chip8.Screen.GetLength(1); y++)
            {
                for(int x = 0; x < chip8.Screen.GetLength(0); x++)
                {
                    Assert.False(screen[x, y]); 
                }
            }
        }
        [Fact]
        public void ExecuteInstruction_00EE_ReturnFromSubroutine()
        {
            //Arrange
            chip8.memory.StackPUSH(chip8.cpu.SP, 550);
            chip8.cpu.SP++;
            chip8.Load(new byte[]
            {
                0x00,
                0xEE
            });

            //Act
            chip8.Run();

            //Assert
            Assert.Equal(550, chip8.cpu.PC);
        }
    }
}
