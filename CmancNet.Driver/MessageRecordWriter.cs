using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmancNet.Compiler;
using CmancNet.Compiler.Utils.Logging;

namespace CmancNet.Driver
{
    class MessageRecordWriter
    {
        public static void WriteLine(MessageRecord mr)
        {
            //source file
            Console.Write(mr.SourceFile);
            //position defined
            if ((mr.Line != null) && (mr.Pos != null))
            {
                Console.Write(string.Format("({0},{1}): ", mr.Line, mr.Pos));
            }
            else
            {
                Console.Write(": ");
            }
            ConsoleColor textColor = Console.ForegroundColor;
            //message type
            if (mr.Message.Type == MsgType.Warning)
                Console.ForegroundColor = ConsoleColor.Yellow;
            if (mr.Message.Type == MsgType.Error)
                Console.ForegroundColor = ConsoleColor.Red;
            if (mr.Message.Type == MsgType.Info)
                Console.ForegroundColor = ConsoleColor.Cyan;

            Console.Write(Convert.ToString(mr.Message.Type).ToLower());
            Console.Write(' ');
            //message code
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(Convert.ToString(mr.Message.Type)[0]);
            Console.Write(Convert.ToInt32(mr.Code));
            Console.Write(": ");
            //message
            Console.Write(mr.Message.Format(mr.Data));
            Console.WriteLine();
            Console.ForegroundColor = textColor;
        }
    }
}
