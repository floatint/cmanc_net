using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using CmancNet.Compiler.Utils.Logging;

namespace CmancNet.Compiler
{
    public interface ICompiler
    {
        AssemblyBuilder Compile(string sourcePath);
        IEnumerable<MessageRecord> Messages { get; }
        bool Error { get; }
    }
}
