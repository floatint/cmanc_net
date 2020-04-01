using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST
{
    class ASTExpressionNode : ASTNode
    {
        public static ASTExpressionNode BuildExpressionSubTree(CmanParser.ExprContext context, ASTNode parent)
        {
            //call, var, literal
            if (context.ChildCount == 1)
            {
                var child = context.GetChild(0);

                if (child is CmanParser.StringLiteralContext)
                    return new ASTStringLiteralNode((CmanParser.StringLiteralContext)child, parent);
                if (child is CmanParser.NumberLiteralContext)
                    return new ASTNumberLiteralNode((CmanParser.NumberLiteralContext)child, parent);
                if (child is CmanParser.VarOrExprContext)
                {
                    if (child is CmanParser.VarContext)
                        return new ASTVariableNode((CmanParser.VarContext)child, parent);
                    else
                        return ASTExpressionNode.BuildExpressionSubTree((CmanParser.ExprContext)child, parent);
                }
            }
            //unar oper
            if (context.ChildCount == 2)
            {
                var firstChild = context.GetChild(0);
                var secondChild = context.GetChild(1);
                if (firstChild is CmanParser.UnarOpContext)
                    return new ASTUnarMinusOpNode(
                        (CmanParser.UnarOpContext)firstChild,
                        BuildExpressionSubTree((CmanParser.ExprContext)secondChild, parent),
                        parent);
                    
            }
            return null;
        }

        public ASTExpressionNode(ASTNode parent) : base(parent)
        {
        }
    }
}
