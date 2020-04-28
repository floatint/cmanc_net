using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.Utils.Logging
{
    class Message
    {
        public Message(MsgType type, string fmtMsg)
        {
            Type = type;
            _formatString = fmtMsg;
        }

        public MsgType Type { private set; get; }

        public string Format(params object[] args)
        {
            //just string
            if (args == null)
                return _formatString;
            //formatted
            return string.Format(_formatString, args);
        }

        private string _formatString;
    }
}
