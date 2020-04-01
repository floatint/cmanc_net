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

        public ASTBodyNode(CmanParser.BodyStatementContext context, ASTNode parent) : base(parent)
        {
            SetLocation(context);
        }
    }
}
