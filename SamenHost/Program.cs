using SamenHost.Core;
using SamenHost.Internet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamenHost
{
    internal class Program
    {
        static void Main(string[] args)
        {

            /*
             *  Temp code, should be replace with a launcher at some point.
             */

            Console.WriteLine("Starting Samen...");
            Server.Start(4041);

            Console.WriteLine("Ready!");
            while (true)
            {
                Server.Update();
            }
        }
    }
}
