using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.Utils.Logging
{
    enum MsgCode
    {
        //Errors starts with 1024
        UndefinedVariable = 1024,
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
    }
}
