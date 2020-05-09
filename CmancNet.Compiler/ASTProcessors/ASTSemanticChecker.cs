using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmancNet.Compiler.ASTParser.AST;
using CmancNet.Compiler.ASTParser.AST.Statements;
using CmancNet.Compiler.ASTParser.AST.Expressions;
using CmancNet.Compiler.ASTParser.AST.Expressions.Unary;
using CmancNet.Compiler.ASTParser.AST.Expressions.Binary;
using CmancNet.Compiler.ASTProcessors.Analysis;
using CmancNet.Compiler.Utils.Logging;
using CmancNet.Compiler.ASTInfo;

namespace CmancNet.Compiler.ASTProcessors
{
    class ASTSemanticChecker
    {
        public ASTSemanticChecker(ASTCompileUnitNode compileUnit, SymbolTable symbolTable)
        {
            _compileUnit = compileUnit;
            _symbolTable = symbolTable;
            _messages = new List<MessageRecord>();
        }

        public IEnumerable<MessageRecord> Messages => _messages.AsEnumerable();

        public bool IsValid()
        {
            if (_compileUnit.Procedures != null)
            {
                foreach (var s in _compileUnit.Procedures)
                {
                    CheckSubroutine(s);
                }
            }
            else
            {
                _messages.Add(new MessageRecord(
                    MsgCode.EmptyCompileUnit,
                    _compileUnit.SourcePath,
                    _compileUnit.StartLine,
                    _compileUnit.StartPos)
                );
            }
            return _messages.Where(x => x.Message.Type == MsgType.Error).Count() == 0 ? true : false;
        }


        private void CheckSubroutine(ASTSubStatementNode subNode)
        {
            _currentSub = (UserSubroutine)_symbolTable.FindSymbol(subNode.Name);
            if (subNode.Body != null)
            {
                CheckBody(subNode.Body);
            } else
            {
                _messages.Add(new MessageRecord(
                    MsgCode.EmptyBody,
                    subNode.SourcePath,
                    subNode.StartLine,
                    subNode.StartPos,
                    null
                    ));
            }
        }

        private void CheckBody(ASTBodyStatementNode bodyNode)
        {
            foreach (var s in bodyNode.Statements)
                CheckStatement(s);
        }

        private void CheckStatement(IASTStatementNode stmtNode)
        {
            if (stmtNode is ASTAssignStatementNode assignNode)
                CheckAssignStatement(assignNode);
            if (stmtNode is ASTCallStatementNode callNode)
                CheckSubCall(callNode, false);
            if (stmtNode is ASTIfStatementNode ifNode)
                CheckIfStatement(ifNode);
            if (stmtNode is ASTWhileStatementNode whileNode)
                CheckWhileStatement(whileNode);
            if (stmtNode is ASTForStatementNode forNode)
                CheckForStatement(forNode);
            if (stmtNode is ASTReturnStatementNode retNode)
                CheckReturnStatement(retNode);
        }

        private void CheckAssignStatement(ASTAssignStatementNode assignNode)
        {   
            if (IsRvalue(assignNode.Left))
            {
                _messages.Add(new MessageRecord(
                    MsgCode.RvalueAssign,
                    assignNode.SourcePath,
                    assignNode.StartLine,
                    assignNode.StartPos,
                    null
                    ));
            }
            else
                CheckExpression(assignNode.Left);
            CheckExpression(assignNode.Right);
        }

        private void CheckExpression(IASTExprNode exprNode)
        {
            if (exprNode is ASTVariableNode varNode)
                CheckVariable(varNode);
            if (exprNode is ASTCallStatementNode callNode)
                CheckSubCall(callNode, true);
            if (exprNode is IASTBinOpNode binOpNode)
                CheckBinOp(binOpNode);
            if (exprNode is IASTUnarOpNode unarOpNode)
                CheckUnarOp(unarOpNode);
        }

        private void CheckVariable(ASTVariableNode varNode)
        {
            ISymbol definedVar = _currentSub.FindLocal(varNode.Name);
            if (definedVar == null)
            {
                _messages.Add(new MessageRecord(
                    MsgCode.UndefinedVariable,
                    varNode.SourcePath,
                    varNode.StartLine,
                    varNode.StartPos,
                    varNode.Name
                    ));
            }
        }

        private void CheckBinOp(IASTBinOpNode binOpNode)
        {
            CheckExpression(binOpNode.Left);
            CheckExpression(binOpNode.Right);
            //equal and not equal - exception
            if (!(binOpNode is ASTEqualOpNode))
            {
                CheckImplicitCast(binOpNode.Left, typeof(decimal));
                CheckImplicitCast(binOpNode.Right, typeof(decimal));
            }
            else
            {
                CheckImplicitCast(binOpNode.Left, typeof(bool));
                CheckImplicitCast(binOpNode.Right, typeof(bool));
            }
        }

