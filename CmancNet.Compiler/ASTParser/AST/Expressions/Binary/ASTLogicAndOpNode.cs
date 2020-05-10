using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.Compiler.ASTParser.AST.Expressions.Binary
{
    class ASTLogicAndOpNode : ASTAbstractBinOpNode, IASTLogicOpNode
    {
        public ASTLogicAndOpNode(CmanParser.LogicAndContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
        }
    }
}
