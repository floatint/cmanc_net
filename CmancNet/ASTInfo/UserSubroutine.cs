using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmancNet.ASTParser.AST.Statements;

namespace CmancNet.ASTInfo
{
    class UserSubroutine : ISubroutine
    {
        public int ArgumentsCount
        {
            get
            {
                return Node.ArgList is null ? 0 : Node.ArgList.Arguments.Count();
            }
        }

        public bool Return { set; get;}

        public bool IsNative()
        {
            return false;
        }

        public bool IsSubroutine()
        {
            return true;
        }

        public bool IsVariable()
        {
            return false;
        }

        public bool IsArgument() => false;

        public ASTSubStatementNode Node { private set; get; }
        

        public UserSubroutine(ASTSubStatementNode node)
        {
            Node = node;
            _locals = new Dictionary<string, ISymbol>();
        }

        public ISymbol FindLocal(string name)
        {
            _locals.TryGetValue(name, out ISymbol sym);
            return sym;
        }

        public void AddLocal(string name, ISymbol local)
        {
            if (_locals.ContainsKey(name))
                return;
            _locals.Add(name, local);
        }

        private Dictionary<string, ISymbol> _locals;
    }
}
