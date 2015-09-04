using System;
using System.Linq;
using System.Threading;
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
            InsertSomeTestData();
            GetOne();
            GetAllConfigs();;
            PrintAndWatchApiKey();

            while (true)
            {
                Thread.Sleep(3000);
                InsertSomeRandomApiKey();
            }
        }

        public static void InsertSomeTestData()
        {
            _client.Set("/appconfig/internportalen/dbConnectionString", "db-01.bekk.no;Database=internportalen;user:foo;password:bar");
            _client.Set("/appconfig/internportalen/apikey", "some-secret-api-key");
        }

        public static void GetOne()
        {
            Console.WriteLine("\n\n");
            var dbConnectionString = _client.Get("/appconfig/internportalen/dbConnectionString").Node.Value;
            Console.ForegroundColor = ConsoleColor.DarkGreen;;
            Console.WriteLine(dbConnectionString);
            Console.ResetColor();
        }
        public static void GetAllConfigs()
        {
            Console.WriteLine("\n\n");
            Console.ForegroundColor = ConsoleColor.DarkMagenta; ;
            var config = _client.Get("/appconfig/internportalen", true);
            foreach (var c in config.Node.Nodes.ToArray())
            {
                Console.WriteLine("{0} {1}", c.Key.Split(Convert.ToChar("/")).Last(), c.Value);
            }
            Console.WriteLine("\n");
            Console.ResetColor();
        }

        public static void PrintAndWatchApiKey()
        {
            var res = _client.Get("/appconfig/internportalen/apikey");
            
            Console.WriteLine("{0} {1}", res.Node.Key.Split(Convert.ToChar("/")).Last(), res.Node.Value);

            _client.Watch("/appconfig/internportalen", CallBack, true);
        }

        public static void CallBack(EtcdResponse res)
        {
            Console.WriteLine("Reloading Config \n");
            PrintAndWatchApiKey();
        }

        public static void InsertSomeRandomApiKey()
        {
            _client.Set("/appconfig/internportalen/apikey", Guid.NewGuid().ToString());
        }
    }
}
