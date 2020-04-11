using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace CmancNet
{
    public interface ICompiler
    {
        AssemblyBuilder Compile(string sourcePath);
        bool HasErrors();
        IList<string> Errors { get; }
    }
}