        private void CheckUnarOp(IASTUnarOpNode unarOpNode)
        {
            if (unarOpNode is ASTIndexOpNode indexOp)
                CheckIndexOp(indexOp);
            if (unarOpNode is ASTNotOpNode notOp)
                CheckNotOp(notOp);
            if (unarOpNode is ASTMinusOpNode minusOp)
                CheckMinusOp(minusOp);
        }

        private void CheckIndexOp(ASTIndexOpNode indexOpNode)
        {
            if (!(indexOpNode.Expression is ASTIndexOpNode) &&
                !(indexOpNode.Expression is ASTVariableNode) &&
                !(indexOpNode.Expression is ASTCallStatementNode)
                )
            {
                _messages.Add(new MessageRecord(
                    MsgCode.RvalueIndexing,
                    indexOpNode.SourcePath,
                    indexOpNode.StartLine,
                    indexOpNode.StartPos,
                    null
                    ));
            }
            else
            {
                if (indexOpNode.Expression is ASTIndexOpNode indexOp)
                    CheckIndexOp(indexOp);
                if (indexOpNode.Expression is ASTVariableNode varNode)
                    CheckVariable(varNode);
                if (indexOpNode.Expression is ASTCallStatementNode callNode)
                    CheckSubCall(callNode, true);
            }
            //TODO: null index check
            CheckExpression(indexOpNode.Index);
        }

        private void CheckNotOp(ASTNotOpNode notOp)
        {
            CheckExpression(notOp.Expression);
            CheckImplicitCast(notOp.Expression, typeof(bool));
        }

