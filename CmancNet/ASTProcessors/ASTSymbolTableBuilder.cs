using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmancNet.ASTParser.AST.Statements;
using CmancNet.ASTInfo;

namespace CmancNet.ASTProcessors
{
    class ASTSymbolTableBuilder
    {
        public SymbolTable BuildSymbolTable(ASTCompileUnitNode compileUnit)
        {
            _symbolTable = new SymbolTable();
            _symbolTable.ConnectNativeTable(new SystemEnvironment()); //connect sys methods
            EnterCompileUnit(compileUnit);
            return _symbolTable;
        }

        private void EnterCompileUnit(ASTCompileUnitNode compileUnit)
        {
            foreach (var s in compileUnit.Procedures)
            {
                EnterSub(s);
            }
        }

        private void EnterSub(ASTSubStatementNode sub)
        {
            _symbolTable.AddSymbol(
                sub.Name,
                new UserSubroutine(sub)
                );
            
        }

        private void EnterStatement(IASTStatementNode stmt)
        { }

        private SymbolTable _symbolTable;
    }
}
