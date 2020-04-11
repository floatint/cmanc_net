using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTInfo
{
    interface ISymbol
    {
        bool IsSubroutine();
        bool IsVariable();
        bool IsNative();
    }
}
