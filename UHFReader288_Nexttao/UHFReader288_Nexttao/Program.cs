using System;

namespace UHFReader288_Nexttao
{
    class Program
    {
        static void Main(string[] args)
        {
            Reader reader = new Reader();
            reader.Start();
            if (Console.ReadKey() != null) {
                reader.Stop();
            }
        }
    }
}
