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
                    typeof(Console).GetMethod("Write", new Type[] { typeof(object) }), 
                    1,
                    null
                ));
            base.AddSymbol(
                "println",
                new NativeSubroutine(
                    typeof(Console).GetMethod("WriteLine", new Type[] { typeof(object) }),
                    1,
                    null
            ));

            base.AddSymbol(
                "array",
                new NativeSubroutine(
                    typeof(Activator).GetMethod("CreateInstance", Type.EmptyTypes).MakeGenericMethod(typeof(Dictionary<object, object>)),
                    0,
                    typeof(Dictionary<object, object>))
                );

            base.AddSymbol(
                "type",
                new NativeSubroutine(
                    typeof(object).GetMethod("GetType", new Type[] { }),
                    1,
                    typeof(Type)
                    )
                );

            base.AddSymbol(
                "str",
                new NativeSubroutine(
                    typeof(Convert).GetMethod("ToString", new Type[] { typeof(object)}),
                    1,
                    typeof(string)
                    )
                );

            base.AddSymbol(
                "decimal",
                new NativeSubroutine(
                    typeof(Convert).GetMethod("ToDecimal", new Type[] { typeof(object) }),
                    1,
                    typeof(decimal)
                    )
                );
        }
    }
}
