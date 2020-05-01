using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime;
using CmancNet.Compiler.ASTParser.AST;
using CmancNet.Compiler.ASTParser.AST.Statements;
using CmancNet.Compiler.ASTParser.AST.Expressions;
using CmancNet.Compiler.ASTParser.AST.Expressions.Binary;
using CmancNet.Compiler.ASTParser.AST.Expressions.Unary;
using CmancNet.Compiler.Utils.Logging;

namespace CmancNet.Compiler.ASTParser
{
    class ASTBuilderListener : CmanParserBaseListener
    {
        //nodes stack
        private Stack<ASTNode> _nodes;

        public ASTCompileUnitNode CompilationUnit { private set; get; }

        public bool Error => Messages.Any(x => x.Message.Type == MsgType.Error);
        public IList<MessageRecord> Messages { private set; get; }

        public ASTBuilderListener()
        {
            _nodes = new Stack<ASTNode>();
            Messages = new List<MessageRecord>();
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
                procNode.ArgList = argListNode;
            }
            else
            {
                procNode = (ASTSubStatementNode)_nodes.Pop();
            }
            if (bodyNode != null)
                procNode.Body = bodyNode;

            var compileUnitNode = (ASTCompileUnitNode)_nodes.Peek();
            compileUnitNode.AddProcedure(procNode);
        }

        public override void EnterReturnStatement([NotNull] CmanParser.ReturnStatementContext context)
        {
            _nodes.Push(new ASTReturnStatementNode(context, _nodes.Peek()));
        }

