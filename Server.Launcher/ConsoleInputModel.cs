using System;

namespace Server.Launcher
{
    public static class ConsoleInputModel
    {
        public static string ObtainIP4()
        {
            Console.WriteLine("Enter server ip4:");
            string ip4 = Console.ReadLine();
            return ip4;
        }

        public static int ObtainPort()
        {
            Console.WriteLine("Enter server port:");
            int port = Int32.Parse(Console.ReadLine());
            return port;
        }

        public static int ObtainClientCount()
        {
            Console.WriteLine("Client count:");
            int count = Int32.Parse(Console.ReadLine());
            return count;
        }
    }
}