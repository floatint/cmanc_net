using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST
{
    class ASTExprListNode : ASTNode
    {
        public IList<ASTExpressionNode> Expressions { get; private set; }

        public ASTExprListNode(CmanParser.ExprListContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
        }

        public void AddExpression(ASTExpressionNode e)
        {
            if (Expressions == null)
                Expressions = new List<ASTExpressionNode>();
            Expressions.Add(e);
        }

        public override IList<ASTNode> Children => Expressions.ToList<ASTNode>();
    }
}
