using System;
using System.Linq;
using etcetera;

namespace FagdagDemo
{
    class Program
    {
        private static EtcdClient _client;
        static void Main(string[] args)
        {
            var etcdUrl = new Uri("http://localhost:4001");
            _client = new EtcdClient(etcdUrl);
            GetOne();
            GetAllConfigss();;
            PrintAndWatch();
            Console.ReadKey();
        }

        public static void GetOne()
        {
            Console.WriteLine("\n\n");
            var dbConnectionString = _client.Get("/appconfig/internportalen/dbConnectionString").Node.Value;
            Console.ForegroundColor = ConsoleColor.DarkGreen;;
            Console.WriteLine(dbConnectionString);
            Console.ResetColor();
        }
        public static void GetAllConfigss()
        {
            Console.WriteLine("\n\n");
            Console.ForegroundColor = ConsoleColor.DarkMagenta; ;
            var config = _client.Get("/appconfig/internportalen", true);
            foreach (var c in config.Node.Nodes.ToArray())
            {
                Console.WriteLine("{0} {1}", c.Key.Split(Convert.ToChar("/")).Last(), c.Value);
            }
            Console.ResetColor();
        }

        public static void PrintAndWatch()
        {
            Console.WriteLine("\n\n");
            var config = _client.Get("/appconfig/internportalen", true);
            foreach (var c in config.Node.Nodes.ToArray())
            {
                Console.WriteLine("{0} {1}", c.Key.Split(Convert.ToChar("/")).Last(), c.Value);
            }

            _client.Watch("/appconfig/internportalen", CallBack, true);
        }

        public static void CallBack(EtcdResponse res)
        {
            Console.WriteLine("Reloading Config");
            Console.ForegroundColor = ConsoleColor.Yellow;
            PrintAndWatch();
            Console.ResetColor();
        }
    }
}
