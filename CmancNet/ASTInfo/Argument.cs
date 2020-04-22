using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTInfo
{
    class Argument : ISymbol
    {
        public bool IsArgument() => true;

        public bool IsNative() => true;

        public bool IsSubroutine() => false;

        public bool IsVariable() => false;
    }
}
