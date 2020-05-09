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

        /// <summary>
        /// Builds compilation unit
        /// </summary>
        /// <returns>Built assembly</returns>
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
            if (!_emitter.StackEmpty())
                throw new ApplicationException("CLR stack was corrupted");
            //builde assembly
            return _codeHolder.Assembly;
        }

        /// <summary>
        /// Builds subroutine
        /// </summary>
        /// <param name="subNode">Subroutine node</param>
        /// <param name="subSym">Subroutine symbol</param>
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

        /// <summary>
        /// Builds body statement
        /// </summary>
        /// <param name="bodyNode">Body statement node</param>
        private void BuildBodyStatement(ASTBodyStatementNode bodyNode)
        {
            foreach (var s in bodyNode.Statements)
                BuildStatement(s);
        }

        /// <summary>
        /// Builds concrete statement
        /// </summary>
        /// <param name="stmtNode">Statement node</param>
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
            if (stmtNode is ASTReturnStatementNode retNode)
                BuildReturnStatement(retNode);
        }

        /// <summary>
        /// Builds assignment statement
        /// </summary>
        /// <param name="assignNode">Assignment statement node</param>
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

        /// <summary>
        /// Builds concrete expression
        /// </summary>
        /// <param name="exprNode">Expression node</param>
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

        /// <summary>
        /// Builds concrete binary operation
        /// </summary>
        /// <param name="binOpExpr">Binary operation node</param>
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
            if (binOpExpr is ASTNotEqualOpNode notEqualNode)
                BuildNotEqualOpExpr(notEqualNode);
            if (binOpExpr is ASTGreaterOpNode greaterNode)
                BuildGreaterOpExpr(greaterNode);
            if (binOpExpr is ASTLessOpNode lessNode)
                BuildLessOpExpr(lessNode);
        }

        /// <summary>
        /// Builds addition operator
        /// </summary>
        /// <param name="addNode">Addition operator node</param>
        private void BuildAddOpExpr(ASTAddOpNode addNode)
        {
            BuildExpression(addNode.Left);
            if (_emitter.StackPeek() != typeof(decimal))
                _emitter.ToDecimal();
            BuildExpression(addNode.Right);
            if (_emitter.StackPeek() != typeof(decimal))
                _emitter.ToDecimal();
            _emitter.StaticCall(typeof(decimal), "Add", new Type[] { typeof(decimal), typeof(decimal) });
        }

        /// <summary>
        /// Buids subtraction operator
        /// </summary>
        /// <param name="subNode">Subtraction operator node</param>
        private void BuildSubOpExpr(ASTSubOpNode subNode)
        {
            BuildExpression(subNode.Left);
            if (_emitter.StackPeek() != typeof(decimal))
                _emitter.ToDecimal();
            BuildExpression(subNode.Right);
            if (_emitter.StackPeek() != typeof(decimal))
                _emitter.ToDecimal();
            _emitter.StaticCall(typeof(decimal), "Subtract", new Type[] { typeof(decimal), typeof(decimal) });
        }

        /// <summary>
        /// Builds multiply operator
        /// </summary>
        /// <param name="mulNode">Multiply operator node</param>
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
        
        /// <summary>
        /// Builds division operator
        /// </summary>
        /// <param name="divNode">Division operator node</param>
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

        /// <summary>
        /// Builds equal operator
        /// </summary>
        /// <param name="equalNode">Equals operator node</param>
        private void BuildEqualOpExpr(ASTEqualOpNode equalNode)
        {
            BuildExpression(equalNode.Left);
            _emitter.Box();
            BuildExpression(equalNode.Right);
            _emitter.Box();
            _emitter.VirtualCall(typeof(object), "Equals", new Type[] { typeof(object) });
        }

        private void BuildNotEqualOpExpr(ASTNotEqualOpNode notEqualNode)
        {
            BuildExpression(notEqualNode.Left);
            _emitter.Box();
            BuildExpression(notEqualNode.Right);
            _emitter.Box();
            _emitter.VirtualCall(typeof(object), "Equals", new Type[] { typeof(object) });
            _emitter.PushLong(0);
            _emitter.IsEqual();
        }

        /// <summary>
        /// Builds greater operator
        /// </summary>
        /// <param name="greaterNode">Greater operator node</param>
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

        /// <summary>
        /// Builds less operator
        /// </summary>
        /// <param name="lessNode">Less operator node</param>
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

        /// <summary>
        /// Builds negate operator
        /// </summary>
        /// <param name="minusNode">Minus operator node</param>
        private void BuildMinusOpExpr(ASTMinusOpNode minusNode)
        {
            BuildExpression(minusNode.Expression);
            if (_emitter.StackPeek() != typeof(decimal))
                _emitter.ToDecimal();
            _emitter.StaticCall(typeof(decimal), "Negate", new Type[] { typeof(decimal) });
        }

        /// <summary>
        /// Builds not operator
        /// </summary>
        /// <param name="notNode">Not operator node</param>
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
            }
        }

        /// <summary>
        /// Builds call statement
        /// </summary>
        /// <param name="callNode">Call statement node</param>
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

        /// <summary>
        /// Builds if statement
        /// </summary>
        /// <param name="ifNode">If statement node</param>
        private void BuildIfStatement(ASTIfStatementNode ifNode)
        {
            //define labels
            var elseBody = _emitter.DefineLabel();
            var exitIf = _emitter.DefineLabel();

            //calc condition
            BuildExpression(ifNode.Condition);
            if (_emitter.StackPeek() != typeof(bool))
                _emitter.ToBool();
            //check
            _emitter.JumpFalse(elseBody);
            //body
            if (ifNode.TrueBody != null)
            {
                BuildStatement(ifNode.TrueBody);
                _emitter.Jump(exitIf);
            }
            //else body
            _emitter.MarkLabel(elseBody);
            if (ifNode.ElseBody != null)
                BuildStatement(ifNode.ElseBody);
            //exit
            _emitter.MarkLabel(exitIf);
        }

        /// <summary>
        /// Builds while loop statement
        /// </summary>
        /// <param name="whileNode">While statement node</param>
        private void BuildWhileStatement(ASTWhileStatementNode whileNode)
        {
            var condition = _emitter.DefineLabel();
            var body = _emitter.DefineLabel();
            var exitWhile = _emitter.DefineLabel();

            //calc condition
            _emitter.MarkLabel(condition);
            BuildExpression(whileNode.Condition);
            _emitter.Box();
            if (_emitter.StackPeek() != typeof(bool))
                _emitter.ToBool();
            //check condition
            _emitter.JumpFalse(exitWhile);
            //body
            if (whileNode.Body != null)
            {
                BuildStatement(whileNode.Body);
                //jump back
                _emitter.Jump(condition);
            }
            //while exit
            _emitter.MarkLabel(exitWhile);
        }

        /// <summary>
        /// Builds for loop statement
        /// </summary>
        /// <param name="forNode">For loop node</param>
        private void BuildForStatement(ASTForStatementNode forNode)
        {
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
            
            //load counter
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
            //update counter
            _emitter.StaticCall(typeof(decimal), "Add", new Type[] { typeof(decimal), typeof(decimal) });
            _emitter.Box();
            Store(counter.Name);
            //jump to condition
            _emitter.Jump(condition);
            //exit
            _emitter.MarkLabel(exitFor);
        }

        /// <summary>
        /// Builds return statement
        /// </summary>
        /// <param name="retNode">Return statement node</param>
        private void BuildReturnStatement(ASTReturnStatementNode retNode)
        {
            if (retNode.Expression != null)
                BuildExpression(retNode.Expression);
            //TODO: because all user's subroutines returns System.Object on call side
            //and doesn't box
            _emitter.Box();

            _emitter.Jump(_context.MethodEnd);
        }

        /// <summary>
        /// Loads variable value into stack
        /// </summary>
        /// <param name="varNode">Variable node</param>
        private void BuildVariable(ASTVariableNode varNode)
        {
            if (_context.IsLocal(varNode.Name))
                _emitter.LoadLocal(_context.GetLocal(varNode.Name));
            else
                _emitter.LoadArg(_context.GetArgID(varNode.Name));
        }

        /// <summary>
        /// Process concrete literal
        /// </summary>
        /// <param name="literalNode">Common literal node</param>
        private void BuildLiteral(IASTLiteral literalNode)
        {
            if (literalNode is ASTStringLiteralNode strNode)
                BuildString(strNode);
            if (literalNode is ASTNumberLiteralNode numNode)
                BuildDecimal(numNode);
            if (literalNode is ASTNullLiteralNode nullNode)
                BuildNull(nullNode);
            if (literalNode is ASTBoolLiteralNode boolNode)
                BuildBoolean(boolNode);
        }

        /// <summary>
        /// Pushs string into stack
        /// </summary>
        /// <param name="strNode">String value</param>
        private void BuildString(ASTStringLiteralNode strNode)
        {
            _emitter.PushString(strNode.Value);
        }

        /// <summary>
        /// Pushs number as decimal into stack
        /// </summary>
        /// <param name="numNode">Number for convert to decimal</param>
        private void BuildDecimal(ASTNumberLiteralNode numNode)
        {
            if (numNode.Value is long longVal)
                _emitter.PushLong(longVal);
            if (numNode.Value is double dblVal)
                _emitter.PushDouble(dblVal);
            _emitter.ToDecimal();
        }

        /// <summary>
        /// Pushs null into stack
        /// </summary>
        /// <param name="nullNode">Null node</param>
        private void BuildNull(ASTNullLiteralNode nullNode)
        {
            _emitter.PushNull();
        }

        private void BuildBoolean(ASTBoolLiteralNode boolNode)
        {
            _emitter.PushBool(boolNode.Value);
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

        /// <summary>
        /// Compile unit AST root node
        /// </summary>
        private ASTCompileUnitNode _compileUnit;
        /// <summary>
        /// Defined subroutines in current compile unit (with it's own local variables)
        /// </summary>
        private SymbolTable _symbols;
        /// <summary>
        /// Assembly holder. Contains generated code
        /// </summary>
        private AssemblyHolder _codeHolder;
        /// <summary>
        /// IL generator wrapper for current method
        /// </summary>
        private CodeEmiter _emitter;
        /// <summary>
        /// Current method context. Contains local variables, labels, etc
        /// </summary>
        private MethodContext _context;
        /// <summary>
        /// Built user's subroutines
        /// </summary>
        private Dictionary<string, MethodContext> _builtSubs;
    }
}
