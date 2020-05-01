using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using CmancNet.Compiler.ASTParser.AST.Statements;
using CmancNet.Compiler.ASTParser.AST.Expressions;
using CmancNet.Compiler.ASTParser.AST.Expressions.Binary;
using CmancNet.Compiler.ASTParser.AST.Expressions.Unary;
using CmancNet.Compiler.ASTInfo;

namespace CmancNet.Compiler.Codegen
{
    class CodeBuilder
    {
        public CodeBuilder(ASTCompileUnitNode compileUnit, SymbolTable symbolTable)
        {
            _compileUnit = compileUnit;
            _symbols = symbolTable;
            _codeHolder = new AssemblyHolder(compileUnit.Name);
            _builtSubs = new Dictionary<string, MethodContext>();
        }

        //public IList<string> Messages

        /// <summary>
        /// Build compilation unit
        /// </summary>
        /// <returns></returns>
        public AssemblyBuilder Build()
        {
            if (_compileUnit.Procedures != null)
            {
                foreach (var s in _compileUnit.Procedures)
                {
                    BuildSubroutine(s, (UserSubroutine)_symbols.FindSymbol(s.Name));
                }
            }
            //clr stack check
            if (_emitter.StackEmpty())
                throw new ApplicationException("CLR stack was corrupted");
            //builde assembly
            return _codeHolder.Assembly;
        }

        private void BuildSubroutine(ASTSubStatementNode subNode, UserSubroutine subSym)
        {
            _context = new MethodContext(_codeHolder.GetMethodBuilder(subNode.Name, subSym), subSym.GetLocalsList());
            _emitter = new CodeEmiter(_context.ILGenerator);
            if (subNode.Body != null)
            {
                BuildStatement(subNode.Body);
            }
            else
            {
                _emitter.Nop();
            }
            _emitter.MarkLabel(_context.MethodEnd);
            _emitter.Ret();
            _builtSubs.Add(subNode.Name, _context); //add to built
        }

        private void BuildBodyStatement(ASTBodyStatementNode bodyNode)
        {
            foreach (var s in bodyNode.Statements)
                BuildStatement(s);
        }

        private void BuildStatement(IASTStatementNode stmtNode)
        {
            if (stmtNode is ASTBodyStatementNode bodyNode)
                BuildBodyStatement(bodyNode);
            if (stmtNode is ASTAssignStatementNode assignNode)
                BuildAssignStatement(assignNode);
            if (stmtNode is ASTCallStatementNode callNode)
                BuildCallStatement(callNode);
            if (stmtNode is ASTIfStatementNode ifNode)
                BuildIfStatement(ifNode);
            if (stmtNode is ASTWhileStatementNode whileNode)
                BuildWhileStatement(whileNode);
            if (stmtNode is ASTForStatementNode forNode)
                BuildForStatement(forNode);
        }

        private void BuildAssignStatement(ASTAssignStatementNode assignNode)
        {
            //to dictionary assign
            if (assignNode.Left is ASTIndexOpNode indexNode)
            {
                //get object
                BuildIndexOp(indexNode);
                //get index
                BuildExpression(indexNode.Index);
                _emitter.Box();
                //get value
                BuildExpression(assignNode.Right);
                _emitter.Box();
                //call setter
                _emitter.VirtualCall(typeof(Dictionary<object, object>), "set_Item", new Type[] { typeof(object), typeof(object) });
            }
            if (assignNode.Left is ASTVariableNode varNode)//to local assign
            {
                BuildExpression(assignNode.Right);
                _emitter.Box();
                if (_context.IsLocal(varNode.Name))
                    _emitter.StoreLocal(_context.GetLocal(varNode.Name));
                else
                    _emitter.StoreArg(_context.GetArgID(varNode.Name));
            }
        } 

