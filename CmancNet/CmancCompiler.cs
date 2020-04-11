using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using Antlr4.Runtime.Tree;

namespace CmancNet
{
    public class CmancCompiler : ICompiler
    {

        public AssemblyBuilder Compile(string sourcePath)
        {
            AntlrInputStream source = null;
            try
            {
                source = Utils.SourceProvider.FromFile(sourcePath);
            }
            catch (Exception ex)
            {
                Errors.Add("Error while read source file." + ex.Message);
                return null;
            }

            ITokenSource tokenSource = new CmanLexer(source);
            ITokenStream tokenStream = new CommonTokenStream(tokenSource);
            CmanParser parser = new CmanParser(tokenStream);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(new Utils.ANTLRErrorListener(Errors));
            IParseTree parseTree = parser.compileUnit();
            if (parser.NumberOfSyntaxErrors > 0)
                return null;
            ParseTreeWalker walker = new ParseTreeWalker();



            return null;
        }

        public bool HasErrors()
        {
            return Errors.Count() > 0;
        }

        public IList<string> Errors { private set; get; }

        public CmancCompiler()
        {
            Errors = new List<string>();
        }

    }
}
