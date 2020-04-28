using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmancNet.Compiler.ASTParser.AST.Expressions;

namespace CmancNet.Compiler.ASTParser.AST
{
    class ASTExprListNode : ASTNode
    {
        public IList<IASTExprNode> Expressions { get; private set; }

        public ASTExprListNode(CmanParser.ExprListContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
        }

        public void AddExpression(IASTExprNode e)
        {
            if (Expressions == null)
                Expressions = new List<IASTExprNode>();
            ((ASTNode)e).Parent = this;
            Expressions.Insert(0, e);
        }

        public override IList<ASTNode> Children => Expressions.Cast<ASTNode>().ToList();
    }
}
