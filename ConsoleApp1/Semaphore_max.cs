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

namespace ConsoleApp1
{
    static class TryCatch
    {
        static public bool Wrap(Action action)
        {
            try
            {
                action();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        static async public Task<bool> WrapAsyc(Func<Task> action)
        {
            try
            {
                await action();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }




    static class Semaphore_max
    {
        // OutOfMemoryException.
        static public void test0()
        {
            var sp = new SemaphoreSlim(1, 1);// ~ lock
            var cntr = 0;
            while (true)
            {
                var isDone = TryCatch.Wrap(() =>
                {
                    sp.WaitAsync();
                    cntr++;
                });

                if (!isDone)
                    break;
            }

            Console.WriteLine("while true ERR. cntr=" + cntr);
        }

        static public void test00()
        {
            var sp = new SemaphoreSlim(1, 1);// ~ lock
            var cntr = 0;
            while (true)
            {
                sp.WaitAsync(10000);
                cntr++;
            }

            Console.WriteLine("while true ERR. cntr=" + cntr);
        }

        // OutOfMemoryException. долго ждать
        static public void test2()
        {
            var sp = new SemaphoreSlim(1, 1);// ~ lock
            var cntr = 0;
            while (true)
            {
                Task.Run(() =>
                    TryCatch.Wrap(() =>
                    {
                        Console.Write(cntr + " ");
                        sp.WaitAsync();
                        cntr++;
                        //Thread.Sleep(10);
                    })
                );

            }
        }


        static public void test3()
        {
            var sp = new SemaphoreSlim(1, 1);// ~ lock
            var cntr = 0;
            while (true)
            {
                Task.Run(async () =>
                    await TryCatch.WrapAsyc(async () =>
                    {
                        //Console.Write(cntr + " ");
                        cntr++;
                        await sp.WaitAsync();
                    })
                );

            }
        }

        // новые таски кажется имеют бьольший приоритет
        static volatile int test30_cntr = 0;
        static public void test30()
        {
            var sp = new SemaphoreSlim(1, 1);// ~ lock
            while (test30_cntr < 1000)
            {
                Task.Run(async () =>
                {
                    Console.Write(test30_cntr + " ");
                    test30_cntr++;
                    var res = await sp.WaitAsync(1000).ConfigureAwait(true);
                    test30_cntr--;
                    Console.Write(test30_cntr + " ");
                }
                );

            }
        }

    }
}