        private void BuildExpression(IASTExprNode exprNode)
        {
            if (exprNode is IASTLiteral literalNode)
                BuildLiteral(literalNode);
            if (exprNode is ASTVariableNode varNode)
                BuildVariable(varNode);
            if (exprNode is ASTIndexOpNode indexNode)
            {
                if (indexNode.Expression is ASTIndexOpNode subIndexOp)
                    BuildExpression((IASTExprNode)subIndexOp);
                if (indexNode.Expression is ASTVariableNode indexVarNode)
                {
                    BuildVariable(indexVarNode);
                }
                BuildExpression(indexNode.Index);
                _emitter.Box();
                _emitter.VirtualCall(typeof(Dictionary<object, object>), "get_Item", new Type[] { typeof(object) });
            }
            if (exprNode is ASTCallStatementNode callNode)
                BuildCallStatement(callNode);
            if (exprNode is IASTBinOpNode binOpNode)
                BuildBinOpExpr(binOpNode);
            //TODO: unary operators
            if (exprNode is ASTMinusOpNode minusNode)
                BuildMinusOpExpr(minusNode);
            if (exprNode is ASTNotOpNode notNode)
                BuildNotOpExpr(notNode);
        }

        private void BuildBinOpExpr(IASTBinOpNode binOpExpr)
        {
            //arithm
            if (binOpExpr is ASTAddOpNode addNode)
                BuildAddOpExpr(addNode);
            if (binOpExpr is ASTSubOpNode subOpNode)
                BuildSubOpExpr(subOpNode);
            if (binOpExpr is ASTMulOpNode mulOpNode)
                BuildMulOpExpr(mulOpNode);
            if (binOpExpr is ASTDivOpNode divOpNode)
                BuildDivOpExpr(divOpNode);
            //comparing
            if (binOpExpr is ASTEqualOpNode equalNode)
                BuildEqualOpExpr(equalNode);
            if (binOpExpr is ASTGreaterOpNode greaterNode)
                BuildGreaterOpExpr(greaterNode);
            if (binOpExpr is ASTLessOpNode lessNode)
                BuildLessOpExpr(lessNode);
        }

        private void BuildAddOpExpr(ASTAddOpNode addNode)
        {
            BuildExpression(addNode.Left);
            if (_emitter.StackPeek() != typeof(decimal))
                _emitter.ToDecimal();
            BuildExpression(addNode.Right);
            if (_emitter.StackPeek() != typeof(decimal))
                _emitter.ToDecimal();
            _emitter.Call(typeof(decimal).GetMethod("Add", new Type[] { typeof(decimal), typeof(decimal) }));
        }

        private void BuildSubOpExpr(ASTSubOpNode subNode)
        {
            BuildExpression(subNode.Left);
            if (_emitter.StackPeek() != typeof(decimal))
                _emitter.ToDecimal();
            BuildExpression(subNode.Right);
            if (_emitter.StackPeek() != typeof(decimal))
                _emitter.ToDecimal();
            _emitter.Call(typeof(decimal).GetMethod("Subtract", new Type[] { typeof(decimal), typeof(decimal) }));
        }

        private void BuildMulOpExpr(ASTMulOpNode mulNode)
        {
            BuildExpression(mulNode.Left);
            if (_emitter.StackPeek() != typeof(decimal))
                _emitter.ToDecimal();
            BuildExpression(mulNode.Right);
            if (_emitter.StackPeek() != typeof(decimal))
                _emitter.ToDecimal();
            _emitter.StaticCall(typeof(decimal), "Multiply", new Type[] { typeof(decimal), typeof(decimal)});
        }

        private void BuildDivOpExpr(ASTDivOpNode divNode)
        {
            BuildExpression(divNode.Left);
            if (_emitter.StackPeek() != typeof(decimal))
                _emitter.ToDecimal();
            BuildExpression(divNode.Right);
            if (_emitter.StackPeek() != typeof(decimal))
                _emitter.ToDecimal();
            _emitter.StaticCall(typeof(decimal), "Divide", new Type[] { typeof(decimal), typeof(decimal) });
        }

        private void BuildEqualOpExpr(ASTEqualOpNode equalNode)
        {
            BuildExpression(equalNode.Left);
            _emitter.Box();
            BuildExpression(equalNode.Right);
            _emitter.Box();
            _emitter.VirtualCall(typeof(object), "Equals", new Type[] { typeof(object) });
        }

