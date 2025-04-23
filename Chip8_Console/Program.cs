using Chip8_Library;

namespace Chip8_Console
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            Chip8 chip8 = new();
            byte[] file = File.ReadAllBytes("C:/CODE/Test.ch8");
            if(chip8.Load(file))
            {
                Console.WriteLine("Data has been loaded successfully.");
            }
            else
            {
                Console.WriteLine("Fail to load data.");
            }

            chip8.Run();
    }


    }
}
