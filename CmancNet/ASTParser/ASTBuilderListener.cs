using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime;
using CmancNet.ASTParser.AST;
using CmancNet.ASTParser.AST.Statements;
using CmancNet.ASTParser.AST.Expressions;
using CmancNet.ASTParser.AST.Expressions.Binary;
using CmancNet.ASTParser.AST.Expressions.Unary;

namespace CmancNet.ASTParser
{
    class ASTBuilderListener : CmanParserBaseListener
    {
        //nodes stack
        private Stack<ASTNode> _nodes; 

        public ASTCompileUnitNode CompilationUnit { private set; get; }

        public ASTBuilderListener()
        {
            _nodes = new Stack<ASTNode>();
        }

        public override void EnterCompileUnit([NotNull] CmanParser.CompileUnitContext context)
        {
            _nodes.Push(new ASTCompileUnitNode(context));
        }

        public override void ExitCompileUnit([NotNull] CmanParser.CompileUnitContext context)
        {
            CompilationUnit = (ASTCompileUnitNode)_nodes.Pop();
        }

        public override void EnterSubStatement([NotNull] CmanParser.SubStatementContext context)
        {
            _nodes.Push(new ASTSubStatementNode(context, (ASTCompileUnitNode)_nodes.Peek()));
        }

        public override void ExitSubStatement([NotNull] CmanParser.SubStatementContext context)
        {
            ASTSubStatementNode procNode;
            ASTBodyStatementNode bodyNode = null;
            if (_nodes.Peek() is ASTBodyStatementNode)
                bodyNode = (ASTBodyStatementNode)_nodes.Pop();
            if (_nodes.Peek() is ASTArgListNode argListNode)
            {
                _nodes.Pop();
                procNode = (ASTSubStatementNode)_nodes.Pop();
                procNode.Arguments = argListNode;
            }
            else
            {
                procNode = (ASTSubStatementNode)_nodes.Pop();
            }
            procNode.Body = bodyNode;

            var compileUnitNode = (ASTCompileUnitNode)_nodes.Peek();
            compileUnitNode.AddProcedure(procNode);
        }

        public override void EnterArgListDecl([NotNull] CmanParser.ArgListDeclContext context)
        {
            _nodes.Push(new ASTArgListNode(context, _nodes.Peek()));
        }

        public override void ExitArgListDecl([NotNull] CmanParser.ArgListDeclContext context)
        {
            var args = new List<ASTNode>();
            while (!(_nodes.Peek() is ASTArgListNode))
                args.Add(_nodes.Pop());
            var argListNode = (ASTArgListNode)_nodes.Peek();
            foreach (var a in args)
                argListNode.AddArgument((ASTVariableNode)a);
        }

        public override void EnterBodyStatement([NotNull] CmanParser.BodyStatementContext context)
        {
            _nodes.Push(new ASTBodyStatementNode(context, _nodes.Peek()));
        }

        public override void ExitBodyStatement([NotNull] CmanParser.BodyStatementContext context)
        {
            //TODO: пересмотреть
            var statements = new List<IASTStatementNode>();
            while (_nodes.Peek() is IASTStatementNode && !(_nodes.Peek() is ASTBodyStatementNode))
                statements.Add((IASTStatementNode)_nodes.Pop());
            
            var bodyNode = (ASTBodyStatementNode)_nodes.Peek();
            foreach (var s in statements)
            {
                bodyNode.AddStatement(s);
            }
        }

        //Push variable node to stack
        public override void ExitVar([NotNull] CmanParser.VarContext context)
        {
            _nodes.Push(new ASTVariableNode(context, _nodes.Peek()));
        }

        //Push number literal to stack
        public override void ExitNumberLiteral([NotNull] CmanParser.NumberLiteralContext context)
        {
            _nodes.Push(new ASTNumberLiteralNode(context, _nodes.Peek()));
        }

        //Push string literal to stack
        public override void ExitStringLiteral([NotNull] CmanParser.StringLiteralContext context)
        {
            _nodes.Push(new ASTStringLiteralNode(context, _nodes.Peek()));
        }

        //Push call statement to stack
        public override void EnterSubCallStatement([NotNull] CmanParser.SubCallStatementContext context)
        {
            _nodes.Push(new ASTCallStatementNode(context, _nodes.Peek()));
        }

        //Pop call arguments
        public override void ExitSubCallStatement([NotNull] CmanParser.SubCallStatementContext context)
        {
            if (_nodes.Peek() is ASTExprListNode exprListNode)
            {
                _nodes.Pop();
                ASTCallStatementNode callStmtNode = (ASTCallStatementNode)_nodes.Peek();
                callStmtNode.Arguments = exprListNode;
            }
        }

        public override void EnterExprList([NotNull] CmanParser.ExprListContext context)
        {
            _nodes.Push(new ASTExprListNode(context, _nodes.Peek()));
        }

