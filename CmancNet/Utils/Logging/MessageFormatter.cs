using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmancNet.ASTParser.AST;

namespace CmancNet.Utils.Logging
{
    class MessageFormatter
    {
        /// <summary>
        /// Message formatter for messages with code location
        /// </summary>
        /// <returns></returns>
        public static string Format(ASTNode node, MsgCode code, params object[] args)
        {
            string msgPattern = "{0}({1},{2}): {3} {4}{5}: {6}";
            Message msg = MessageTable.Get()[code];
            return string.Format(
                msgPattern,
                node.SourcePath, //source code file
                node.StartLine, //message line
                node.StartPos, //message char pos in line
                Convert.ToString(msg.Type).ToLower(), //message type lower case represent
                Convert.ToString(msg.Type)[0], //first letter for msg code
                Convert.ToInt32(code), //message code
                msg.Format(args)
                );
        }
    }
}
