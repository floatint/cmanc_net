using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST
{
    class ASTUnarNotOpNode : ASTExpressionNode
    {
        public ASTExpressionNode Expression { set; get; }

        public override IList<ASTNode> Children => new List<ASTNode> { Expression };

        public ASTUnarNotOpNode(CmanParser.UnarOpContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
        }

        public ASTUnarNotOpNode(CmanParser.UnarOpContext context, ASTExpressionNode expr, ASTNode parent)
            : this(context, parent)
        {
            Expression = expr;
        }
    }
}
