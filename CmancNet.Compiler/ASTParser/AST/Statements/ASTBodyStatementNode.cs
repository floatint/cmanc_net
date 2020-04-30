using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.Compiler.ASTParser.AST.Statements
{
    class ASTBodyStatementNode : ASTNode, IASTStatementNode
    {
        public IList<IASTStatementNode> Statements { private set; get; }

        public ASTBodyStatementNode(CmanParser.BodyStatementContext context, ASTNode parent) : base(parent)
        {
            SetLocation(context);
        }

        public override IList<ASTNode> Children => 
            Statements == null ? new List<ASTNode>() : Statements.Cast<ASTNode>().ToList();

        public void AddStatement(IASTStatementNode stmt)
        {
            if (Statements == null)
                Statements = new List<IASTStatementNode>();
            ((ASTNode)stmt).Parent = this;
            Statements.Insert(0, stmt);
        }
    }
}
