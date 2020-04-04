using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime;
using CmancNet.ASTParser.AST;

namespace CmancNet.ASTParser
{
    class ASTBuilderListener : CmanParserBaseListener
    {
        //current procudure
        private ASTProcNode _currentProc;
        //code blocks stack
        private Stack<ASTBodyNode> _codeBlocks;
        //nodes stack
        private Stack<ASTNode> _nodes; 


        //список выражений текущего оператора
        private IList<Tuple<ParserRuleContext, ASTExpressionNode>> _expressionList;
        private Tuple<ParserRuleContext, ASTExpressionNode> _currentExpression;

        //текущий оператор
        private ASTStatementNode _currentStatement;
        

        //private List<ASTExpressionNode> _expresions;

        public ASTCompilationUnitNode CompilationUnit { set; get; }

        public ASTBuilderListener()
        {
            _codeBlocks = new Stack<ASTBodyNode>();
            _nodes = new Stack<ASTNode>();
            _expressionList = new List<Tuple<ParserRuleContext, ASTExpressionNode>>();
        }

        public override void EnterCompileUnit([NotNull] CmanParser.CompileUnitContext context)
        {
            _nodes.Push(new ASTCompilationUnitNode(context));
            //CompilationUnit = new ASTCompilationUnitNode(context);
        }

        public override void ExitCompileUnit([NotNull] CmanParser.CompileUnitContext context)
        {
            CompilationUnit = (ASTCompilationUnitNode)_nodes.Pop();
        }

        public override void EnterProcStatement([NotNull] CmanParser.ProcStatementContext context)
        {
            _nodes.Push(new ASTProcNode(context, (ASTCompilationUnitNode)_nodes.Peek()));
            //_currentProc = new ASTProcNode(context, CompilationUnit);
        }

        public override void ExitProcStatement([NotNull] CmanParser.ProcStatementContext context)
        {
            ASTProcNode procNode;
            ASTBodyNode bodyNode = null;
            //TODO: учесть что тела может и не быть
            if (_nodes.Peek() is ASTBodyNode)
                bodyNode = (ASTBodyNode)_nodes.Pop();
            //ASTBodyNode body = (ASTBodyNode)_nodes.Pop();
            if (_nodes.Peek() is ASTArgListNode argListNode)
            {
                _nodes.Pop();
                procNode = (ASTProcNode)_nodes.Pop();
                procNode.Arguments = argListNode;
            }
            else
            {
                procNode = (ASTProcNode)_nodes.Pop();
            }
            procNode.Body = bodyNode;

            var compileUnitNode = (ASTCompilationUnitNode)_nodes.Peek();
            //if (compileUnitNode.Procedures == null)
            //    compileUnitNode.Procedures = new List<ASTProcNode>();
            //compileUnitNode.Procedures.Add((ASTProcNode)procNode);
            compileUnitNode.AddProcedure(procNode);
            //CompilationUnit.Procedures.Add(_currentProc);
            //_currentProc = null;
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
            _nodes.Push(new ASTBodyNode(context, _nodes.Peek()));
            //if no has proc dody, loops
            /*if (_codeBlocks.Count == 0)
            {
                _codeBlocks.Push(new ASTBodyNode(context, _currentProc));
            }
            else
            {
                _codeBlocks.Push(new ASTBodyNode(context, _codeBlocks.Peek()));
            }*/
        }

        public override void ExitBodyStatement([NotNull] CmanParser.BodyStatementContext context)
        {
            var statements = new List<ASTStatementNode>();
            while (_nodes.Peek() is ASTStatementNode)
                statements.Add((ASTStatementNode)_nodes.Pop());
            
            var bodyNode = (ASTBodyNode)_nodes.Peek();
            bodyNode.Statements = statements;

            /*var statement = _nodes.Peek();

            if (statement is ASTProcNode procStmt)
                procStmt.Body = bodyNode;
            if (statement is ASTWhileStatementNode whileStmt)
                whileStmt.Body = bodyNode;*/
            /*if (_codeBlocks.Count == 1)
            {
                _currentProc.Body = _codeBlocks.Peek();
            }
            _codeBlocks.Pop();*/
        }