        public override void ExitReturnStatement([NotNull] CmanParser.ReturnStatementContext context)
        {
            //TODO: return node пушить в любом случае. дальше обработает семантический чекер
            if (_nodes.Peek() is IASTExprNode)
            {
                IASTExprNode expr = (IASTExprNode)_nodes.Pop();
                ((ASTReturnStatementNode)_nodes.Peek()).Expression = expr;
            }
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

        public override void ExitNull([NotNull] CmanParser.NullContext context)
        {
            _nodes.Push(new ASTNullLiteralNode(context, _nodes.Peek()));
        }

        public override void ExitTrue([NotNull] CmanParser.TrueContext context)
        {
            _nodes.Push(new ASTBoolLiteralNode(context, _nodes.Peek()));
        }

        
        public override void ExitFalse([NotNull] CmanParser.FalseContext context)
        {
            _nodes.Push(new ASTBoolLiteralNode(context, _nodes.Peek()));
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
            var op = context.GetChild(0);
            if (op.GetText() == "-")
                _nodes.Push(new ASTMinusOpNode(context, _nodes.Peek()));
            if (op.GetText() == "!")
                _nodes.Push(new ASTNotOpNode(context, _nodes.Peek()));
        }

        ////Pop operand of unar operation
        //public override void ExitUnarOp([NotNull] CmanParser.UnarOpContext context)
        //{
        //    var expr = (IASTExprNode)_nodes.Pop();
        //    var opNode = (IASTUnarOpNode)_nodes.Peek();
        //    opNode.Expression = expr;
        //    /*if (opNode is ASTMinusOpNode minusNode)
        //        minusNode.Expression = expr;
        //    if (opNode is ASTUnarNotOpNode notNode)
        //        notNode.Expression = expr;*/
        //}

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

        public override void EnterMulOrDivOp([NotNull] CmanParser.MulOrDivOpContext context)
        {
            if (context.GetText() == "/")
                _nodes.Push(new ASTDivOpNode(context, _nodes.Peek()));
            if (context.GetText() == "*")
                _nodes.Push(new ASTMulOpNode(context, _nodes.Peek()));
        }

        public override void EnterCompOp([NotNull] CmanParser.CompOpContext context)
        {
            if (context.GetText() == "<")
                _nodes.Push(new ASTLessOpNode(context, _nodes.Peek()));
            if (context.GetText() == ">")
                _nodes.Push(new ASTGreaterOpNode(context, _nodes.Peek()));
            if (context.GetText() == "==")
                _nodes.Push(new ASTEqualOpNode(context, _nodes.Peek()));

        }

        //Bin operators handling
        public override void ExitExpr([NotNull] CmanParser.ExprContext context)
        {
            var t = context.GetText();
            //var op = _nodes.LastOrDefault(x => x is ASTAddOpNode || x is ASTSubOpNode);
            var op = _nodes.ElementAt(1); //get op node
            if (op is IASTBinOpNode binOpNode)
            {
                if (binOpNode.Left == null && binOpNode.Right == null)
                {
                    var right = (IASTExprNode)_nodes.Pop();
                    _nodes.Pop();
                    var left = (IASTExprNode)_nodes.Pop();
                    binOpNode.Left = left;
                    binOpNode.Right = right;
                    _nodes.Push(op);
                }
            }
            if (op is IASTUnarOpNode unarOpNode)
            {
                unarOpNode.Expression = (IASTExprNode)_nodes.Pop();
            }
            /*if (op is ASTAddOpNode addNode)
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
            if (op is ASTLessCmpOpNode lessCmpNode)
            {
                var right = (IASTExprNode)_nodes.Pop();
                _nodes.Pop();
                var left = (IASTExprNode)_nodes.Pop();
                lessCmpNode.Left = left;
                lessCmpNode.Right = right;
                _nodes.Push(op);
            }
            if (op is ASTGreaterCmpOpNode greaterCmpNode)
            {
                var right = (IASTExprNode)_nodes.Pop();
                _nodes.Pop();
                var left = (IASTExprNode)_nodes.Pop();
                greaterCmpNode.Left = left;
                greaterCmpNode.Right = right;
                _nodes.Push(op);
            }*/

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

        public override void EnterForStatement([NotNull] CmanParser.ForStatementContext context)
        {
            _nodes.Push(new ASTForStatementNode(context, _nodes.Peek()));
            //base.EnterForStatement(context);
        }

        public override void ExitForStatement([NotNull] CmanParser.ForStatementContext context)
        {
            ASTBodyStatementNode body = null;
            IASTExprNode step = null;
            IASTExprNode cond = null;
            ASTNode counter = null;
            Queue<ASTNode> childs = new Queue<ASTNode>();
            while (!(_nodes.Peek() is ASTForStatementNode))
            {
                childs.Enqueue(_nodes.Pop());
                //childs.Push(_nodes.Pop());
            }
            
            //has body
            if (childs.Peek() is ASTBodyStatementNode)
            {
                body = (ASTBodyStatementNode)childs.Dequeue();
            }
            int childCount = childs.Count();
            //counter, condition, step
            if (childCount == 3)
            {
                step = (IASTExprNode)childs.Dequeue();
            }
            cond = (IASTExprNode)childs.Dequeue();
            counter = childs.Dequeue();

            ASTForStatementNode forNode = (ASTForStatementNode)_nodes.Peek();

            forNode.Counter = counter;
            forNode.Condition = cond;
            forNode.Step = step;
            forNode.Body = body;
        }

        public override void EnterIfStatement([NotNull] CmanParser.IfStatementContext context)
        {
            _nodes.Push(new ASTIfStatementNode(context, _nodes.Peek()));
        }

        public override void ExitIfStatement([NotNull] CmanParser.IfStatementContext context)
        {
            ASTBodyStatementNode elseBody = null;
            ASTBodyStatementNode trueBody = null;
            IASTExprNode condition = null;
            if (_nodes.Peek() is ASTBodyStatementNode)
            {
                var tmp = _nodes.Pop();
                if (_nodes.Peek() is ASTBodyStatementNode)
                {
                    elseBody = (ASTBodyStatementNode)tmp;
                    trueBody = (ASTBodyStatementNode)_nodes.Pop();
                    condition = (IASTExprNode)_nodes.Pop();
                }
                else
                {
                    trueBody = (ASTBodyStatementNode)tmp;
                    condition = (IASTExprNode)_nodes.Pop();
                }
            }
            else
            {
                condition = (IASTExprNode)_nodes.Pop();
                //((ASTIfStatementNode)_nodes.Peek()).Condition = condition;
            }
            //TODO: ifnode не привязывает к себе ноды
            var ifNode = (ASTIfStatementNode)_nodes.Peek();
            ifNode.Condition = condition;
            ifNode.TrueBody = trueBody;
            ifNode.ElseBody = elseBody;
            return;
        }
    }
}
