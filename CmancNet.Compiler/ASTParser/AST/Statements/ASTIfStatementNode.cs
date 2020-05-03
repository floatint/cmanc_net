using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmancNet.Compiler.ASTParser.AST.Expressions;

namespace CmancNet.Compiler.ASTParser.AST.Statements
{
    class ASTIfStatementNode : ASTNode, IASTStatementNode
    {
        public IASTExprNode Condition { set; get; }
        public ASTBodyStatementNode TrueBody { set; get; }
        public ASTBodyStatementNode ElseBody { set; get; }

        public override IList<ASTNode> Children
        {
            get
            {
                var list = new List<ASTNode>() { (ASTNode)Condition, TrueBody, ElseBody };
                list.RemoveAll(x => x == null);
                return list;
            }
        }

        public ASTIfStatementNode(CmanParser.IfStatementContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
        }
    }
}