        /*
         * Высокоуровневый обработчик.
         * 
         *
         */
        //public override void EnterExpr([NotNull] CmanParser.ExprContext context)
        //{
        //    /*
        //      ПУШИТЬ ТОЛЬКО ЛИТЕРАЛЫ,ПЕРЕМЕННЫЕ
        //     */
        //    //literals, variable, call
            
        //    if (context.ChildCount == 1)
        //    {
        //        Console.WriteLine("Push: " + context.GetText());
        //        var child = context.GetChild(0);
        //        if (child is CmanParser.VarOrExprContext varOrExpr)
        //            _nodes.Push(new ASTVariableNode((CmanParser.VarContext)varOrExpr.GetChild(0), _nodes.Peek()));
        //        if (child is CmanParser.NumberLiteralContext numContext)
        //            _nodes.Push(new ASTNumberLiteralNode(numContext, _nodes.Peek()));
        //        if (child is CmanParser.StringLiteralContext strContext)
        //            _nodes.Push(new ASTStringLiteralNode(strContext, _nodes.Peek()));
        //        if (child is CmanParser.ProcCallStatementContext callContext)
        //            _nodes.Push(new ASTCallStatementNode(callContext, _nodes.Peek()));
        //    }
        //    if (context.ChildCount == 2)
        //    {
        //        var exprContext = (CmanParser.ExprContext)context.children.First(x => x is CmanParser.ExprContext);

        //        var op = context.GetChild(0);
        //        if (op is CmanParser.UnarOpContext unOpContext)
        //        {
        //            Console.WriteLine("Push: " + op.GetText());
        //            if (unOpContext.GetText() == "-")
        //                _nodes.Push(new ASTUnarMinusOpNode(unOpContext, _nodes.Peek()));
        //            if (unOpContext.GetText() == "!")
        //                _nodes.Push(new ASTUnarNotOpNode(unOpContext, _nodes.Peek()));
        //        }
        //        else //index operator
        //        {
        //            Console.WriteLine("Push: " + context.GetChild(1).GetText());
        //            _nodes.Push(new ASTIndexOpNode((CmanParser.IndexOpContext)context.GetChild(1), _nodes.Peek()));
        //        }
        //    }
        //    if (context.ChildCount == 3)
        //    {
        //        var opContext = context.GetChild(1);
        //        if (opContext is CmanParser.AddOrSubOpContext addOrSubContext)
        //        {
        //            Console.WriteLine("Push: " + addOrSubContext.GetText());
        //            if (opContext.GetText() == "-")
        //                _nodes.Push(new ASTSubOpNode(addOrSubContext, _nodes.Peek()));
        //            if (opContext.GetText() == "+")
        //                _nodes.Push(new ASTAddOpNode(addOrSubContext, _nodes.Peek()));
        //        }
        //    }
        //    /*Console.WriteLine("Enter " + context.GetText());
        //    Console.WriteLine("===============================");
        //    if (_expressionList.Count == 0)
        //    {
        //        _expressionList.Add(new Tuple<ParserRuleContext, ASTExpressionNode>(
        //            context,
        //            ASTExpressionNode.BuildExpressionSubTree(context, _currentStatement)));
        //    }
        //    else
        //    {
        //        var tmp = _expressionList.Where(x => x.Item1.Parent == context.Parent).ToList();
        //        if (tmp.Count != 0)
        //        {
        //            _expressionList.Add(new Tuple<ParserRuleContext, ASTExpressionNode>(
        //                context,
        //                ASTExpressionNode.BuildExpressionSubTree(context, _currentStatement)));
        //        }
        //    }*/
        //}

