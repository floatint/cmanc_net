using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTInfo
{
    class SystemEnvironment : SymbolTable
    {
        public SystemEnvironment()
        {
            base.AddSymbol(
                "print",
                new NativeSubroutine(
                typeof(Console).GetMethod("Write", new Type[] { typeof(object) }), 1
                ));
            base.AddSymbol(
                "println",
                new NativeSubroutine(
                typeof(Console).GetMethod("WriteLine", new Type[] { typeof(object) }), 1
            ));
        }
    }
}
