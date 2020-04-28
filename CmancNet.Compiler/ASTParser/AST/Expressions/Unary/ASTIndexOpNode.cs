using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.Compiler.ASTParser.AST.Expressions.Unary
{
    class ASTIndexOpNode : ASTNode, IASTUnarOpNode
    {
        public IASTExprNode Expression { set; get; }
        public IASTExprNode Index { set; get; }

        public override IList<ASTNode> Children => 
            new List<ASTNode> { (ASTNode)Expression, (ASTNode)Index };

        public ASTIndexOpNode(CmanParser.IndexOpContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
        }
    }
}
