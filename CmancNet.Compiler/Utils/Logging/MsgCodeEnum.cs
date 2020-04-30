using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.Compiler.Utils.Logging
{
    public enum MsgCode
    {
        //Errors starts with 1024
        CompilerError = 1024,
        EntryPointNotFound,
        UndefinedVariable,
        UndefinedSub,
        RvalueIndexing,
        RvalueAssign,
        ReturnNotFound,
        TooManyArguments,
        TooFewArguments,
        NativeSubOverride,
        UserSubOverride,
        //Warnings starts with 2048
        EmptyBody = 2048,
        EmptyCompileUnit,
        EmptyForStep,
        ImplicitBoolToIntCast,
        ImplicitIntToBoolCast,
        //Info starts with 4096
        CompilationSuccessful = 4096,
        CompilationFailed,
        CompilationTime,
    }
}
