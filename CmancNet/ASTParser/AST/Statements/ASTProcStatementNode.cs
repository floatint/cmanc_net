using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST.Statements
{
    class ASTProcStatementNode : ASTNode, IASTStatementNode
    {
        public string Name { set; get; }
        public ASTBodyStatementNode Body { set; get; }
        public ASTArgListNode Arguments { set; get; }


        public ASTProcStatementNode(CmanParser.ProcStatementContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
            Name = context.children.First(x => x is CmanParser.NameContext).GetText();
        }

        public override IList<ASTNode> Children
        {
            get
            {
                var list = new List<ASTNode> { Arguments, Body };
                list.RemoveAll(x => x is null);
                return list;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", GetType().Name, Name);
        }
    }
}