        private void BuildGreaterOpExpr(ASTGreaterOpNode greaterNode)
        {
            BuildExpression(greaterNode.Left);
            if (_emitter.StackPeek() != typeof(decimal))
                _emitter.ToDecimal();
            BuildExpression(greaterNode.Right);
            if (_emitter.StackPeek() != typeof(decimal))
                _emitter.ToDecimal();
            _emitter.StaticCall(typeof(decimal), "Compare", new Type[] { typeof(decimal), typeof(decimal)});
            _emitter.PushLong(0);
            _emitter.IsGreater();
        }

        private void BuildLessOpExpr(ASTLessOpNode lessNode)
        {
            BuildExpression(lessNode.Left);
            if (_emitter.StackPeek() != typeof(decimal))
                _emitter.ToDecimal();
            BuildExpression(lessNode.Right);
            if (_emitter.StackPeek() != typeof(decimal))
                _emitter.ToDecimal();
            _emitter.StaticCall(typeof(decimal), "Compare", new Type[] { typeof(decimal), typeof(decimal) });
            _emitter.PushLong(0);
            _emitter.IsLess();
        }

        private void BuildMinusOpExpr(ASTMinusOpNode minusNode)
        {
            BuildExpression(minusNode.Expression);
            if (_emitter.StackPeek() != typeof(decimal))
                _emitter.ToDecimal();
            _emitter.StaticCall(typeof(decimal), "Negate", new Type[] { typeof(decimal) });
        }

        private void BuildNotOpExpr(ASTNotOpNode notNode)
        {
            BuildExpression(notNode.Expression);
            _emitter.Box();
            _emitter.StaticCall(typeof(Convert), "ToBoolean", new Type[] { typeof(object) });
            _emitter.PushLong(0);
            _emitter.IsEqual();
        }

        //TODO: пересмотреть
        private void BuildIndexOp(ASTIndexOpNode indexOp)
        {
            if (indexOp.Expression is ASTIndexOpNode subIndexOp)
            {
                //get sub object
                BuildIndexOp(subIndexOp);
                //get index
                BuildExpression(subIndexOp.Index);
                _emitter.Box();
                //get item
                _emitter.VirtualCall(typeof(Dictionary<object, object>), "get_Item", new Type[] { typeof(object) });
            }
            if (indexOp.Expression is ASTVariableNode varNode)
            {
                //get root object
                BuildVariable(varNode);
                //calc index
                //BuildExpression(indexOp.Index);
                //_emitter.Box();
                //get item
                //_emitter.InternalCallvirt(typeof(Dictionary<object, object>), "get_Item", new Type[] { typeof(object) });
            }
        }

        private void BuildCallStatement(ASTCallStatementNode callNode)
        {
            int argCnt = 0;
            //push arguments
            if (callNode.Arguments != null)
            {
                argCnt = callNode.Arguments.Expressions.Count;
                foreach (var e in callNode.Arguments.Expressions)
                {
                    BuildExpression(e);
                    _emitter.Box();
                }
            }
            //get symbol
            var symbol = (ISubroutine)_symbols.FindSymbol(callNode.ProcedureName);
            if (symbol is NativeSubroutine nativeSub)
            {
                _emitter.Call(nativeSub.NativeMethod);
            }
            if (symbol is UserSubroutine userSub)
            {
                var m = _builtSubs[callNode.ProcedureName].Builder;
                _emitter.UnwrapCall(m);
                //hand handling because type not built
                _emitter.StackPop(userSub.ArgumentsCount);
                if (userSub.Return)
                    _emitter.StackPush(typeof(object));
            }
        }

        private void BuildIfStatement(ASTIfStatementNode ifNode)
        {
            //define labels
            var elseBody = _emitter.DefineLabel();
            var exitIf = _emitter.DefineLabel();

            BuildExpression(ifNode.Condition);
            if (_emitter.StackPeek() != typeof(bool))
                _emitter.ToBool();
            _emitter.JumpFalse(elseBody);
            if (ifNode.TrueBody != null)
            {
                BuildStatement(ifNode.TrueBody);
                _emitter.Jump(exitIf);
            }
            _emitter.MarkLabel(elseBody);
            if (ifNode.ElseBody != null)
                BuildStatement(ifNode.ElseBody);
            _emitter.MarkLabel(exitIf);
            //_emitter.Nop();
        }

