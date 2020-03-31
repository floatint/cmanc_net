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
            var ast = astBuilder.BuiltAST;
            Console.WriteLine(parseTree.ToStringTree(parser));
            return;
        }
    }
}
