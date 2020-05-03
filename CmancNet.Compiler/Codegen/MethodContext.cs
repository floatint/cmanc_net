using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using CmancNet.Compiler.ASTInfo;

namespace CmancNet.Compiler.Codegen
{
    /// <summary>
    /// Contains all information for code generation 
    /// </summary>
    class MethodContext
    {
        public MethodContext(MethodBuilder methodBuilder, Dictionary<string, ISymbol> locals)
        {
            Builder = methodBuilder;
            ILGenerator = Builder.GetILGenerator();
            _locals = new Dictionary<string, LocalBuilder>();
            _args = new Dictionary<string, int>();
            MethodEnd = ILGenerator.DefineLabel();
            //define locals
            foreach (var l in locals)
            {
                DefineSymbol(l.Key, l.Value);
            }
        }

        private void DefineSymbol(string name, ISymbol symbol)
        {
            if (symbol is Argument)
                _args.Add(name, _argCounter++);
            if (symbol is Variable)
                _locals.Add(name, ILGenerator.DeclareLocal(typeof(object)));
        }

        public bool IsLocal(string name) => _locals.ContainsKey(name);

        public LocalBuilder GetLocal(string s)
        {
            return _locals[s];
        }

        public int GetArgID(string s)
        {
            return _args[s];
        }

        public ILGenerator ILGenerator { private set; get; }
        public MethodBuilder Builder { private set; get; }
        public Label MethodEnd { private set; get; }

        private Dictionary<string, LocalBuilder> _locals;
        private Dictionary<string, int> _args;
        private int _argCounter;
    }
}
