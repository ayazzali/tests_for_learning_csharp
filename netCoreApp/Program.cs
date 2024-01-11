using Org.BouncyCastle.Pkcs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace netCoreApp
{
    class Program
    {
        static void Main(string[] args)
        {
            PFX_SN();
            var f=GetSignCertificate("Eight CM.PFX", "Safety01");

            Console.WriteLine("Hello World!");

            //IEnumerable<String> strings = new List<String>();
            //IEnumerable<Object> objects = strings;
            //var f = (List<int>)objects; // ERR 

            ProtoTest.qwe().GetAwaiter().GetResult();

            //MainAync().GetAwaiter().GetResult();

            //locks();

            //span();


            //arr();

            Console.WriteLine("END");
            Console.ReadKey(); return;
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

        static void locks()
        {
            var m_t_variable = 1;
            Volatile.Write(ref m_t_variable, 99);
            var yi = Thread.Yield();
            if (!yi)
                Console.WriteLine("there is no other threads to execute :(");


            //Thread.SpinWait(111222333); // READ Win32- эквиваленты:.Sleep,.SwitchToThread.и.YieldProcessor


            var sl = new SpinLock();
            Console.WriteLine(sl.GetHashCode());
            var sl_lock = false;
            sl.Enter(ref sl_lock);
            sl.Exit();
            //sl.TryEnter(ref sl_lock); // use anither sl_lock! or set to false


            //var are = new ManualResetEvent(false);
            var are = new AutoResetEvent(false);
            //var are = new AutoResetEvent(true);
            new Thread(_ =>
            {
                Thread.Sleep(100);
                Console.WriteLine("are.Set ...");
                are.Set();
                Console.WriteLine("are.Set DONE");
            }).Start();
            new Thread(_ =>
            {
                Console.WriteLine("are.WaitOne t2 ...");
                are.WaitOne();
                Console.WriteLine("are.WaitOne t2 DONE");
            }).Start();
            are.WaitOne();
            Console.WriteLine("WaitOne DONE");
            // are.Dispose();
            //are.WaitOne();
            //Console.WriteLine("WaitOne 2 DONE"); // will not run if u use AutoResetEvent. because it resets event automatically.


            var sf = new Semaphore(10, 10); // like an empty bus :)
            //sf.Release(); // err


            var mx = new Mutex();
            //mx.WaitOne();
            mx.WaitOne(); // runs anyway

            new Thread(_ =>
            {
                mx.WaitOne();
                //mx.ReleaseMutex();
                Thread.Sleep(100);
                Thread.Sleep(100);


                Console.WriteLine("[mx]   new Thread closing ...");
            }).Start();
            Thread.Sleep(100);
            //mx.WaitOne();// ERR because other thread is abandoned mx
            //mx.ReleaseMutex();

            mx.WaitOne();
            mx.ReleaseMutex();


            var q = new BlockingQueue<int>();
            new Thread(_ =>
            {
                Console.WriteLine("[Queue]   new Thread ...");
                q.Enqueue(2);
            }).Start();

            //Thread.Sleep(10);
            q.Dequeue();
        }

        public static async Task MainAync()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            var doing = DoAync(cts.Token);
            //cts.Cancel();
            await doing;
        }

        public static async Task DoAync(CancellationToken ct)
        {
            await Task.Delay(100);
            await Task.Run(() => throw new Exception(""), ct)
               .ContinueWith(_ =>
               {
                   Task.Delay(100).Wait();
                   Console.WriteLine(_.Exception.Message);
                   return _;
               }, ct, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Current)
               .ContinueWith(_ =>
               {
                   Task.Delay(100).Wait();
                   Console.WriteLine(_.Exception.Message);
                   return _;
               }, ct)
               .Unwrap()
               .Unwrap()
               ;
        }

        public static void PFX_SN()
        {
            var cert = new X509Certificate2("acp_test_sign.pfx", "000000");
            Console.WriteLine("SerialNumber: " + cert.SerialNumber);

            //var cert = new X509Certificate("acp_test_sign.pfx", "000000");
            //Console.WriteLine("SerialNumber: " + cert.GetSerialNumberString());
        }

        public static object GetSignCertificate(string certificate, string certPwd)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(certificate, FileMode.Open, FileAccess.Read);
                var store = new Pkcs12Store(fs, certPwd.ToCharArray());

                var alias = string.Empty;
                foreach (string n in store.Aliases)
                {
                    if (store.IsKeyEntry(n))
                    {
                        alias = n;
                    }
                }

                var chain = store.GetCertificateChain(alias);
                var cert = chain[0].Certificate;
                return new
                {
                    key = store.GetKey(alias).Key,
                    cert = cert,
                    certId = cert.SerialNumber.ToString(),
                };
                //return new UnionPayCertificate
                //{
                //    key = store.GetKey(alias).Key,
                //    cert = cert,
                //    certId = cert.SerialNumber.ToString()
                //};

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            return null;
        }

    }
    internal sealed class SynchronizedQueue<T>
    {
        private readonly Object m_lock = new Object();
        private readonly Queue<T> m_queue = new Queue<T>();
        public void Enqueue(T item)
        {
            Monitor.Enter(m_lock);
            // После постановки элемента в очередь пробуждаем 
            // один/все ожидающие потоки
            m_queue.Enqueue(item);
            Monitor.PulseAll(m_lock);
            Monitor.Exit(m_lock);
        }
        public T Dequeue()
        {
            Monitor.Enter(m_lock);
            // Выполняем цикл, пока очередь не опустеет (условие)
            while (m_queue.Count == 0)
                Monitor.Wait(m_queue); // potential ERR !!!
            // Удаляем элемент из очереди и возвращаем его на обработку
            T item = m_queue.Dequeue();
            Monitor.Exit(m_lock);
            return item;
        }
    }
    public class BlockingQueue<T> // i dont think its good
    {
        private Queue<T> queue = new Queue<T>();

        public void Enqueue(T obj)
        {
            lock (queue)
            {
                queue.Enqueue(obj);
                Monitor.Pulse(queue);
            }
        }

        public T Dequeue()
        {
            T obj;
            lock (queue)
            {
                while (queue.Count == 0)
                {
                    Monitor.Wait(queue);
                }
                obj = queue.Dequeue();
            }
            return obj;
        }
    }

    public interface IQwe<out T>
    {

    }

}
