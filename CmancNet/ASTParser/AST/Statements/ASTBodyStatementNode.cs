using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST.Statements
{
    class ASTBodyStatementNode : ASTNode, IASTStatementNode
    {
        public IList<IASTStatementNode> Statements { set; get; }

        public ASTBodyStatementNode(CmanParser.BodyStatementContext context, ASTNode parent) : base(parent)
        {
            SetLocation(context);
        }

        public override IList<ASTNode> Children => Statements.Cast<ASTNode>().ToList();
    }
}
