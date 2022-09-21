using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Problem01
{
    class Program
    {
        static byte[] Data_Global = new byte[1000000000];
        static long Sum_Global = 0;
        static int THREAD_AMOUNT = Environment.ProcessorCount; //Environment.ProcessorCount return Max processor in our Environment

        static int ReadData()
        {
            int returnData = 0;
            FileStream fs = new FileStream("Problem01.dat", FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();

            try 
            {
                Data_Global = (byte[]) bf.Deserialize(fs);
            }
            catch (SerializationException se)
            {
                Console.WriteLine("Read Failed:" + se.Message);
                returnData = 1;
            }
            finally
            {
                fs.Close();
            }

            return returnData;
        }
        static long sum(int firstIndex, int lastIndex)
        {
            long sumOfTask = 0;
            for(int i = firstIndex ; i < lastIndex ; i++)
            {
                
                if ((((int)Data_Global[i]) & 1) == 0) //modulo 2
                {
                    sumOfTask -= Data_Global[i];
                }
                else if (((((short)Data_Global[i]) * 0x55555556L >> 30) & 3) == 0) //modulo 3
                {
                    sumOfTask += (Data_Global[i] << 1); //multiply by 2
                }
                else if (Data_Global[i] % 5 == 0)
                {
                    sumOfTask += (Data_Global[i] >> 1); // divided by 2
                }
                else if (Data_Global[i] % 7 == 0)
                {
                    sumOfTask += (Data_Global[i] / 3);
                }
                Data_Global[i] = 0;
            }
            return sumOfTask;
        }

        static void worker(object num)
        {
            long Sum_Local = 0;
            //(int)(1000000000 * ((int)num*1.0 / THREAD_AMOUNT*1.0)) is a first index of this Thread
            //(int)(1000000000 * (((int)num + 1)*1.0 / THREAD_AMOUNT*1.0)) is a last index of this Thread
            Sum_Local = sum((int)(1000000000 * ((int)num*1.0 / THREAD_AMOUNT*1.0)), (int)(1000000000 * (((int)num + 1)*1.0 / THREAD_AMOUNT*1.0))); 
            Sum_Global+=Sum_Local;
        }

        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            int y;

            Thread[] allThread = new Thread[THREAD_AMOUNT];

            /* Read data from file */
            Console.Write("Data read...");
            y = ReadData();
            if (y == 0)
            {
                Console.WriteLine("Complete.");
            }
            else
            {
                Console.WriteLine("Read Failed!");
            }


            /* Start */
            Console.Write("\n\nWorking...");
            sw.Start();

            for (int i=0; i<THREAD_AMOUNT; i++)
            {
                //Start New Thread
                allThread[i] = new Thread(worker);
                allThread[i].Start(i);
            }
            for (int i=0; i<THREAD_AMOUNT; i++)
                //Waiting All Thread finish our Task
                allThread[i].Join();               

            sw.Stop();
            Console.WriteLine("Done.");           
            /* STOP */

            /* Result */
            Console.WriteLine("Summation result: {0}", Sum_Global);
            Console.WriteLine("Time used: " + sw.ElapsedMilliseconds.ToString() + "ms");
        }
    }
}
