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
                    var tmp = VisitSubStatement(s);
                    if (tmp.Key != null)
                        Symbols.AddSymbol(tmp.Key, tmp.Value);
                }
            }
            return ErrorsCount == 0;
        }

        private KeyValuePair<string, ISymbol> VisitSubStatement(ASTSubStatementNode subNode)
        {
            var existsSub = (ISubroutine)Symbols.FindSymbol(subNode.Name);
            if (existsSub != null)
            {
                if (existsSub is NativeSubroutine)
                    Messages.Add(new MessageRecord(MsgCode.NativeSubOverride, subNode.SourcePath, subNode.StartLine, subNode.StartPos, subNode.Name));
                else
                    Messages.Add(new MessageRecord(MsgCode.UserSubOverride, subNode.SourcePath, subNode.StartLine, subNode.StartPos, subNode.Name));
                return new KeyValuePair<string, ISymbol>();
            }
            UserSubroutine sub = new UserSubroutine(subNode);
            if (subNode.ArgList != null)
            {
                foreach(var a in subNode.ArgList.Arguments)
                {
                    sub.AddLocal(a.Name, new Argument());
                }
            }
            if (subNode.Body != null)
            {
                var retStmt = (ASTReturnStatementNode)subNode.Body.Statements.FirstOrDefault(x => x is ASTReturnStatementNode);
                if ((retStmt != null) && (retStmt.Expression != null))
                    sub.Return = true;
                foreach (var s in subNode.Body.Statements)
                {
                    var tmp = VisitStatement(s);
                    if (tmp.Key != null)
                        sub.AddLocal(tmp.Key, tmp.Value);
                }
            }
            return new KeyValuePair<string, ISymbol>(subNode.Name, sub);
        }

        private KeyValuePair<string, ISymbol> VisitStatement(IASTStatementNode stmtNode)
        {
            if (stmtNode is ASTAssignStatementNode asgNode)
                return VisitAssignStatement(asgNode);
            if (stmtNode is ASTForStatementNode forStmt)
                return VisitForStatement(forStmt);
            return new KeyValuePair<string, ISymbol>();
        }

        private KeyValuePair<string, ISymbol> VisitAssignStatement(ASTAssignStatementNode assignNode)
        {
            if (assignNode.Left is ASTVariableNode varNode)
                return new KeyValuePair<string, ISymbol>(varNode.Name, new Variable());
            return new KeyValuePair<string, ISymbol>();
        }


        private KeyValuePair<string, ISymbol> VisitForStatement(ASTForStatementNode forStmt)
        {
            if (forStmt.Counter is ASTVariableNode varNode)
                return new KeyValuePair<string, ISymbol>(varNode.Name, new Variable());
            if (forStmt.Counter is ASTAssignStatementNode assignStmt)
                return VisitAssignStatement(assignStmt);
            return new KeyValuePair<string, ISymbol>();
        }

        private ASTCompileUnitNode _compileUnit;
    }
}
