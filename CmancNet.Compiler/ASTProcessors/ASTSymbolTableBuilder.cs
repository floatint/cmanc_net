using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmancNet.Compiler.ASTParser.AST.Statements;
using CmancNet.Compiler.ASTParser.AST.Expressions;
using CmancNet.Compiler.ASTParser.AST.Expressions.Unary;
using CmancNet.Compiler.ASTInfo;
using CmancNet.Compiler.Utils.Logging;

namespace CmancNet.Compiler.ASTProcessors
{

    class ASTSymbolTableBuilder
    {
        public IList<MessageRecord> Messages { private set; get; }
        public int ErrorsCount => Messages.Where(x => x.Message.Type == MsgType.Error).Count();
        public SymbolTable Symbols { private set; get; }

        public ASTSymbolTableBuilder(ASTCompileUnitNode compileUnit)
        {
            _compileUnit = compileUnit;
            Messages = new List<MessageRecord>();
        }
        /// <summary>
        /// Top level symbol table building method
        /// </summary>
        /// <returns>Symbol table build status</returns>
        public bool Build()
        {
            Symbols = new SymbolTable();
            //connect native subs
            Symbols.ConnectNativeTable(new SystemEnvironment());
            if (_compileUnit.Procedures != null)
            {
                foreach (var s in _compileUnit.Procedures)
                {
                    VisitSubStatement(s);
                }
            }
            return ErrorsCount == 0;
        }

        private void VisitSubStatement(ASTSubStatementNode subNode)
        {
            //check overriding
            var existsSub = (ISubroutine)Symbols.FindSymbol(subNode.Name);
            if (existsSub != null)
            {
                if (existsSub is NativeSubroutine)
                    Messages.Add(new MessageRecord(MsgCode.NativeSubOverride, subNode.SourcePath, subNode.StartLine, subNode.StartPos, subNode.Name));
                else
                    Messages.Add(new MessageRecord(MsgCode.UserSubOverride, subNode.SourcePath, subNode.StartLine, subNode.StartPos, subNode.Name));
            }
            else
            {
                UserSubroutine sub = new UserSubroutine(subNode);
                //add arguments
                if (subNode.ArgList != null)
                {
                    foreach (var a in subNode.ArgList.Arguments)
                    {
                        sub.AddLocal(a.Name, new Argument());
                    }
                }
                _currentSubroutine = sub;
                //body check
                if (subNode.Body != null)
                {
                    VisitBodyStatement(subNode.Body);
                }
                Symbols.AddSymbol(subNode.Name, _currentSubroutine);
            }
        }

        private void VisitBodyStatement(ASTBodyStatementNode bodyNode)
        {
            foreach (var s in bodyNode.Statements)
            {
                VisitStatement(s);
            }
        }

        private void VisitStatement(IASTStatementNode stmtNode)
        {
            if (stmtNode is ASTAssignStatementNode assignNode)
                VisitAssignStatement(assignNode);
            if (stmtNode is ASTForStatementNode forNode)
                VisitForStatement(forNode);
            if (stmtNode is ASTWhileStatementNode whileNode)
                VisitWhileStatement(whileNode);
            if (stmtNode is ASTIfStatementNode ifNode)
                VisitIfStatement(ifNode);
            if (stmtNode is ASTReturnStatementNode retNode)
                VisitReturnStatement(retNode);
        }

        private void VisitVariable(ASTVariableNode varNode)
        {
            var existsLocal = _currentSubroutine.FindLocal(varNode.Name);
            if (existsLocal == null)
            {
                _currentSubroutine.AddLocal(varNode.Name, new Variable());
            }
        }

        private void VisitAssignStatement(ASTAssignStatementNode assignNode)
        {
            if (assignNode.Left is ASTVariableNode varNode)
                VisitVariable(varNode);
        }


        private void VisitForStatement(ASTForStatementNode forStmt)
        {
            if (forStmt.Counter is ASTVariableNode varNode)
                VisitVariable(varNode);
               
            if (forStmt.Counter is ASTAssignStatementNode assignStmt)
                VisitAssignStatement(assignStmt);

            if (forStmt.Body != null)
            {
                VisitBodyStatement(forStmt.Body);
            }
        }

        private void VisitWhileStatement(ASTWhileStatementNode whileNode)
        {
            if (whileNode.Body != null)
                VisitBodyStatement(whileNode.Body);
        }

        private void VisitIfStatement(ASTIfStatementNode ifNode)
        {
            if (ifNode.TrueBody != null)
                VisitBodyStatement(ifNode.TrueBody);
            if (ifNode.ElseBody != null)
                VisitBodyStatement(ifNode.ElseBody);
        }

        private void VisitReturnStatement(ASTReturnStatementNode retNode)
        {
            //first ret statement
            if (_hasRet == null)
                if (retNode.Expression != null)
                {
                    _hasRet = true;
                    _currentSubroutine.Return = true;                    
                }
                else
                    _hasRet = false;
            else //after first
            {
                //already return value
                if ((bool)_hasRet)
                {
                    if (retNode.Expression == null)
                    {
                        Messages.Add(new MessageRecord(
                            MsgCode.AmbiguousReturn,
                            retNode.SourcePath,
                            retNode.StartLine,
                            retNode.StartPos,
                            "value",
                            "void"
                            ));
                    }
                }
                else //no value return
                {
                    if (retNode.Expression != null)
                    {
                        Messages.Add(new MessageRecord(
                            MsgCode.AmbiguousReturn,
                            retNode.SourcePath,
                            retNode.StartLine,
                            retNode.StartPos,
                            "void",
                            "value"
                            ));
                    }
                }
            }   
        }

        private object _hasRet; //helper for return validation
        private ASTCompileUnitNode _compileUnit;
        private UserSubroutine _currentSubroutine;
    }
}
