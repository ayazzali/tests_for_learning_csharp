using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Xml.Serialization;
using System.Runtime.InteropServices;

namespace ConsoleApp1
{
    class finClass
    {
        long[] arr = new long[1000_000];

        //public finClass() => Console.WriteLine("finClass ctor " + Thread.CurrentThread.ManagedThreadId);
        public finClass()
        {
            Console.WriteLine("finClass ctor " + Thread.CurrentThread.ManagedThreadId);
            for (int i = 0; i < arr.Length; i++)
                arr[i] = i;
        }

        ~finClass()
        {
            Console.WriteLine("finClass ~" + Thread.CurrentThread.ManagedThreadId);
            //Thread.Sleep(60 * 1 * 60);
            //throw new Exception("123");
        }
    }

    [DebuggerDisplay("val = {val}")]
    struct Number : IEquatable<int>
    {
        int val;

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public bool Equals(int other)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


    }
    //[Serializable] 
    internal class qweClass : MarshalByRefObject
    {
        static decimal q = 100;
        static byte counter;
        static void l(long o) { Console.WriteLine("\n" + ++counter + ": " + o / 10000 + "\n"); }
        static void l(object o) { Console.WriteLine("\n" + Thread.CurrentThread.ManagedThreadId + " " + ++counter + ": " + o + "\n"); }
        internal static void method() { l(01); q--; Thread.Sleep(1000); }
        static Action a;
        public string _method()
        {
            qweClass.method();
            return "000";


        }
        public static void methodWithWr(object f)
        {
            // l(DateTime.Now.Ticks - ((DateTime)f).Ticks);
            var ff = (Stopwatch)f;
            l(ff.ElapsedTicks);
            if (a != null) a();
            a = method;
            //    while (true)
            //        counter++;
            method();
        }
    }

    public class Program : MarshalByRefObject
    {
        static int counter;
        static void t() => Thread.Sleep(10000);
        static void l(long o) { Console.WriteLine("\n" + ++counter + ": " + o / 1 + "\n"); }

        static void methodWait() { Thread.Sleep(int.MaxValue); }
        static void testforeground()
        {
            var f = new Thread(methodWait);
            f.IsBackground = false;
            f.Start();
        }
        static void testAppDomain()
        {
            qweClass.method();
            var d = AppDomain.CreateDomain("qwed");
            ////var res = (qweClass)d.CreateInstanceAndUnwrap(/*Assembly.GetEntryAssembly().FullName*/"ConsoleApp1", "ConsoleApp1.qweClass");
            //var f=res._method();
            qweClass.method();
            var res = (Program)d.CreateInstanceAndUnwrap(/*Assembly.GetEntryAssembly().FullName*/"ConsoleApp1", "ConsoleApp1.Program");
            res._testMethodAndOperation();

            t();
        }

        public void _testMethodAndOperation() => testMethodAndOperation();
        static void testMethodAndOperation()
        {
            var sw = new Stopwatch();
            sw.Start();

            //for (int i = 0; i < 5; i++) { Thread.Sleep(100); }

            //l(sw.ElapsedTicks);
            //sw.Reset(); sw.Start();

            //for (int i = 0; i < 5; i++) { qweClass.method(); }

            //l(sw.ElapsedTicks);
            //sw.Reset(); sw.Start();

            ThreadPool.GetMinThreads(out int q, out int w);
            l(q);
            l(w);
            Thread.Sleep(500);
            try
            {

                for (int i = 0; i < 20; i++)
                {
                    // l(0);
                    //new Thread(qweClass.methodWithWr).Start(sw);
                    ThreadPool.QueueUserWorkItem(qweClass.methodWithWr, sw);
                }

            }
            catch
            {
                l(1);
            }
            Thread.Sleep(10000);
            //l(sw.ElapsedTicks);
            //sw.Reset(); sw.Start();
        }
        class test6DerivedWithNew : test6Class
        {
            public override int a { get; set; } = 3;
            //int f()
            //{
            //    return ++base.a;
            //}
        }
        class test6Class
        {
            public virtual int a { get; set; }
            public virtual int b { get; set; }
            public void ChangeA() => a++;
        }
        static void test6()
        {
            var f = new test6DerivedWithNew();
            f.ChangeA();
            l(f.a);
            l(f.b);
        }

