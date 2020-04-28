using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.Compiler.ASTParser.AST.Expressions.Unary
{
    class ASTMinusOpNode : ASTNode, IASTUnarOpNode
    {
        public IASTExprNode Expression { set; get; }

        public ASTMinusOpNode(CmanParser.UnarOpContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
        }

        public override IList<ASTNode> Children => new List<ASTNode> { (ASTNode)Expression };

    }
}