        private void CheckMinusOp(ASTMinusOpNode minusOp)
        {
            CheckExpression(minusOp.Expression);
            CheckImplicitCast(minusOp.Expression, typeof(decimal));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callNode">AST node of subroutine call</param>
        /// <param name="requireReturn">Requires subroutine return value</param>
        private void CheckSubCall(ASTCallStatementNode callNode, bool requireReturn)
        {
            //check define
            ISubroutine sub = (ISubroutine)_symbolTable.FindSymbol(callNode.ProcedureName);
            if (sub == null)
            {
                _messages.Add(new MessageRecord(
                    MsgCode.UndefinedSub,
                    callNode.SourcePath,
                    callNode.StartLine,
                    callNode.StartPos,
                    callNode.ProcedureName
                    ));
            }
            else
            {
                //check return value
                if (requireReturn)
                {
                    if (!sub.Return)
                    {
                        _messages.Add(new MessageRecord(
                            MsgCode.ReturnNotFound,
                            callNode.SourcePath,
                            callNode.StartLine,
                            callNode.StartPos,
                            callNode.ProcedureName
                            ));
                    }
                }

                //check arguments count
                int argCnt = sub.ArgumentsCount;
                if (callNode.Arguments != null)
                {
                    if (argCnt > callNode.Arguments.Children.Count)
                    {
                        _messages.Add(new MessageRecord(
                            MsgCode.TooFewArguments,
                            callNode.SourcePath,
                            callNode.StartLine,
                            callNode.StartPos,
                            argCnt,
                            callNode.Arguments.Children.Count
                            ));
                    }
                    if (argCnt < callNode.Arguments.Children.Count)
                    {
                        _messages.Add(new MessageRecord(
                            MsgCode.TooManyArguments,
                            callNode.SourcePath,
                            callNode.StartLine,
                            callNode.StartPos,
                            argCnt,
                            callNode.Arguments.Children.Count
                            ));
                    }
                    //check arguments
                    foreach(var a in callNode.Arguments.Expressions)
                    {
                        CheckExpression(a);
                    }
                }
                else //no arguments
                {
                    if (argCnt != 0)
                    {
                        _messages.Add(new MessageRecord(
                           MsgCode.TooFewArguments,
                           callNode.SourcePath,
                           callNode.StartLine,
                           callNode.StartPos,
                           argCnt,
                           0
                           ));
                    }
                }
            }
        }

        private void CheckIfStatement(ASTIfStatementNode ifNode)
        {
            CheckExpression(ifNode.Condition);
            CheckImplicitCast(ifNode.Condition, typeof(bool));
            //TODO: сначала высчитывать условие а потом смотреть тела
            if (ASTExprHelper.IsValuable(ifNode.Condition))
            {
                var isTrue = Convert.ToBoolean(ASTExprHelper.GetValue(ifNode.Condition));
                //permanetnly true
                if (isTrue)
                {
                    if (ifNode.TrueBody != null)
                    {
                        _messages.Add(new MessageRecord(
                            MsgCode.PermanentlyExecution,
                            ifNode.TrueBody.SourcePath,
                            ifNode.TrueBody.StartLine,
                            ifNode.TrueBody.StartPos
                            ));
                    }
                    if (ifNode.ElseBody != null)
                    {
                        _messages.Add(new MessageRecord(
                            MsgCode.UnreachableCode,
                            ifNode.ElseBody.SourcePath,
                            ifNode.ElseBody.StartLine,
                            ifNode.ElseBody.StartPos
                            ));
                    }
                }
                else //permanently false
                {
                    if (ifNode.TrueBody != null)
                    {
                        _messages.Add(new MessageRecord(
                            MsgCode.UnreachableCode,
                            ifNode.TrueBody.SourcePath,
                            ifNode.TrueBody.StartLine,
                            ifNode.TrueBody.StartPos
                            ));
                    }
                    if (ifNode.ElseBody != null)
                    {
                        _messages.Add(new MessageRecord(
                            MsgCode.PermanentlyExecution,
                            ifNode.ElseBody.SourcePath,
                            ifNode.ElseBody.StartLine,
                            ifNode.ElseBody.StartPos
                            ));
                    }
                }
            }
            if (ifNode.TrueBody != null)
            {
                CheckBody(ifNode.TrueBody);
            }
            if (ifNode.ElseBody != null)
            {
                CheckBody(ifNode.ElseBody);
            }
        }

        private void CheckWhileStatement(ASTWhileStatementNode whileNode)
        {
            //check condition type
            CheckImplicitCast(whileNode.Condition, typeof(bool));
            CheckExpression(whileNode.Condition);
            //CheckImplicitCast(whileNode.Condition, typeof(bool));            
            if (whileNode.Body != null)
            {
                //check infinity loop or dead code
                if (ASTExprHelper.IsValuable(whileNode.Condition))
                {
                    //infinity loop
                    if (Convert.ToBoolean(ASTExprHelper.GetValue(whileNode.Condition))) {
                        _messages.Add(new MessageRecord(
                            MsgCode.InfinityLoop,
                            whileNode.Body.SourcePath,
                            whileNode.Body.StartLine,
                            whileNode.Body.StartPos
                            ));
                    }
                    else //unreachable code
                    {
                        _messages.Add(new MessageRecord(
                            MsgCode.UnreachableCode,
                            whileNode.Body.SourcePath,
                            whileNode.Body.StartLine,
                            whileNode.Body.StartPos
                            ));
                    }
                }
                //body check
                CheckBody(whileNode.Body);
            } else
            {
                _messages.Add(new MessageRecord(
                    MsgCode.EmptyBody,
                    whileNode.SourcePath,
                    whileNode.StartLine,
                    whileNode.StartPos
                    ));
            }
        }

        private void CheckForStatement(ASTForStatementNode forNode)
        {
            //check counter
            if (forNode.Counter is ASTVariableNode varNode)
                CheckVariable(varNode);
            if (forNode.Counter is ASTAssignStatementNode assignNode)
                CheckAssignStatement(assignNode);
            //check condition
            CheckExpression(forNode.Condition);
            CheckImplicitCast(forNode.Condition, typeof(bool));

            //check step if his defined
            if (forNode.Step != null)
            {
                CheckExpression(forNode.Step);
                CheckImplicitCast(forNode.Step, typeof(decimal));   
            }

            //checkbody
            if (forNode.Body != null)
                CheckBody(forNode.Body);

        }

        private void CheckReturnStatement(ASTReturnStatementNode retNode)
        {
            if (retNode.Expression != null)
                CheckExpression(retNode.Expression);
        }



        //Some helpers

        /// <summary>
        /// Check expression for rvalue or lvalue
        /// </summary>
        /// <param name="expr">Expression for check</param>
        /// <returns>Expression is rvalue or not</returns>
        private bool IsRvalue(IASTExprNode expr)
        {
            if (expr is ASTVariableNode)
                return false;
            if (expr is ASTIndexOpNode)
                return false;
            if (expr is ASTCallStatementNode)
                return false;
            return true;
        }

        private void CheckImplicitCast(IASTExprNode expr, Type type)
        {
            var exprType = ASTExprHelper.GetExpressionType(expr);
            if (exprType != type)
            {
                var tmp = (ASTNode)expr;
                _messages.Add(new MessageRecord(
                    MsgCode.ImplicitCast,
                    tmp.SourcePath,
                    tmp.StartLine,
                    tmp.StartPos,
                    exprType,
                    type
                    ));
            }
        }

        private ASTCompileUnitNode _compileUnit;
        private SymbolTable _symbolTable;
        private UserSubroutine _currentSub;
        private IList<MessageRecord> _messages;
    }
}