        static void testAbortThread()
        {

            var thSt = new ThreadStart(testAbortQwe);
            var f = new Thread(thSt);
            f.Start();
            Thread.Sleep(300);
            f.Abort();
            f.Join();
            Console.WriteLine("Join done");
        }
        static void testAbortQwe()
        {
            Console.WriteLine("child()");

            //AppDomain.CurrentDomain.UnhandledException += 
            AppDomain.CurrentDomain.ProcessExit += (object o, EventArgs e) => { Console.WriteLine("убивают. prcossExit"); };

            while (true)
            {
                try
                {
                    Thread.Sleep(100);
                    Console.WriteLine("живу");
                }

                catch (ThreadAbortException ex)
                {
                    Console.WriteLine("меня хотят убить");
                    // Thread.ResetAbort();
                }
            }

        }
        [Serializable]
        class simpleInterlocked
        {
            int g;
            public void Enter()
            {
                while (true)
                {
                    if (Interlocked.Exchange(ref g, 1) == 0)
                        return;
                    //Thread.Sleep(10);
                    new AutoResetEvent(false).WaitOne(10);
                    //Thread.SpinWait(10000);
                }
            }
            public void Leave()
            {
                Interlocked.Exchange(ref g, 0);
            }
        }
        static void methodInterlocked(object simpleLock)
        {
            var sl = (simpleInterlocked)simpleLock;
            //Thread.Sleep(2000);
            for (var i = 0; i < 1000; i++)
            {
                sl.Enter();
                //System.Runtime.Serialization.ser

                Console.Write('.');
                //new DateTime(1234 + i * 90 / 3);
                Thread.Sleep(10);
                sl.Leave();
            }
        }
        static void testInterlokedSimple()
        {
            var sl = new simpleInterlocked();
            sl.Enter();
            for (int i = 0; i < 20; i++)
                new Thread(methodInterlocked).Start(sl);
            Thread.Sleep(5000);
            sl.Leave();
        }


        [Serializable]
        public struct Point<T> { public T x, y; private T z => x; }
        private static void OptInSerialization()
        {
            var pt = new Point<int> { x = 1, y = 2 };
            using (var stream = new FileStream("qwe.txt", FileMode.OpenOrCreate))
            {
                //var t = typeof(XmlSerializer);
                //var q = t.GetConstructor(new Type[0]);
                //var qqq = (XmlSerializer)  (q.Invoke(null));

                new XmlSerializer(typeof(List<Point<int>>)).Serialize(stream, new List<Point<int>> { new Point<int>() { x = 1, y = 2 }, new Point<int>() { x = 1, y = 999999999 } }); // исключение
                //new BinaryFormatter().Serialize(stream, pt);// new List<Point> { new Point() { x = 1, y = 2 }, new Point() { x = 1, y = 999999999 } }); // исключение
                stream.Position = 0;
                //var f=new XmlSerializer().Deserialize(stream);
            }
        }

        static void printAllAssemblies()
        {
            Console.SetWindowSize(300, 70);
            Console.CursorSize = 20;

            var w = new Action<object>((r) => Console.WriteLine(r));
            foreach (var assemb in AppDomain.CurrentDomain.GetAssemblies())
            {
                w(""); w("assembly " + assemb.FullName);
                foreach (var as1 in assemb.GetTypes().GroupBy(_ => _.Namespace))
                {
                    w(""); w("    namespase " + as1.Key);
                    foreach (var as2 in as1)
                    {
                        w("        " + (as2.IsVisible ? "" : "* ") + as2.Name + "  " + as2.FullName);
                        Thread.Sleep(as2.IsVisible ? 500 : 100);
                    }
                }
            }
        }




        public class MyInt { public int MyValue; }
        public static void Go()
        {
            MyInt x = new MyInt();
            x.MyValue = 2;
            //DoSomething(x);
            DoSomethingRef(ref x);

            Console.WriteLine(x.MyValue.ToString());
        }
        public static void DoSomething(MyInt pValue)
        {
            pValue = new MyInt();
        }
        public static void DoSomethingRef(ref MyInt pValue)
        {
            pValue = new MyInt();
        }



        public static void thread20000()
        {
            Thread.Sleep(2000);
            Console.WriteLine($"{GC.GetTotalMemory(false):N0}");
            for (var i = 0; i <= 10000; i++)
            {
                Thread thread = new Thread(Test, 200_000);
                thread.IsBackground = true;

                thread.Start();
            }
            Console.WriteLine($"{GC.GetTotalMemory(false):N0}");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Console.WriteLine($"{GC.GetTotalMemory(false):N0}");
        }

