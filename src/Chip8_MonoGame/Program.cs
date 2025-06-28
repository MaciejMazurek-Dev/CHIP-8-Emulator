using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters;

internal class Program
{
    private static void Main(string[] args)
    {
        string path = "C:/Test.ch8";
        if(args.Length > 0)
        {
            path = args[0];
        }

        if(File.Exists(path))
        {
            using var game = new Chip8_MonoGame.Game1(path);
            game.Run();
        }
    }
}