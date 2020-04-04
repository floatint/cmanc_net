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

        public ASTUnarMinusOpNode(CmanParser.UnarOpContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
        }

        public override IList<ASTNode> Children => new List<ASTNode> { Expression };

    }
}
