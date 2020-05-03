using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.Compiler.ASTParser.AST.Expressions
{
    class ASTNullLiteralNode : ASTNode, IASTExprNode, IASTLiteral
    {
        public ASTNullLiteralNode(CmanParser.NullContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
        }

        public override IList<ASTNode> Children => throw new NotImplementedException();
    }
}
