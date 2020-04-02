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
            _expressionList = new List<Tuple<ParserRuleContext, ASTExpressionNode>>();
        }

        public override void EnterCompileUnit([NotNull] CmanParser.CompileUnitContext context)
        {
            CompilationUnit = new ASTCompilationUnitNode(context);
        }

        public override void EnterProcStatement([NotNull] CmanParser.ProcStatementContext context)
        {
            _currentProc = new ASTProcNode(context, CompilationUnit);
        }

        public override void ExitProcStatement([NotNull] CmanParser.ProcStatementContext context)
        {
            CompilationUnit.Procedures.Add(_currentProc);
            _currentProc = null;
        }

        public override void EnterBodyStatement([NotNull] CmanParser.BodyStatementContext context)
        {
            //if no has proc dody, loops
            if (_codeBlocks.Count == 0)
            {
                _codeBlocks.Push(new ASTBodyNode(context, _currentProc));
            }
            else
            {
                _codeBlocks.Push(new ASTBodyNode(context, _codeBlocks.Peek()));
            }
        }

        public override void ExitBodyStatement([NotNull] CmanParser.BodyStatementContext context)
        {
            if (_codeBlocks.Count == 1)
            {
                _currentProc.Body = _codeBlocks.Peek();
            }
            _codeBlocks.Pop();
        }

        /*
         * Высокоуровневый обработчик.
         * 
         *
         */
        public override void EnterExpr([NotNull] CmanParser.ExprContext context)
        {
            if (_expressionList.Count == 0)
            {
                _expressionList.Add(new Tuple<ParserRuleContext, ASTExpressionNode>(
                    context,
                    ASTExpressionNode.BuildExpressionSubTree(context, _currentStatement)));
            }
            else
            {
                var tmp = _expressionList.Where(x => x.Item1.Parent == context.Parent).ToList();
                if (tmp.Count != 0)
                {
                    _expressionList.Add(new Tuple<ParserRuleContext, ASTExpressionNode>(
                        context,
                        ASTExpressionNode.BuildExpressionSubTree(context, _currentStatement)));
                }
            }
        }


        public override void EnterAssignStatement([NotNull] CmanParser.AssignStatementContext context)
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
        }


        public override void EnterProcCallStatement([NotNull] CmanParser.ProcCallStatementContext context)
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
        }

        public override void EnterWhileStatement([NotNull] CmanParser.WhileStatementContext context)
        {

            base.EnterWhileStatement(context);
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
