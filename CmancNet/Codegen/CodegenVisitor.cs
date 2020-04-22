using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace CmancNet.Codegen
{
    class CodegenVisitor
    {
        void IL()
        {
            object a = 100;
            Console.WriteLine(a);
        }

        void IL2()
        {
            object a = "kesk";
        }

        void IL3(String[] args)
        {
            args = null;
        }

        public AssemblyBuilder BuildAssembly(string name)
        {
            System.Reflection.AssemblyName assemblyName = new System.Reflection.AssemblyName(name);
            AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.Save
                );
            
            ModuleBuilder mb =  ab.DefineDynamicModule(assemblyName.Name, "test.exe");
            
            TypeBuilder tb = mb.DefineType("Program", 
                System.Reflection.TypeAttributes.Public);
            MethodBuilder metB = tb.DefineMethod("proc_1",
                System.Reflection.MethodAttributes.Public 
                | System.Reflection.MethodAttributes.Static
                | System.Reflection.MethodAttributes.HideBySig
                );
            
            metB.SetParameters(typeof(string[]));
            ILGenerator il = metB.GetILGenerator();
            LocalBuilder lb = il.DeclareLocal(typeof(object));
            il.DeclareLocal(typeof(Object));
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldc_I4_S, 100);
            il.Emit(OpCodes.Box, typeof(int));
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Call, 
                typeof(System.Console).GetMethod("WriteLine", new Type[] { typeof(Object)}));
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ret);

            tb.CreateType();

            ab.SetEntryPoint(tb.GetMethod("proc_1", new Type[] { typeof(string[]) }));
            return ab;

        }
    }
}
