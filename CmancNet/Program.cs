using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Tree;
using Antlr4.Runtime;
using CmancNet.ASTParser;

namespace CmancNet
{
    class Program
    {
        static void Main(string[] args)
        {
            var source = Utils.SourceProvider.FromFile(args[0]);
            ITokenSource tokenSource = new CmanLexer(source);
            ITokenStream tokenStream = new CommonTokenStream(tokenSource);
            CmanParser parser = new CmanParser(tokenStream);
            IParseTree parseTree = parser.compileUnit();
            ParseTreeWalker walker = new ParseTreeWalker();
            //ast builder
            var astBuilder = new ASTBuilderListener();
            walker.Walk(astBuilder, parseTree);
            var ast = astBuilder.CompilationUnit;
            //Console.WriteLine(ast.ToString());
            //Console.WriteLine(parseTree.ToStringTree(parser));
            Console.WriteLine(Utils.ASTPrinter.Print(ast));
            System.IO.File.WriteAllText(string.Format("{0}_ast_listing.txt", ast.Name), Utils.ASTPrinter.Print(ast));
            return;
        }
    }
}
