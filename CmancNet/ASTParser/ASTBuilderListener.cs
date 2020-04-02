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
            return;

            if (_currentExpression == null)
            {
                _currentExpression = new Tuple<ParserRuleContext, ASTExpressionNode>(
                    context,
                    ASTExpressionNode.BuildExpressionSubTree(context, _currentStatement));
            }
            
            var children = context.children;
            Console.WriteLine(context.GetText());
            foreach (var c in children)
            {
                Console.WriteLine(c.GetText() + ' ' + c.GetType());
            }
            Console.WriteLine("============================");
            return;
            //var, literal or func call
            if (context.ChildCount == 1)
            {
                //Console.WriteLine(context.GetChild(0).GetType());
                if (context.GetChild(0) is CmanParser.VarOrExprContext)
                {
                    if (context.GetChild(0).GetChild(0) is CmanParser.VarContext)
                        Console.WriteLine("variable");
                }
                if (context.GetChild(0) is CmanParser.NumberLiteralContext)
                    Console.WriteLine("number");
                if (context.GetChild(0) is CmanParser.StringLiteralContext)
                    Console.WriteLine("string");
            }
            //_expresions.Add(new ASTExpressionNode(context, null));
            
        }

        public override void ExitExpr([NotNull] CmanParser.ExprContext context)
        {
            //if (_currentExpression.Item1 == context)
            //    _currentExpression = null;
        }


        public override void EnterAssignStatement([NotNull] CmanParser.AssignStatementContext context)
        {

            //сделать обработку Left child
            _currentStatement = new ASTAssignStatementNode(context, _codeBlocks.Peek());

        }

        public override void ExitAssignStatement([NotNull] CmanParser.AssignStatementContext context)
        {
            var assignSttement = (_currentStatement as ASTAssignStatementNode);
            assignSttement.Left = _expressionList.ElementAt(0).Item2;
            assignSttement.Right = _expressionList.ElementAt(1).Item2;
            _expressionList.Clear();
            var body = _codeBlocks.Peek();
            if (body.Statements == null)
                body.Statements = new List<ASTStatementNode>();
            body.Statements.Add(_currentStatement);
            _currentStatement = null;
            return;
            var curStatement = (_currentStatement as ASTAssignStatementNode);
            curStatement.Right = _currentExpression.Item2;
            //var body = _codeBlocks.Peek();
            if (body.Statements == null)
                body.Statements = new List<ASTStatementNode>();
            body.Statements.Add(_currentStatement);
            _currentExpression = null;
            _currentStatement = null;
        }



    }
}