        /*public override void ExitExpr([NotNull] CmanParser.ExprContext context)
        {

            if (context.ChildCount == 1)
            {
                Console.WriteLine("Pop: " + context.GetText());
                var child = context.GetChild(0);
                if (child is CmanParser.VarOrExprContext varOrExpr)
                    _nodes.Push(new ASTVariableNode((CmanParser.VarContext)varOrExpr.GetChild(0), _nodes.Peek()));
                if (child is CmanParser.NumberLiteralContext numContext)
                    _nodes.Push(new ASTNumberLiteralNode(numContext, _nodes.Peek()));
                if (child is CmanParser.StringLiteralContext strContext)
                    _nodes.Push(new ASTStringLiteralNode(strContext, _nodes.Peek()));
                if (child is CmanParser.ProcCallStatementContext callContext)
                    _nodes.Push(new ASTCallStatementNode(callContext, _nodes.Peek()));
            }
            if (context.ChildCount == 2)
            {
                var exprContext = (CmanParser.ExprContext)context.children.First(x => x is CmanParser.ExprContext);

                var op = context.GetChild(0);
                if (op is CmanParser.UnarOpContext unOpContext)
                {
                    Console.WriteLine("Pop: " + op.GetText());
                    if (unOpContext.GetText() == "-")
                        _nodes.Push(new ASTUnarMinusOpNode(unOpContext, _nodes.Peek()));
                    if (unOpContext.GetText() == "!")
                        _nodes.Push(new ASTUnarNotOpNode(unOpContext, _nodes.Peek()));
                }
                else //index operator
                {
                    Console.WriteLine("Pop: " + context.GetChild(1).GetText());
                    
                }
            }
            if (context.ChildCount == 3)
            {
                var opContext = context.GetChild(1);
                if (opContext is CmanParser.AddOrSubOpContext addOrSubContext)
                {
                    Console.WriteLine("Pop: " + addOrSubContext.GetText());
                }
            }
            var t = context.GetText();
            
            Console.WriteLine("Exit " + context.GetText());
            Console.WriteLine("===============================");
        }*/

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
        public override void EnterProcCallStatement([NotNull] CmanParser.ProcCallStatementContext context)
        {
            _nodes.Push(new ASTCallStatementNode(context, _nodes.Peek()));
        }

