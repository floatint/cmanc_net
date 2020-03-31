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
        private ASTProcNode _currentProc;
        

        private Stack<ASTBodyNode> Blocks { set; get; }
        public ASTCompilationUnitNode BuiltAST { set; get; }

        public ASTBuilderListener()
        {
            Blocks = new Stack<ASTBodyNode>();
        }

        public override void EnterCompileUnit([NotNull] CmanParser.CompileUnitContext context)
        {
            BuiltAST = new ASTCompilationUnitNode(context.Start.InputStream.SourceName);
            BuiltAST.SetLocation(context.Start.Line, context.Start.Column, context.Stop.Line, context.Stop.Column);
            var procs = context.children.Where(x => x is CmanParser.ProcStatementContext).ToList();
            if (procs.Count != 0)
            {
                foreach (var p in procs)
                {
                    EnterProcStatement((CmanParser.ProcStatementContext)p);
                }
            }
            else
            {
                //log
            }
            return;
        }

        public override void EnterProcStatement([NotNull] CmanParser.ProcStatementContext context)
        {
            Console.WriteLine("proc" + context.children.First(x => x is CmanParser.NameContext).GetText());
            ASTProcNode proc = new ASTProcNode(BuiltAST);
            proc.Name = context.children.First(x => x is CmanParser.NameContext).GetText();
            BuiltAST.Procedures.Add(proc);
            var body = (ParserRuleContext)context.children.First(x => x is CmanParser.BodyStatementContext);
            //ASTBodyNode bodyNode = new ASTBodyNode(proc);
            //CurrentBlock.Push(bodyNode);
            EnterBodyStatement((CmanParser.BodyStatementContext)body);
            //BuiltAST.Procedures.Last().Body = bodyNode;
        }

        public override void EnterBodyStatement([NotNull] CmanParser.BodyStatementContext context)
        {
            if (context.children != null)
            {
                ASTNode parent;
                //если тело принадлежит процедуре
                if (Blocks.Count == 0)
                {
                    parent = BuiltAST.Procedures.Last();
                }
                else //Если тело принадлежит циклу
                {
                    parent = Blocks.Peek();
                }
               
                var bodyNode = new ASTBodyNode(parent);
                if (parent is ASTProcNode)
                    ((ASTProcNode)parent).Body = bodyNode;

                Blocks.Push(bodyNode);

                foreach (var c in context.children)
                {
                    if (c is CmanParser.AssignStatementContext)
                        EnterAssignStatement((CmanParser.AssignStatementContext)c);
                    if (c is CmanParser.WhileStatementContext)
                        EnterWhileStatement((CmanParser.WhileStatementContext)c);
                }
            }
        }

        public override void ExitBodyStatement([NotNull] CmanParser.BodyStatementContext context)
        {
            Blocks.Pop();
        }

        public override void EnterAssignStatement([NotNull] CmanParser.AssignStatementContext context)
        {
            
        }


    }
}
