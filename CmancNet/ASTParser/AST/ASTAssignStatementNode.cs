using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST
{
    /*
        По поводу работы с массивами:
        
        Это можно офрмить в виде унарного оператора

        expr [varOrExpr]

        Дальше смотреть что лежит в expr.
    */
    class ASTAssignStatementNode : ASTStatementNode
    {
        //may be expression ?
        public ASTStatementNode Left { set; get; }
        public ASTStatementNode Right { set; get; }

        public ASTAssignStatementNode(CmanParser.AssignStatementContext context, ASTNode parent)
            : base(parent)
        {

        }
    }
}
