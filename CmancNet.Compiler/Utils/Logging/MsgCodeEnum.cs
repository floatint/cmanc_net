﻿using System;
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
        AmbiguousReturn,
        BreakNotAllowed,
        //Warnings starts with 2048
        EmptyBody = 2048,
        EmptyCompileUnit,
        EmptyForStep,
        ImplicitCast,
        UnreachableCode,
        InfinityLoop,
        PermanentlyExecution,

        //Info starts with 4096
        CompilationSuccessful = 4096,
        CompilationFailed,
        CompilationTime,
    }
}
