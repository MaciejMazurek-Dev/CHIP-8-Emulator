using Chip8_Library;

namespace Chip8_WinForms
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Chip8 chip8 = new();
            byte[] file = File.ReadAllBytes("C:/CODE/Test.ch8");
            if (chip8.Load(file))
            {
                Console.WriteLine("Data has been loaded successfully.");
            }
            else
            {
                Console.WriteLine("Fail to load data.");
            }

            chip8.Run();

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainWindow());
        }
    }
}