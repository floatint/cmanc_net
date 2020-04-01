using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST
{
    class ASTUnarMinusOpNode : ASTExpressionNode
    {
        public ASTExpressionNode Expression { set; get; }

        public ASTUnarMinusOpNode(CmanParser.UnarOpContext context, ASTExpressionNode expr, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
            Expression = expr;
        }
    }
}
