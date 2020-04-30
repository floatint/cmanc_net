using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.Compiler.ASTParser.AST.Expressions.Binary
{
    class ASTAddOpNode : ASTAbstractBinOpNode
    {
        //public IASTExprNode Left { set; get; }
        //public IASTExprNode Right { set; get; }

        public ASTAddOpNode(CmanParser.AddOrSubOpContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context); 
        }

        //public override IList<ASTNode> Children => 
        //    new List<ASTNode> { (ASTNode)Left, (ASTNode)Right };
    }
}
