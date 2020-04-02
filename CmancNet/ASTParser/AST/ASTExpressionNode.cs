using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST
{
    class ASTExpressionNode : ASTNode
    {
        /*
         * Низкоуровневый парсер ASTExpression
         * 
         * Просто смотрит сколько детей, что за оператор и строит рекурсивно поддерево.
         */
        public static ASTExpressionNode BuildExpressionSubTree(CmanParser.ExprContext context, ASTNode parent)
        {
            ASTExpressionNode node = null;
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
                    var subChild = child.GetChild(0);
                    if (subChild is CmanParser.VarContext)
                        return new ASTVariableNode((CmanParser.VarContext)subChild, parent);
                    else
                    {
                        return BuildExpressionSubTree((CmanParser.ExprContext)subChild, parent);
                    }
                }
            }
            //unar oper, index
            if (context.ChildCount == 2)
            {
                var exprContext = context.children.First(x => x is CmanParser.ExprContext);
                var opContext = context.children.First(x => x is CmanParser.UnarOpContext || x is CmanParser.IndexOpContext);
                if (opContext is CmanParser.UnarOpContext)
                {
                    if (opContext.GetText() == "-")
                    {
                        node = new ASTUnarMinusOpNode(
                            (CmanParser.UnarOpContext)opContext,
                            null,
                            parent);
                        ((ASTUnarMinusOpNode)node).Expression = BuildExpressionSubTree(
                            (CmanParser.ExprContext)exprContext,
                            node
                            );
                    }

                    if (opContext.GetText() == "!")
                    {
                        node = new ASTUnarNotOpNode(
                            (CmanParser.UnarOpContext)opContext,
                            parent
                        );
                        ((ASTUnarNotOpNode)node).Expression = BuildExpressionSubTree(
                            (CmanParser.ExprContext)exprContext,
                            node
                            );
                    }

                }
                if (opContext is CmanParser.IndexOpContext)
                {
                    node = new ASTIndexOpNode(
                        (CmanParser.ExprContext)exprContext,
                        null,
                        null,
                        parent);
                    ((ASTIndexOpNode)node).Expression = BuildExpressionSubTree(
                        (CmanParser.ExprContext)exprContext,
                        node
                        );
                    ((ASTIndexOpNode)node).Index = BuildExpressionSubTree(
                        (CmanParser.ExprContext)opContext.GetChild(1),
                        node
                        );
                }
                return node;
                var firstChild = context.GetChild(0);
                var secondChild = context.GetChild(1);
                if (firstChild is CmanParser.UnarOpContext unarOpContext)
                {
                    //CmanParser.UnarOpContext unarOpContext = (CmanParser.UnarOpContext)firstChild;
                    //-expr
                    if (unarOpContext.GetText() == "-")
                    {
                        node = new ASTUnarMinusOpNode(
                            (CmanParser.UnarOpContext)firstChild,
                            null,
                            parent);
                        ((ASTUnarMinusOpNode)node).Expression = BuildExpressionSubTree(
                            (CmanParser.ExprContext)secondChild,
                            node
                            );
                    }
                    //!expr
                    if (unarOpContext.GetText() == unarOpContext.NOT().GetText())
                    {
                        node = new ASTUnarNotOpNode(
                            (CmanParser.UnarOpContext)firstChild,
                            parent
                            );
                        ((ASTUnarNotOpNode)node).Expression = BuildExpressionSubTree(
                            (CmanParser.ExprContext)secondChild,
                            node
                            );
                    }
                    //return node;
                }
                else //index op
                {
                    node = new ASTIndexOpNode(
                        (CmanParser.ExprContext)firstChild,
                        null,
                        null,
                        parent);
                    ((ASTIndexOpNode)node).Expression = BuildExpressionSubTree(
                        (CmanParser.ExprContext)firstChild,
                        node
                        );
                    ((ASTIndexOpNode)node).Index = BuildExpressionSubTree(
                        (CmanParser.ExprContext)secondChild.GetChild(1),
                        node
                        );
                    //return node;
                }
                    
            }
            //binary operators
            if (context.ChildCount == 3)
            {
                
            }
            return node;
        }

        public ASTExpressionNode(ASTNode parent) : base(parent)
        {
        }
    }
}
