using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST
{
    class ASTStringLiteralNode : ASTExpressionNode
    {
        public string Value { set; get; }

        public ASTStringLiteralNode(CmanParser.StringLiteralContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
            Value = context.STRING().GetText().Trim(new char[] { '"' });
        }
    }
}
