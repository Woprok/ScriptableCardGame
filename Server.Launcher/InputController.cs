namespace Server.Launcher
{
    public static class InputController
    {
        public static string ObtainIP4() => ConsoleInputModel.ObtainIP4();

        public static int ObtainPort() => ConsoleInputModel.ObtainPort();

        public static int ObtainClientCount() => 10;
    }
}