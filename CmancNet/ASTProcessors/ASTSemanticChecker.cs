using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmancNet.ASTParser.AST;
using CmancNet.ASTParser.AST.Statements;
using CmancNet.ASTParser.AST.Expressions;
using CmancNet.ASTParser.AST.Expressions.Unary;
using CmancNet.ASTInfo;
using CmancNet.Utils;

namespace CmancNet.ASTProcessors
{
    class ASTSemanticChecker
    {
        public ASTSemanticChecker(ASTCompileUnitNode compileUnit, SymbolTable symbolTable)
        {
            _compileUnit = compileUnit;
            _symbolTable = symbolTable;
            _errors = new List<string>();
        }
        public IList<string> Errors => _errors;

        public bool IsValid()
        {
            foreach (var s in _compileUnit.Procedures)
            {
                CheckSubroutine(s);
            }
            return !_error;
        }

        private void CheckSubroutine(ASTSubStatementNode subNode)
        {
            _currentSub = (UserSubroutine)_symbolTable.FindSymbol(subNode.Name);
            if (subNode.Body != null)
            {
                foreach (var s in subNode.Body.Statements)
                {
                    CheckStatement(s);
                }
            }
        }

        private void CheckStatement(IASTStatementNode stmtNode)
        {
            if (stmtNode is ASTAssignStatementNode assignNode)
                CheckAssignStatement(assignNode);
            if (stmtNode is ASTCallStatementNode callNode)
                CheckSubCall(callNode, false);
            if (stmtNode is ASTWhileStatementNode whileNode)
                CheckWhileStatement(whileNode);
        }

        private void CheckAssignStatement(ASTAssignStatementNode assignNode)
        {
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
                string msg = "\'${0}\' undefined variable";
                _errors.Add(ErrorFormatter.Format(varNode, string.Format(msg, varNode.Name)));
                _error = true;
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
                string msg = "index statement requires lvalue, but rvalue found";
                _errors.Add(ErrorFormatter.Format(indexOpNode, string.Format(msg)));
                _error = true;
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
                string msg = "\'{0}\' subroutine not defined";
                _errors.Add(ErrorFormatter.Format(callNode, string.Format(msg, callNode.ProcedureName)));
                _error = true;
            }
            else
            {
                //check return value
                if (requireReturn)
                {
                    if (!sub.Return)
                    {
                        string msg = "statement requires a return value, but the \'{0}\' returns void";
                        _errors.Add(ErrorFormatter.Format(callNode, string.Format(msg, callNode.ProcedureName)));
                        _error = true;
                    }
                }

                //check arguments count
                int argCnt = sub.ArgumentsCount;
                if (callNode.Arguments != null)
                {
                    if (argCnt > callNode.Arguments.Children.Count)
                    {
                        string msg = "too few arguments, {0} required, but {1} found";
                        _errors.Add(ErrorFormatter.Format(callNode, string.Format(msg, argCnt, callNode.Arguments.Children.Count)));
                        _error = true;
                    }
                    if (argCnt < callNode.Arguments.Children.Count)
                    {
                        string msg = "too many arguments, {0} required, but {1} found";
                        _errors.Add(ErrorFormatter.Format(callNode, string.Format(msg, argCnt, callNode.Arguments.Children.Count)));
                        _error = true;
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
                        string msg = "too few arguments, {0} required, but {1} found";
                        _errors.Add(ErrorFormatter.Format(callNode, string.Format(msg, argCnt, 0)));
                        _error = true;
                    }
                }
            }
        }

        private void CheckWhileStatement(ASTWhileStatementNode whileNode)
        {

        }



        private IList<string> _errors;
        private bool _error;
        private ASTCompileUnitNode _compileUnit;
        private SymbolTable _symbolTable;
        private UserSubroutine _currentSub;
    }
}