        public static void Test()
        {
            Thread.Sleep(20000);
        }


        //static void qweExc(object sender, /*UnhandledExceptionEventArgs */ EventArgs e)
        //{
        //    Console.WriteLine("меня хотят убить");
        //    //Thread.ResetAbort();
        //}

        //[DllImport("kernel32.dll", SetLastError = true)]
        //internal static extern bool WriteConsoleOutputCharacter(
        // IntPtr hConsoleOutput,
        // StringBuilder lpCharacter,
        // uint nLength,
        // COORD dwWriteCoord,
        // out uint lpNumberOfCharsWritten);
        [DllImport("user32.dll")]
        private static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);


        static Action Foo()
        {
            var actions = new List<Action>();
            for (int i = 0; i < 10; i++)
                actions.Add(() => Console.WriteLine(i));

            foreach (var i in Enumerable.Range(11, 15))
                actions.Add(() => Console.WriteLine(i));

            var q = 4;
            actions.Add(() => Console.WriteLine(q));
            q = 44;

            foreach (var a in actions)
                a();
            return actions.Last();
        }


        static void Main(string[] args)
        {
            //testAbortThread
            //testforeground();
            //testMethodAndOperation();
            // while (true) { counter++; }
            // testAppDomain();
            // test6();
            //testInterlokedSimple();
            //OptInSerialization();

            //printAllAssemblies();

            //Semaphore_max.test30();

            //Array_weight.test1();
            //Array_weight.test2();
            //Array_weight.test3();
            //Array_weight.test4();

            //Array_weight.test5();
            //Array_weight.test6();


            //var q1 = new Number();
            //_ = q1.Equals(1);


            //Go(); return;


            //{
            //    ThreadPool.GetMaxThreads(out var t, out var io);
            //    Console.WriteLine($"ThreadPool.GetMaxThreads {t} {io}");
            //}
            //{
            //    ThreadPool.GetMinThreads(out var t, out var io);
            //    Console.WriteLine($"ThreadPool.GetMinThreads {t} {io}");
            //}
            //{
            //    ThreadPool.GetAvailableThreads(out var t, out var io);
            //    Console.WriteLine($"ThreadPool.GetAvailableThreads {t} {io}");
            //}
            //ThreadPool.SetMinThreads(0, 0);
            //ThreadPool.SetMaxThreads(0, 0);
            //{
            //    ThreadPool.GetMinThreads(out var t, out var io);
            //    Console.WriteLine($"ThreadPool.GetMinThreads {t} {io}");
            //}
            //{
            //    ThreadPool.GetMaxThreads(out var t, out var io);
            //    Console.WriteLine($"ThreadPool.GetMaxThreads {t} {io}");
            //}
            //{
            //    ThreadPool.GetAvailableThreads(out var t, out var io);
            //    Console.WriteLine($"ThreadPool.GetAvailableThreads {t} {io}");
            //}


            //{
            //    Console.WriteLine($"{GC.GetTotalMemory(false):N0}");
            //    Thread.Sleep(1000);
            //    var list = new List<object>();
            //    int i = 0;
            //    try
            //    {
            //        for (; i < 1_00; i++) list.Add(new finClass());
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"{GC.GetTotalMemory(false):N0}");
            //        Console.WriteLine("catch");
            //        Console.WriteLine(ex);
            //        list.RemoveAt(1);
            //    }
            //    Console.WriteLine("i=" + i);
            //}


            //thread20000();


            //while(true) 
            //if (Process.GetCurrentProcess().PriorityClass != ProcessPriorityClass.Idle)
            //{
            //    Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Idle;
            //    var psi = new ProcessStartInfo() { FileName = Assembly.GetExecutingAssembly().Location, CreateNoWindow = true, UseShellExecute = false };
            //    for (int i = 0; i < 50; i++)
            //        Process.Start(psi);
            //}


            //while (true) { Marshal.AllocHGlobal(1024); }


            //MessageBox(new IntPtr(0), "Hello, world!", "My box", 0);


            var qq = Foo();
            //qq();

            var f = new int[1].SingleOrDefault();
            Console.WriteLine(f);

            Thread.Sleep(100_000);
        }

    }
}