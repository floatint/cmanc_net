using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using CmancNet.Compiler.Utils.Logging;

namespace CmancNet.Compiler.Utils.Logging
{
    class ANTLRErrorListener : BaseErrorListener
    {
        public ANTLRErrorListener()
        {
            _messages = new List<MessageRecord>();
        }

        public override void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
        {
            _messages.Add(new MessageRecord(
                MsgCode.CompilerError,
                offendingSymbol.InputStream.SourceName,
                line,
                charPositionInLine,
                msg
                ));
        }

        public IEnumerable<MessageRecord> Messages => _messages.AsEnumerable();
        public bool Error => _messages.Any(x => x.Message.Type == MsgType.Error);

        private IList<MessageRecord> _messages;

    }
}
