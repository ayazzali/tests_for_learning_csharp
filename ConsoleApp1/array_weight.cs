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
using System.Collections.Concurrent;

namespace ConsoleApp1
{
    public static class Array_weight
    {
        // медленный
        static public void test0()
        {
            var list = new BlockingCollection<decimal>();
            for (int i = 0; i < 60_000_000; i++)//1 098 MB !!!
            {
                list.Add(
                    ///* !!! */i +
                    00000000111333111333111333111.1117771117771117771117771117771117771117771117777111777111777111777111777111777111777M);
            }
            //list.CompleteAdding();
        }

        // !!!
        static public void test6()
        {
            var list = new ConcurrentQueue<decimal>();
            for (int i = 0; i < 60_000_000; i++)//1 098 MB 
            {
                list.Enqueue(/* !!! */i + 00000000111333111333111333111.1117771117771117771117771117771117771117771117777111777111777111777111777111777111777M);
            }
        }

        // !!!
        static public void test5()
        {
            var list = new ConcurrentQueue<decimal>();
            for (int i = 0; i < 60_000_000; i++)//1 115 MB 
            {
                list.Enqueue(11133311133311133311133311100.1117771117771117771117771117771117771117771117777111777111777111777111777111777111777M);
            }
        }

        //медленный
        static public void test4()
        {
            var list = new ConcurrentStack<decimal>();
            for (int i = 0; i < 60_000_000; i++)//1 629 MB
            {
                list.Push(11133311133311133311133311100.1117771117771117771117771117771117771117771117777111777111777111777111777111777111777M);
            }
        }

        static public void test3()
        {
            var list = new List<decimal>();
            for (int i = 0; i < 60_000_000; i++)//1 436 MB
            {
                list.Add(11133311133311133311133311100.1117771117771117771117771117771117771117771117777111777111777111777111777111777111777M);
            }
        }

        static public void test2()
        {
            var list = new Queue<decimal>();
            for (int i = 0; i < 60_000_000; i++)//1 429 MB
            {
                list.Enqueue(11133311133311133311133311100.1117771117771117771117771117771117771117771117777111777111777111777111777111777111777M);
            }
        }

        static public void test1()
        {
            var list = new Stack<decimal>();
            for (int i = 0; i < 60_000_000; i++)//1 429 MB
            {
                list.Push(11133311133311133311133311100.1117771117771117771117771117771117771117771117777111777111777111777111777111777111777M);
            }
        }
    }
}
