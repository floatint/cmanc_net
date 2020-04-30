using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace CmancNet.Compiler.Utils
{
    class ANTLRErrorListener : BaseErrorListener
    {
        public ANTLRErrorListener(IList<string> errorList)
        {
            _errorList = errorList;
        }

        public override void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
        {
            string message = "{0}({1},{2}): {3}";
            _errorList.Add(string.Format(
                message,
                recognizer.InputStream.SourceName,
                line,
                charPositionInLine,
                msg
                ));
        }

        private IList<string> _errorList;

    }
}
