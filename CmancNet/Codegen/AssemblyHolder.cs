using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using CmancNet.ASTInfo;

namespace CmancNet.Codegen
{
    /// <summary>
    /// AssemblyBuilder wrapper
    /// </summary>
    class AssemblyHolder
    {
        public AssemblyHolder(string assemblyName)
        {
            _assemblyName = new AssemblyName(assemblyName);
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.Save);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(_assemblyName.Name, _assemblyName.Name + ".exe");
            _typeBuilder = _moduleBuilder.DefineType("Program", TypeAttributes.Public);
        }

        public AssemblyBuilder Assembly
        {
            get
            {
                _typeBuilder.CreateType();
                return _assemblyBuilder;
            }
        }

        public MethodBuilder GetMethodBuilder(string name, UserSubroutine symbol)
        {
            MethodBuilder mb;
            mb = _typeBuilder.DefineMethod(
                name,
                MethodAttributes.Public |
                MethodAttributes.Static |
                MethodAttributes.HideBySig
                );
            if (symbol.Return)
                mb.SetReturnType(typeof(object));
            if (symbol.ArgumentsCount > 0)
                mb.SetParameters(Enumerable.Repeat(typeof(object), symbol.ArgumentsCount).ToArray());
            return mb;
        }

        private AssemblyName _assemblyName;
        private AssemblyBuilder _assemblyBuilder;
        private ModuleBuilder _moduleBuilder;
        private TypeBuilder _typeBuilder;
    }
}
