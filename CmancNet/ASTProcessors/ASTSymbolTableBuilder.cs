using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmancNet.ASTParser.AST.Statements;
using CmancNet.ASTParser.AST.Expressions;
using CmancNet.ASTParser.AST.Expressions.Unary;
using CmancNet.ASTInfo;

namespace CmancNet.ASTProcessors
{
    class ASTSymbolTableBuilder
    {
        /// <summary>
        /// Top level symbol table building method
        /// </summary>
        /// <param name="compileUnit">Compile unit AST node</param>
        /// <returns>Symbol table of subroutines</returns>
        public SymbolTable Build(ASTCompileUnitNode compileUnit)
        {
            SymbolTable st = new SymbolTable();
            st.ConnectNativeTable(new SystemEnvironment());
            if (compileUnit.Procedures != null)
            {
                foreach (var s in compileUnit.Procedures)
                {
                    var tmp = VisitSub(s);
                    if (tmp.Key != null)
                        st.AddSymbol(tmp.Key, tmp.Value);
                }
            }
            return st;
        }

        private KeyValuePair<string, ISymbol> VisitSub(ASTSubStatementNode subNode)
        {
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

        private KeyValuePair<string, ISymbol> VisitVariable(ASTVariableNode varNode)
        {
            return new KeyValuePair<string, ISymbol>(varNode.Name, new Variable());
        }

        private KeyValuePair<string, ISymbol> VisitForStatement(ASTForStatementNode forStmt)
        {
            if (forStmt.Counter is ASTVariableNode varNode)
                return new KeyValuePair<string, ISymbol>(varNode.Name, new Variable());
            if (forStmt.Counter is ASTAssignStatementNode assignStmt)
                return VisitAssignStatement(assignStmt);
            return new KeyValuePair<string, ISymbol>();
        }
    }
}
