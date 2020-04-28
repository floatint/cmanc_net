using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace CmancNet.ASTParser.AST.Expressions
{
    class ASTNumberLiteralNode : ASTNode, IASTExprNode, IASTLiteral
    {
        public object Value { set; get; }

        public ASTNumberLiteralNode(CmanParser.NumberLiteralContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
            if (context.INT() != null)
                Value = long.Parse(context.GetText());
            if (context.FLOAT() != null)
                Value = double.Parse(context.GetText(), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);
            if (context.HEX() != null)
                Value = int.Parse(context.GetText());
        }

        public override IList<ASTNode> Children => new List<ASTNode>();

        public override string ToString()
        {
            return string.Format("{0}: [{1}]", base.ToString(), Value);
        }
    }
}
