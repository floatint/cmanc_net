using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST
{
    class ASTWhileStatementNode : ASTStatementNode
    {
        public ASTExpressionNode Condition { set; get; }
        public ASTBodyNode Body { set; get; }

        public ASTWhileStatementNode(CmanParser.WhileStatementContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
        }

        public override IList<ASTNode> Children => new List<ASTNode> { Condition, Body };

    }
}
