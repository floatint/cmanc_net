using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.Compiler.ASTParser.AST.Expressions
{
    class ASTBoolLiteralNode : ASTNode, IASTExprNode, IASTLiteral
    {
        public bool Value { private set; get; }

        public ASTBoolLiteralNode(CmanParser.TrueContext context, ASTNode parent)
            : base(parent)
        {
            Value = true;
            SetLocation(context);
        }

        public ASTBoolLiteralNode(CmanParser.FalseContext context, ASTNode parent)
            : base(parent)
        {
            Value = false;
            SetLocation(context);
        }

        public override IList<ASTNode> Children => throw new NotImplementedException();
    }
}
