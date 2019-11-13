using System;
using System.IO;

namespace SpecEntityNetCore3
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbPath = Path.Combine(AppContext.BaseDirectory, "userdatabase.db");
            Console.WriteLine("Hello World!");
        }
    }
}
