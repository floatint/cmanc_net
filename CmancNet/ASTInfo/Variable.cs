using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTInfo
{
    class Variable : ISymbol
    {
        bool ISymbol.IsArgument() => false;

        bool ISymbol.IsNative() => false;

        bool ISymbol.IsSubroutine() => false;

        bool ISymbol.IsVariable() => true;
    }
}