        public override void ExitExprList([NotNull] CmanParser.ExprListContext context)
        {
            var exprList = new List<IASTExprNode>();
            while (!(_nodes.Peek() is ASTExprListNode))
            {
                exprList.Add((IASTExprNode)_nodes.Pop());
            }
            ASTExprListNode exprListNode = (ASTExprListNode)_nodes.Peek();
            foreach (var e in exprList)
            {
                exprListNode.AddExpression(e);
            }

            
        }

        //Push assign statement to stack
        public override void EnterAssignStatement([NotNull] CmanParser.AssignStatementContext context)
        {
            _nodes.Push(new ASTAssignStatementNode(context, _nodes.Peek()));
        }

        //Pop left and right side of assignment statement
        public override void ExitAssignStatement([NotNull] CmanParser.AssignStatementContext context)
        {
            var right = (IASTExprNode)_nodes.Pop();
            var left = (IASTExprNode)_nodes.Pop();
            var assignStmt = (ASTAssignStatementNode)_nodes.Peek();
            assignStmt.Right = right;
            assignStmt.Left = left;
        }

        //Push unar operation to stack
        public override void EnterUnarOp([NotNull] CmanParser.UnarOpContext context)
        {
            var child = context.GetChild(0);
            if (child.GetText() == "-")
                _nodes.Push(new ASTMinusOpNode((CmanParser.UnarOpContext)child, _nodes.Peek()));
            if (child.GetText() == "!")
                _nodes.Push(new ASTNotOpNode((CmanParser.UnarOpContext)child, _nodes.Peek()));
        }

        //Pop operand of unar operation
        public override void ExitUnarOp([NotNull] CmanParser.UnarOpContext context)
        {
            var expr = (IASTExprNode)_nodes.Pop();
            var opNode = (IASTUnarOpNode)_nodes.Peek();
            opNode.Expression = expr;
            /*if (opNode is ASTMinusOpNode minusNode)
                minusNode.Expression = expr;
            if (opNode is ASTUnarNotOpNode notNode)
                notNode.Expression = expr;*/
        }

        //Push index op to stack
        public override void EnterIndexOp([NotNull] CmanParser.IndexOpContext context)
        {
            var indexOpNode = new ASTIndexOpNode(context, _nodes.Peek());
            /*Возможно тут у expression нужно менять родителя
             Можно это переложить на свойства
             */
            indexOpNode.Expression = (IASTExprNode)_nodes.Pop();
            _nodes.Push(indexOpNode);
        }

        //Pop index operator operand
        public override void ExitIndexOp([NotNull] CmanParser.IndexOpContext context)
        {
            var index = (IASTExprNode)_nodes.Pop();
            ((ASTIndexOpNode)_nodes.Peek()).Index = index;
        }

        //Push add or sub op into stack
        public override void EnterAddOrSubOp([NotNull] CmanParser.AddOrSubOpContext context)
        {
            if (context.GetText() == "-")
                _nodes.Push(new ASTSubOpNode(context, _nodes.Peek()));
            if (context.GetText() == "+")
                _nodes.Push(new ASTAddOpNode(context, _nodes.Peek()));
        }

        public override void ExitExpr([NotNull] CmanParser.ExprContext context)
        {
            var t = context.GetText();
            //var op = _nodes.LastOrDefault(x => x is ASTAddOpNode || x is ASTSubOpNode);
            var op = _nodes.ElementAt(1); //get op node
            if (op is ASTAddOpNode addNode)
            {
                var right = (IASTExprNode)_nodes.Pop();
                _nodes.Pop();
                var left = (IASTExprNode)_nodes.Pop();
                addNode.Left = left;
                addNode.Right = right;
                _nodes.Push(op);
            }
            if (op is ASTSubOpNode subNode)
            {
                var right = (IASTExprNode)_nodes.Pop();
                _nodes.Pop();
                var left = (IASTExprNode)_nodes.Pop();
                subNode.Left = left;
                subNode.Right = right;
                _nodes.Push(op);
            }
        }

        //
        public override void EnterWhileStatement([NotNull] CmanParser.WhileStatementContext context)
        {
            _nodes.Push(new ASTWhileStatementNode(context, _nodes.Peek()));
        }

        public override void ExitWhileStatement([NotNull] CmanParser.WhileStatementContext context)
        {
            ASTWhileStatementNode whileStmtNode;
            ASTBodyStatementNode bodyNode = null;
            if (_nodes.Peek() is ASTBodyStatementNode)
                bodyNode = (ASTBodyStatementNode)_nodes.Pop();
            IASTExprNode condNode = (IASTExprNode)_nodes.Pop();
            whileStmtNode = (ASTWhileStatementNode)_nodes.Peek();
            whileStmtNode.Body = bodyNode;
            whileStmtNode.Condition = condNode;
        }
    }
}
