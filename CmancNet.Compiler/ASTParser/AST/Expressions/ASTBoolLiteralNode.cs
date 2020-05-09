using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.Compiler.ASTParser.AST.Expressions
{
    class ASTBoolLiteralNode : ASTNode, IASTExprNode, IASTLiteral, IASTLogicOpNode
    {
        public bool Value { private set; get; }

        public ASTBoolLiteralNode(CmanParser.BoolLiteralContext context, ASTNode parent)
            : base(parent)
        {
            if (context.TRUE() != null)
                Value = true;
            else
                Value = false;
            SetLocation(context);
        }

        public override IList<ASTNode> Children => throw new NotImplementedException();
    }
}
