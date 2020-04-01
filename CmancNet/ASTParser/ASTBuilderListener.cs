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

        private Tuple<ParserRuleContext, ASTExpressionNode> _currentExpression;

        private ASTStatementNode _currentStatement;
        

        private List<ASTExpressionNode> _expresions;

        public ASTCompilationUnitNode CompilationUnit { set; get; }

        public ASTBuilderListener()
        {
            _codeBlocks = new Stack<ASTBodyNode>();
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

        public override void EnterIndexStatement([NotNull] CmanParser.IndexStatementContext context)
        {
            //Console.WriteLine(context.GetText() + ' ' + context.Parent.GetText() + ' ' + context.Parent.GetChild(0).GetText());
        }

        public override void EnterAssignStatement([NotNull] CmanParser.AssignStatementContext context)
        {
            
        }


    }
}