        private void BuildWhileStatement(ASTWhileStatementNode whileNode)
        {
            var condition = _emitter.DefineLabel();
            var body = _emitter.DefineLabel();
            var exitWhile = _emitter.DefineLabel();

            _emitter.MarkLabel(condition);
            BuildExpression(whileNode.Condition);
            if (_emitter.StackPeek() != typeof(bool))
                _emitter.ToBool();
            _emitter.JumpFalse(exitWhile);
            //body
            if (whileNode.Body != null)
            {
                BuildStatement(whileNode.Body);
                _emitter.Jump(condition);
            }
            //while exit
            _emitter.MarkLabel(exitWhile);
        }

        private void BuildForStatement(ASTForStatementNode forNode)
        {
            //var nextStep = _emitter.DefineLabel();
            var condition = _emitter.DefineLabel();
            var exitFor = _emitter.DefineLabel();

            ASTVariableNode counter = null;
            //init counter if needed
            if (forNode.Counter is ASTAssignStatementNode assignNode)
            {
                BuildStatement(assignNode);
                counter = (ASTVariableNode)assignNode.Left;
            }
            else
            {
                counter = (ASTVariableNode)forNode.Counter;
            }
            //check condition
            _emitter.MarkLabel(condition);
            BuildExpression(forNode.Condition);
            _emitter.JumpFalse(exitFor);
            //body
            if (forNode.Body != null)
                BuildStatement(forNode.Body);
            //_emitter.Jump(condition);
            //inc counter
            BuildExpression(counter);
            _emitter.ToDecimal();
            //build step
            if (forNode.Step != null)
            {
                BuildExpression(forNode.Step);
                if (_emitter.StackPeek() != typeof(decimal))
                    _emitter.ToDecimal();
            }
            else //default step
            {
                _emitter.PushLong(1);
                _emitter.ToDecimal();
            }
            _emitter.StaticCall(typeof(decimal), "Add", new Type[] { typeof(decimal), typeof(decimal) });
            _emitter.Box();
            Store(counter.Name);
            //_emitter.StoreLocal(_context.GetLocal(counter.Name));
            _emitter.Jump(condition);
            //exit
            _emitter.MarkLabel(exitFor);
        }

        private void BuildVariable(ASTVariableNode varNode)
        {
            if (_context.IsLocal(varNode.Name))
                _emitter.LoadLocal(_context.GetLocal(varNode.Name));
            else
                _emitter.LoadArg(_context.GetArgID(varNode.Name));
        }

        private void BuildLiteral(IASTLiteral literalNode)
        {
            if (literalNode is ASTStringLiteralNode strNode)
                BuildString(strNode);
            if (literalNode is ASTNumberLiteralNode numNode)
                BuildDecimal(numNode);
        }

        private void BuildString(ASTStringLiteralNode strNode)
        {
            _emitter.PushString(strNode.Value);
        }

        private void BuildDecimal(ASTNumberLiteralNode numNode)
        {
            if (numNode.Value is long longVal)
                _emitter.PushLong(longVal);
            if (numNode.Value is double dblVal)
                _emitter.PushDouble(dblVal);
            _emitter.ToDecimal();
        }

        /// <summary>
        /// Pops value from stack and store in local var or argument
        /// </summary>
        /// <param name="name">Variable name</param>
        private void Store(string name)
        {
            if (_context.IsLocal(name))
                _emitter.StoreLocal(_context.GetLocal(name));
            else
                _emitter.StoreArg(_context.GetArgID(name));
        }


        private ASTCompileUnitNode _compileUnit;
        private SymbolTable _symbols; //top level symbol table
        private AssemblyHolder _codeHolder;
        private CodeEmiter _emitter; //low level code emitter of current method
        private MethodContext _context; //current method context
        private Dictionary<string, MethodContext> _builtSubs; //built subroutines
    }
}
