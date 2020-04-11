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

        public bool Return { private set; get;}

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

        public ASTSubStatementNode Node { private set; get; }
        public IList<ISymbol> LocalVariables { private set; get; }

        public UserSubroutine(ASTSubStatementNode node)
        {
            Node = node;
            
        }
    }
}
