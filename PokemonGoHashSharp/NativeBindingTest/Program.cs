﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NativeBindingTest
{
   class Program
   {
      static void Main(string[] args)
      {
         var sampleOutputNative = TestNative();
         var sampleOutputManaged = TestManaged();

         for (var i = 0; i < Math.Min(sampleOutputNative.Length, sampleOutputManaged.Length); i++)
         {
            if (sampleOutputNative[i] != sampleOutputManaged[i])
            {
               throw new Exception($"Output buffer difference at index {i}: native is {sampleOutputNative[i]}, managed is {sampleOutputManaged[i]}");
            }
         }

         if (sampleOutputManaged.Length != sampleOutputNative.Length)
         {
            throw new Exception($"Output length differences: native is {sampleOutputNative.Length}, managed is {sampleOutputManaged.Length}");
         }

         PerfTest();

      }

      static void PerfTest()
      {
         double rounds = 10000;
         var sw = new Stopwatch();

         sw.Start();
         for (var i = 0; i < rounds; i++)
         {
            TestNative();
         }
         sw.Stop();
         var nativeTook = sw.Elapsed;

         sw.Restart();
         for (var i = 0; i < rounds; i++)
         {
            TestManaged();
         }
         sw.Stop();
         var managedTook = sw.Elapsed;

         var avgNative = TimeSpan.FromTicks((long)(nativeTook.Ticks / rounds));
         var avgManaged = TimeSpan.FromTicks((long)(managedTook.Ticks / rounds));

         // 16x
         var diffx = managedTook.TotalSeconds / nativeTook.TotalSeconds;

         Console.WriteLine($"native took {avgNative.TotalMilliseconds} ms per round");
         Console.WriteLine($"managed took {avgManaged.TotalMilliseconds} ms per round");
         Console.WriteLine($"managed took {Math.Round(diffx, 2)} times the amount of time");
         Console.WriteLine($"native {nativeTook.TotalSeconds} vs managed {managedTook.TotalSeconds}");
      }

      static byte[] TestNative()
      {
         byte[] input;
         byte[] iv;
         GetSampleData(out input, out iv);

         var output = PokemonGoHashNative.Util.Hash(input, iv);
         return output;
      }

      static byte[] TestManaged()
      {
         byte[] input;
         byte[] iv;
         GetSampleData(out input, out iv);

         var output = PokemonGoHashSharp.Util.Hash(input, iv);
         return output;
      }

      static void GetSampleData(out byte[] input, out byte[] iv)
      {
         input = new byte[5000];
         for (var i = 0; i < input.Length; i++)
         {
            input[i] = (byte)i;
         }
         iv = new byte[32];
         for (var i = 0; i < iv.Length; i++)
         {
            iv[i] = (byte)i;
         }
      }
   }
}
