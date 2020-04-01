using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST
{
    class ASTNumberLiteralNode : ASTExpressionNode
    {
        public decimal Value { set; get; }

        public ASTNumberLiteralNode(CmanParser.NumberLiteralContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
            Value = decimal.Parse(context.GetText());
        }
    }
}
