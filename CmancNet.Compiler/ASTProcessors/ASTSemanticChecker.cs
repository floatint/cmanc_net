using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmancNet.Compiler.ASTParser.AST;
using CmancNet.Compiler.ASTParser.AST.Statements;
using CmancNet.Compiler.ASTParser.AST.Expressions;
using CmancNet.Compiler.ASTParser.AST.Expressions.Unary;
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
            //Errors = new List<string>();
            //Warnings = new List<string>();
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
                //Messages.Add(PackMessage(MsgCode.EmptyBody, subNode, null));
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
            if (stmtNode is ASTWhileStatementNode whileNode)
                CheckWhileStatement(whileNode);
            if (stmtNode is ASTForStatementNode forNode)
                CheckForStatement(forNode);
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
                //Messages.Add(PackMessage(MsgCode.RvalueAssign, assignNode, null));
                //Errors.Add(MessageFormatter.Format((ASTNode)assignNode.Left, MsgCode.RvalueAssign, null));
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
                //Messages.Add(PackMessage(MsgCode.UndefinedVariable, varNode, varNode.Name));
                //Errors.Add(MessageFormatter.Format(varNode, MsgCode.UndefinedVariable, varNode.Name));
            }
        }

        private void CheckBinOp(IASTBinOpNode binOpNode)
        {
            CheckExpression(binOpNode.Left);
            CheckExpression(binOpNode.Right);
        }

        private void CheckUnarOp(IASTUnarOpNode unarOpNode)
        {
            //CheckExpression(unarOpNode.Expression);
            if (unarOpNode is ASTIndexOpNode indexOp)
                CheckIndexOp(indexOp);
            else
                CheckExpression(unarOpNode.Expression);
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
                //Messages.Add(PackMessage(MsgCode.RvalueIndexing, indexOpNode, null));
                //Errors.Add(MessageFormatter.Format(indexOpNode, MsgCode.RvalueIndexing, null));
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
            CheckExpression(indexOpNode.Index);
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
                //Messages.Add(PackMessage(MsgCode.UndefinedSub, callNode, callNode.ProcedureName));
                //Errors.Add(MessageFormatter.Format(callNode, MsgCode.UndefinedSub, callNode.ProcedureName));
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
                        //Messages.Add(PackMessage(MsgCode.ReturnNotFound, callNode, callNode.ProcedureName));
                        //Errors.Add(MessageFormatter.Format(callNode, MsgCode.ReturnNotFound, callNode.ProcedureName));
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
                        //Messages.Add(PackMessage(MsgCode.TooFewArguments, callNode, argCnt, callNode.Arguments.Children.Count));
                        //Errors.Add(MessageFormatter.Format(callNode, MsgCode.TooFewArguments, argCnt, callNode.Arguments.Children.Count));
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
                        //Messages.Add(PackMessage(MsgCode.TooManyArguments, callNode, argCnt, callNode.Arguments.Children.Count));
                        //string msg = "too many arguments, {0} required, but {1} found";
                        //Errors.Add(MessageFormatter.Format(callNode, MsgCode.TooManyArguments, argCnt, callNode.Arguments.Children.Count));
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
                        //Messages.Add(PackMessage(MsgCode.TooFewArguments, callNode, argCnt, 0));
                        //string msg = "too few arguments, {0} required, but {1} found";
                        //Errors.Add(MessageFormatter.Format(callNode, MsgCode.TooFewArguments, argCnt, 0));
                    }
                }
            }
        }

        private void CheckWhileStatement(ASTWhileStatementNode whileNode)
        {
            if (whileNode.Body != null)
            {
                CheckBody(whileNode.Body);
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
            /*if (!(forNode.Condition is IASTCmpOpNode) && !(forNode.Condition is ASTNotOpNode))
            {
                string msg = "for condition statement requires only boolean operations";
                _errors.Add(ErrorFormatter.Format((ASTNode)forNode.Condition, string.Format(msg)));
                _error = true;
            }
            else
            {
                CheckExpression(forNode.Condition);
            }*/
            CheckExpression(forNode.Condition);
            //check step if his defined
            if (forNode.Step != null)
            {
                CheckExpression(forNode.Step);
            }

            //checkbody
            if (forNode.Body != null)
                CheckBody(forNode.Body);

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

        /// <summary>
        /// Pack message into tuple for top level viewers (json, console, etc)
        /// </summary>
        /// <param name="code">Message code</param>
        /// <param name="node">AST node for message location</param>
        /// <param name="args">Message arguments</param>
        /// <returns>Packed tuple message</returns>
        private Tuple<MsgCode, Message, ASTNode, object[]> PackMessage(MsgCode code, ASTNode node, params object[] args)
        {
            return new Tuple<MsgCode, Message, ASTNode, object[]>(code, MessageTable.Get()[code], node, args);
        }


        private ASTCompileUnitNode _compileUnit;
        private SymbolTable _symbolTable;
        private UserSubroutine _currentSub;

        private IList<MessageRecord> _messages;
    }
}
