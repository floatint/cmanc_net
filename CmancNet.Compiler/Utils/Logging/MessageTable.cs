using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.Compiler.Utils.Logging
{
    /// <summary>
    /// Singleton message table for compiler logging
    /// </summary>
    class MessageTable
    {
        /// <summary>
        /// Get message table instance
        /// </summary>
        /// <returns></returns>
        public static MessageTable Get()
        {
            if (_tableInstance == null)
                _tableInstance = new MessageTable();
            return _tableInstance;
        }

        public Message this[MsgCode code]
        {
            get
            {
                return _messages[code];
            }
        }

        /// <summary>
        /// Initialize message table
        /// </summary>
        private MessageTable()
        {
            _messages = new Dictionary<MsgCode, Message>();
            //errors
            _messages.Add(MsgCode.CompilerError, new Message(MsgType.Error, "{0}"));
            _messages.Add(MsgCode.EntryPointNotFound, new Message(MsgType.Error, "entry point with signature '{0}' not found"));
            _messages.Add(MsgCode.UndefinedVariable, new Message(MsgType.Error, "'${0}' undefined variable"));
            _messages.Add(MsgCode.UndefinedSub, new Message(MsgType.Error, "'{0}' undefined subroutine"));
            _messages.Add(MsgCode.RvalueIndexing, new Message(MsgType.Error, "indexing canno't apply for rvalue"));
            _messages.Add(MsgCode.RvalueAssign, new Message(MsgType.Error, "assign statement requires lvalue, but rvalue found"));
            _messages.Add(MsgCode.ReturnNotFound, new Message(MsgType.Error, "statement requires a return value, but the \'{0}\' returns void"));
            _messages.Add(MsgCode.TooManyArguments, new Message(MsgType.Error, "too few arguments, {0} required, but {1} found"));
            _messages.Add(MsgCode.TooFewArguments, new Message(MsgType.Error, "too many arguments, {0} required, but {1} found"));
            _messages.Add(MsgCode.NativeSubOverride, new Message(MsgType.Error, "native subroutine \'{0}\' override"));
            _messages.Add(MsgCode.UserSubOverride, new Message(MsgType.Error, "subroutine \'{0}\' override"));
            _messages.Add(MsgCode.AmbiguousReturn, new Message(MsgType.Error, "ambiguous return statement. expected {0}, but {1} found"));
            _messages.Add(MsgCode.BreakNotAllowed, new Message(MsgType.Error, "break statement not allowed outside of loops"));
            //warnings
            _messages.Add(MsgCode.EmptyBody, new Message(MsgType.Warning, "empty code block"));
            _messages.Add(MsgCode.EmptyCompileUnit, new Message(MsgType.Warning, "empty compile unit"));
            _messages.Add(MsgCode.EmptyForStep, new Message(MsgType.Warning, "empty for step statement. By default = 1"));
            _messages.Add(MsgCode.ImplicitCast, new Message(MsgType.Warning, "implicit cast from '{0}' to '{1}'"));
            _messages.Add(MsgCode.UnreachableCode, new Message(MsgType.Warning, "unreachable code detected"));
            _messages.Add(MsgCode.InfinityLoop, new Message(MsgType.Warning, "infinity loop detected"));
            _messages.Add(MsgCode.PermanentlyExecution, new Message(MsgType.Warning, "permanently execution detected"));
            //infos
            _messages.Add(MsgCode.CompilationSuccessful, new Message(MsgType.Info, "compilation was successfuly with {0} errors, {1} warnings"));
            _messages.Add(MsgCode.CompilationFailed, new Message(MsgType.Info, "compilation failed with {0} errors, {1} warnings"));
            _messages.Add(MsgCode.CompilationTime, new Message(MsgType.Info, "compilation time: {0:f3} seconds"));
        }

        private Dictionary<MsgCode, Message> _messages;
        private static MessageTable _tableInstance;
    }
}
