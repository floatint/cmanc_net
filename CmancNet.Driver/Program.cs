using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using CmancNet.Compiler;

namespace CmancNet.Driver
{
    class Program
    {
        static void Main(string[] args)
        {
            ICompiler compiler = new CmancCompiler();
            AssemblyBuilder assembly = compiler.Compile("example_2.txt");
            //set entry point
            if (!compiler.Error)
            {
                assembly.SetEntryPoint(assembly.GetType("Program").GetMethod("main"));
                assembly.Save("example_2.exe");
            }
            foreach (var m in compiler.Messages)
                Console.WriteLine(m);
        }
    }
}
