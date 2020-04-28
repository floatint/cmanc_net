using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.Compiler.ASTInfo
{
    interface ISubroutine : ISymbol
    {
        int ArgumentsCount { get; }
        bool Return { get; }
    }
}
