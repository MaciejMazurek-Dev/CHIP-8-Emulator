using Chip8_Library;

namespace Chip8_Library_Test_Unit
{
    public class CpuTests
    {
        Chip8 chip8 = new();

        [Fact]
        public void ExecuteInstruction_00E0_ClearScreen()
        {
            //Arrange
            chip8.Load(new byte[]
            {
                0x00E0
            });
            chip8.Run();
            
        }
    }
}
