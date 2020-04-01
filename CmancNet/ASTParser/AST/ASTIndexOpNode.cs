using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST
{
    class ASTIndexOpNode : ASTExpressionNode
    {
        public ASTExpressionNode Expression { set; get; }
        public ASTExpressionNode Index { set; get; }

        public ASTIndexOpNode(CmanParser.IndexStatementContext context, ASTExpressionNode expr, ASTExpressionNode index, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
        }
    }
}
