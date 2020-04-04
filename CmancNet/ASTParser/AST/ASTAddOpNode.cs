using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST
{
    class ASTAddOpNode : ASTExpressionNode
    {
        public ASTExpressionNode Left { set; get; }
        public ASTExpressionNode Right { set; get; }

        public ASTAddOpNode(CmanParser.AddOrSubOpContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context); 
        }

        public override IList<ASTNode> Children => new List<ASTNode> { Left, Right };
    }
}
