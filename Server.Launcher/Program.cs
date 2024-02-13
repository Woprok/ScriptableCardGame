using System;
using System.Net;
using System.Threading;
using Shared.Networking.Common.Protocol;
using Shared.Networking.Protocol.Models;

namespace Server.Launcher
{
    public sealed class Program
    {
        static void Main(string[] args)
        {
            //init


#if DEBUG
            int port = 1996;
            IPAddress ip = IPAddress.Loopback;
#else
            int port = InputController.ObtainPort();
            IPAddress ip = IPAddress.Parse(InputController.ObtainIP4());
#endif


            IServerMessageModel<CoreMessage> serverMessageModel = new ServerMessageModel(new IPEndPoint(ip, port));

            while (true)
            {
                Console.Write(".");
                Thread.Sleep(1000);
            }
        }
    }
}