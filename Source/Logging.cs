namespace Aeoquotes
{
    public class Logging
    {
        public static void Log(string message) => Console.WriteLine($"[{DateTime.Now}] {message}");

        public static void Log() => Console.WriteLine();
    }
}