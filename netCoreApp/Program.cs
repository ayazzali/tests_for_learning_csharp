using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace netCoreApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            locks();

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
}
