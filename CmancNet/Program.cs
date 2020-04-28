using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using Antlr4.Runtime.Tree;
using Antlr4.Runtime;
using CmancNet.ASTParser;
using CmancNet.ASTProcessors;
using CmancNet.Codegen;
using CmancNet.Utils.Logging;

namespace CmancNet
{
    class Program
    {

        static void Main(string[] args)
        {
            //logged messages
            IEnumerable<MessageRecord> messages = new List<MessageRecord>();
            //Build assembly
            AssemblyBuilder builtAssembly = null;
            //get source stream
            var source = Utils.SourceProvider.FromFile(args[0]);
            //get token source
            ITokenSource tokenSource = new CmanLexer(source);
            //get token stream
            ITokenStream tokenStream = new CommonTokenStream(tokenSource);
            //get parser
            CmanParser parser = new CmanParser(tokenStream);
            //parser.RemoveErrorListeners();
            //parser.AddErrorListener(new Utils.ANTLRErrorListener());
            //parse
            IParseTree parseTree = parser.compileUnit();
            ParseTreeWalker walker = new ParseTreeWalker();
            //ast builder
            var astBuilder = new ASTBuilderListener();
            //build ast tree
            walker.Walk(astBuilder, parseTree);
            var ast = astBuilder.CompilationUnit;
            if (astBuilder.ErrorsCount != 0)
            {
                foreach (var e in astBuilder.Errors)
                {
                    Console.WriteLine(e);
                }
                return;
            }
            var symbolTableBuilder = new ASTSymbolTableBuilder(ast);
            if (!symbolTableBuilder.Build())
            {
                messages = messages.Concat(symbolTableBuilder.Messages);
                //foreach (var m in symbolTableBuilder.Messages)
                //    Console.WriteLine(m.ToString());
                //hasError = true;
            }
            else //next step
            {
                var symbolTable = symbolTableBuilder.Symbols;
                ASTSemanticChecker semanticChecker = new ASTSemanticChecker(ast, symbolTable);
                //sematic check fail
                if (!semanticChecker.IsValid())
                {
                    messages = messages.Concat(semanticChecker.Messages);
                }
                else //compile
                {
                    CodeBuilder codeBuilder = new CodeBuilder(ast, symbolTable);
                    //holder.SetEntryPoint("main");
                    AssemblyBuilder assemblyBuilder = codeBuilder.Build();
                    assemblyBuilder.SetEntryPoint(assemblyBuilder.GetType("Program").GetMethod("main"));
                    assemblyBuilder.Save(ast.Name + ".exe");
                    //add successful message
                }
            }
            //log messages
            foreach (var m in messages)
            {
                Console.WriteLine(m.ToString());
            }
            return;
        }
    }
}
