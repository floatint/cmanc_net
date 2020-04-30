﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace CmancNet.Compiler.Codegen
{
    /// <summary>
    /// Low level code generation helper
    /// </summary>
    class CodeEmiter
    {
        public CodeEmiter(ILGenerator ilGenerator)
        {
            _il = ilGenerator;
            _clrStack = new Stack<Type>();
        }


        /// <summary>
        /// Pops value from stack and store int the local variable
        /// </summary>
        public void StoreLocal(LocalBuilder l)
        {
            if (StackEmpty())
                throw new InvalidOperationException("CLR stack is empty");
            _il.Emit(OpCodes.Stloc, l);
            _clrStack.Pop(); //pop value from stack
        }

        public void StoreArg(int idx)
        {
            if (StackEmpty())
                throw new InvalidOperationException("CLR stack is empty");
            _il.Emit(OpCodes.Starg, idx);
            _clrStack.Pop(); //pop value from stack
        }

        /// <summary>
        /// Pushes local variable into stack
        /// </summary>
        /// <param name="l"></param>
        public void LoadLocal(LocalBuilder l)
        {
            _il.Emit(OpCodes.Ldloc, l);
            _clrStack.Push(typeof(object));
        }

        public void LoadArg(int idx)
        {
            if (idx < 4)
            {
                switch (idx)
                {
                    case 0:
                        _il.Emit(OpCodes.Ldarg_0);
                        break;
                    case 1:
                        _il.Emit(OpCodes.Ldarg_1);
                        break;
                    case 2:
                        _il.Emit(OpCodes.Ldarg_2);
                        break;
                    case 3:
                        _il.Emit(OpCodes.Ldarg_3);
                        break;
                }
            }
            else
            {
                _il.Emit(OpCodes.Ldarg_S, idx);
            }
            _clrStack.Push(typeof(object));
        }

        /// <summary>
        /// Boxes value from stack's top
        /// </summary>
        public void Box()
        {
            if (StackEmpty())
                throw new InvalidOperationException("CLR stack is empty");
            if (_clrStack.Peek().IsValueType)
            {
                _il.Emit(OpCodes.Box, _clrStack.Pop());
                _clrStack.Push(typeof(object));
            }
        }

        public void VirtualCall(Type type, string methodName, Type[] args)
        {
            if (args == null)
                args = new Type[0];
            var method = type.GetMethod(methodName, args);
            _il.Emit(OpCodes.Callvirt, method);
            //clean up
            StackPop(method.GetParameters().Count() + 1); // + object instance
            if (method.ReturnType != typeof(void))
                PushRet(method.ReturnType);
        }


        public void StaticCall(Type type, string methodName, Type[] args)
        {
            if (args == null)
                args = new Type[0];
            var method = type.GetMethod(methodName, args);
            _il.Emit(OpCodes.Call, method);
            StackPop(method.GetParameters().Count());
            if (method.ReturnType != typeof(void))
                _clrStack.Push(method.ReturnType);
        }
        //TODO: Ввести StaticCall() который не будет учитывать объект в стеке
        //ObjectCall() который учитывает объект в стеке
        //и ObjectCallvirt() 
        public void Call(MethodInfo mi)
        {
            _il.Emit(OpCodes.Call, mi);
            StackPop(mi.GetParameters().Length); //pop
            if (mi.ReturnType != typeof(void))
                _clrStack.Push(mi.ReturnType);//PushRet(mi.ReturnType);
        }

        //User subroutine call. Without 
        public void UnwrapCall(MethodBuilder mb)
        {
            _il.Emit(OpCodes.Call, mb);
        }


        /*public void CallVirtual(MethodInfo mi)
        {
            _il.Emit(OpCodes.Call, mi);
            StackPop(mi.GetParameters().Length); //pop 
            if (mi.ReturnParameter.IsRetval)
                PushRet(mi.ReturnParameter.GetType());
        }*/

        public void PushString(string val)
        {
            _il.Emit(OpCodes.Ldstr, val);
            _clrStack.Push(val.GetType());
        }


        public void PushLong(long val)
        {
            _il.Emit(OpCodes.Ldc_I8, val);
            _clrStack.Push(val.GetType());
        }

        public void PushDouble(double val)
        {
            _il.Emit(OpCodes.Ldc_R8, val);
            _clrStack.Push(val.GetType());
        }
        
        public void ToDecimal()
        {
            if (StackEmpty())
                throw new InvalidOperationException("CLR stack is empty");
            _il.Emit(OpCodes.Call, typeof(Convert).GetMethod("ToDecimal", new Type[] { _clrStack.Pop() }));
            _clrStack.Push(typeof(decimal));
        }

        public void Nop()
        {
            _il.Emit(OpCodes.Nop);
        }

        public void Ret()
        {
            _il.Emit(OpCodes.Ret);
        }

        //TODO: продумать
        public void IsEqual()
        {
            _il.Emit(OpCodes.Ceq);
            _clrStack.Push(typeof(bool));
        }

        public void IsGreater()
        {
            _il.Emit(OpCodes.Cgt);
            _clrStack.Push(typeof(bool));
        }

        public void IsLess()
        {
            _il.Emit(OpCodes.Clt);
            _clrStack.Push(typeof(bool));
        }


        public int StackSize() => _clrStack.Count;

        private void PushRet(Type retType)
        {
            _clrStack.Push(retType);
        }

        public void StackPop(int cnt)
        {
            for (int i = 0; i < cnt; i++)
                _clrStack.Pop();
        }

        public void StackPush(Type t)
        {
            _clrStack.Push(t);
        }

        public Type StackPeek()
        {
            return _clrStack.Peek();
        }

        private bool StackEmpty()
        {
            return _clrStack.Count == 0;
        }


        private ILGenerator _il;
        private Stack<Type> _clrStack;
    }
}