using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace CmancNet.ASTInfo
{
    class NativeSubroutine : ISubroutine
    {
        public bool IsNative() => true;

        public bool IsSubroutine() => true;

        public bool IsVariable() => false;

        public bool IsArgument() => false;

        public int ArgumentsCount { private set; get; }

        public bool Return { private set; get; }

        public MethodInfo NativeMethod { private set; get; }

        public NativeSubroutine(MethodInfo method, int argsCnt)
        {
            NativeMethod = method;
            ArgumentsCount = argsCnt;
        }
    }
}
