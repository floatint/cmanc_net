using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Tree;
using Antlr4.Runtime;

namespace CmancNet
{
    class Program
    {
        static void Main(string[] args)
        {
            var source = Utils.SourceProvider.FromFile(args[1]);
            ITokenSource tokenSource = new CmanLexer(source);
            ITokenStream tokenStream = new CommonTokenStream(tokenSource);

        }
    }
}
