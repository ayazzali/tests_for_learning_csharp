using System;
using System.Diagnostics;

namespace netCoreApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            span();


            arr();

            byte q = 0; while (true) q++;
        }


        static void span()
        {
            var nm = "span";
            Console.WriteLine("Press smth to start " + nm); Console.ReadLine();
            var sw = new Stopwatch(); sw.Restart();

            var le = 190_000;
            // Span<long> s = new Span<long>();
            Span<long> s = stackalloc long[le];
            //s.Fill(12345432);
            for (int i = 0; i < le; i++) s[i] = i;
            //for (int i = 0; i < le; i++)
                for (int j = 0; j < le; j++)
                    s[j] = s[j]++;
            //foreach (var item in s) Console.Write($"{item} ");

            sw.Stop(); Console.WriteLine(sw.Elapsed);
            Console.WriteLine(nm + " DONE. Press smth");
            Console.ReadLine();
            GC.Collect();
        }

        static void arr()
        {
            //var nm = new StackTrace()GetFrame(0)GetMethod();
            var nm = "arr";
            Console.WriteLine("Press smth to start " + nm); Console.ReadLine();
            var sw = new Stopwatch(); sw.Restart();

            var le = 190_000;
            // Span<long> s = new Span<long>();
            long[] s = new long[le];
            //s.Fill(12345432);
            for (int i = 0; i < le; i++) s[i] = i;
            //for (int i = 0; i < le; i++)
                for (int j = 0; j < le; j++)
                    s[j] = s[j]++;
            //foreach (var item in s) Console.Write($"{item} ");

            sw.Stop(); Console.WriteLine(sw.Elapsed);
            Console.WriteLine(nm + " DONE. Press smth"); Console.ReadLine();
            GC.Collect();
        }
    }
}