        //Pop call arguments
        public override void ExitProcCallStatement([NotNull] CmanParser.ProcCallStatementContext context)
        {
            List<ASTExpressionNode> args = new List<ASTExpressionNode>();
            while (!(_nodes.Peek() is ASTCallStatementNode))
            {
                args.Add((ASTExpressionNode)_nodes.Pop());
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
            var right = (ASTExpressionNode)_nodes.Pop();
            var left = (ASTExpressionNode)_nodes.Pop();
            var assignStmt = (ASTAssignStatementNode)_nodes.Peek();
            assignStmt.Right = right;
            assignStmt.Left = left;
        }

        //Push unar operation to stack
        public override void EnterUnarOp([NotNull] CmanParser.UnarOpContext context)
        {
            var child = context.GetChild(0);
            if (child.GetText() == "-")
                _nodes.Push(new ASTUnarMinusOpNode((CmanParser.UnarOpContext)child, _nodes.Peek()));
            if (child.GetText() == "!")
                _nodes.Push(new ASTUnarNotOpNode((CmanParser.UnarOpContext)child, _nodes.Peek()));
        }

        //Pop operand of unar operation
        public override void ExitUnarOp([NotNull] CmanParser.UnarOpContext context)
        {
            var expr = (ASTExpressionNode)_nodes.Pop();
            var opNode = _nodes.Peek();
            if (opNode is ASTUnarMinusOpNode minusNode)
                minusNode.Expression = expr;
            if (opNode is ASTUnarNotOpNode notNode)
                notNode.Expression = expr;
        }

        //Push index op to stack
        public override void EnterIndexOp([NotNull] CmanParser.IndexOpContext context)
        {
            var indexOpNode = new ASTIndexOpNode(context, _nodes.Peek());
            /*Возможно тут у expression нужно менять родителя
             Можно это переложить на свойства
             */
            indexOpNode.Expression = (ASTExpressionNode)_nodes.Pop();
            _nodes.Push(indexOpNode);
        }

        //Pop index operator operand
        public override void ExitIndexOp([NotNull] CmanParser.IndexOpContext context)
        {
            var index = (ASTExpressionNode)_nodes.Pop();
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

        /*public override void ExitAddOrSubOp([NotNull] CmanParser.AddOrSubOpContext context)
        {
            var right = (ASTExpressionNode)_nodes.Pop();
            var op = _nodes.Pop();
            var left = (ASTExpressionNode)_nodes.Pop();
            if (op is ASTAddOpNode addNode)
            {
                addNode.Left = left;
                addNode.Right = right;
            }
            if (op is ASTSubOpNode subNode)
            {
                subNode.Left = left;
                subNode.Right = right;
            }
            _nodes.Push(op);
            int i = 0;
        }*/


        public override void ExitExpr([NotNull] CmanParser.ExprContext context)
        {
            var t = context.GetText();
            //var op = _nodes.LastOrDefault(x => x is ASTAddOpNode || x is ASTSubOpNode);
            var op = _nodes.ElementAt(1); //get op node
            if (op is ASTAddOpNode addNode)
            {
                var right = (ASTExpressionNode)_nodes.Pop();
                _nodes.Pop();
                var left = (ASTExpressionNode)_nodes.Pop();
                addNode.Left = left;
                addNode.Right = right;
                _nodes.Push(op);
            }
            if (op is ASTSubOpNode subNode)
            {
                var right = (ASTExpressionNode)_nodes.Pop();
                _nodes.Pop();
                var left = (ASTExpressionNode)_nodes.Pop();
                subNode.Left = left;
                subNode.Right = right;
                _nodes.Push(op);
            }
        }



        /*public override void EnterAddOrSubOp([NotNull] CmanParser.AddOrSubOpContext context)
        {
            Console.WriteLine("Enter op" + context.Parent.GetText());
        }

        public override void ExitAddOrSubOp([NotNull] CmanParser.AddOrSubOpContext context)
        {
            Console.WriteLine("Exit op" + context.Parent.GetText());
        }*/

        /*public override void EnterAssignStatement([NotNull] CmanParser.AssignStatementContext context)
        {
            _nodes.Push(new ASTAssignStatementNode(context, _nodes.Peek()));
        }

        public override void ExitAssignStatement([NotNull] CmanParser.AssignStatementContext context)
        {
            //var right = (ASTExpressionNode)_nodes.Pop();
            //var left = (ASTExpressionNode)_nodes.Pop();

        }*/


        /*public override void EnterAssignStatement([NotNull] CmanParser.AssignStatementContext context)
        {
            _currentStatement = new ASTAssignStatementNode(context, _codeBlocks.Peek());
        }

        public override void ExitAssignStatement([NotNull] CmanParser.AssignStatementContext context)
        {
            var assignSttement = (_currentStatement as ASTAssignStatementNode);
            assignSttement.Left = _expressionList.ElementAt(0).Item2;
            assignSttement.Right = _expressionList.ElementAt(1).Item2;
            _expressionList.Clear();
            PushCurrentStatementToBlock();
        }*/


        /*public override void EnterProcCallStatement([NotNull] CmanParser.ProcCallStatementContext context)
        {
            if (_currentStatement == null)
                _currentStatement = new ASTCallStatementNode(context, _codeBlocks.Peek());
        }

        public override void ExitProcCallStatement([NotNull] CmanParser.ProcCallStatementContext context)
        {
            if (_currentStatement is ASTCallStatementNode callNode)
            {
                if (_expressionList.Count != 0)
                {
                    callNode.Arguments = new List<ASTExpressionNode>();
                    foreach (var e in _expressionList)
                    {
                        callNode.Arguments.Add(e.Item2);
                        Console.WriteLine(((ASTStringLiteralNode)callNode.Arguments.ElementAt(0)).Value);
                    }
                }
                _expressionList.Clear();
                PushCurrentStatementToBlock();
            }
        }*/

        public override void EnterWhileStatement([NotNull] CmanParser.WhileStatementContext context)
        {
            _currentStatement = new ASTWhileStatementNode(context, _codeBlocks.Peek());
        }

        public override void ExitWhileStatement([NotNull] CmanParser.WhileStatementContext context)
        {
            _expressionList.Clear();
            PushCurrentStatementToBlock();
        }


        //Internal
        private void PushCurrentStatementToBlock()
        {
            var body = _codeBlocks.Peek();
            if (body.Statements == null)
                body.Statements = new List<ASTStatementNode>();
            body.Statements.Add(_currentStatement);
            _currentStatement = null;
        }

    }
}
