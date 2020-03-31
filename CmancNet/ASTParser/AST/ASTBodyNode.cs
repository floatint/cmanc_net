using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST
{
    class ASTBodyNode : ASTNode
    {
        public IList<ASTStatementNode> Statements { set; get; }

        public ASTBodyNode(ASTNode parent) : base(parent)
        {
            Statements = new List<ASTStatementNode>();
        }
    }
}
