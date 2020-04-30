using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.Compiler.ASTInfo
{
    interface ISymbol
    {
        bool IsSubroutine();
        bool IsVariable();
        bool IsNative();
        bool IsArgument();
    }
}
