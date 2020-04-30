using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.Compiler.Utils.Logging
{
    public class MessageRecord
    {
        public MsgCode Code { private set;  get; }
        public string SourceFile { private set; get; }
        public int? Line { private set; get; }
        public int? Pos { private set; get; }
        public Message Message { private set; get; }
        public object[] Data { private set; get; }

        public MessageRecord(MsgCode code)
        {
            Code = code;
            Message = MessageTable.Get()[Code];
            SourceFile = null;
            Line = null;
            Pos = null;
            Data = null;
        }

        public MessageRecord(MsgCode code, string sourceFile, int? line, int? pos, params object[] data)
        {
            Code = code;
            Message = MessageTable.Get()[code];
            SourceFile = sourceFile;
            Line = line;
            Pos = pos;
            Data = data;
        }

        /// <summary>
        /// String message formatter
        /// </summary>
        /// <returns>String message representation</returns>
        public override string ToString()
        {
            //Fully not located message
            if (SourceFile == null)
                return string.Format(
                    "{0} {1}{2}: {3}", // error E1024: error message
                    Convert.ToString(Message.Type).ToLower(), //message type lower case represent
                    Convert.ToString(Message.Type)[0], //first letter for msg code
                    Convert.ToInt32(Code), //message code
                    Message.Format(Data)
                    );
            else
            {
                //only filename 
                if ((Line == null) && (Pos == null))
                {
                    return string.Format(
                        "{0}: {1} {2}{3}: {4}", // test.txt: error E1024: error message
                        SourceFile,
                        Convert.ToString(Message.Type).ToLower(), //message type lower case represent
                        Convert.ToString(Message.Type)[0], //first letter for msg code
                        Convert.ToInt32(Code), //message code
                        Message.Format(Data)
                        );
                }
                else //full location
                {
                    return string.Format(
                        "{0}({1},{2}): {3} {4}{5}: {6}", // test.txt: error E1024: error message
                        SourceFile,
                        Line,
                        Pos,
                        Convert.ToString(Message.Type).ToLower(), //message type lower case represent
                        Convert.ToString(Message.Type)[0], //first letter for msg code
                        Convert.ToInt32(Code), //message code
                        Message.Format(Data)
                        );
                }
            }

        }
    }
}
