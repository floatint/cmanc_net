using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST.Statements
{
    class ASTSubStatementNode : ASTNode, IASTStatementNode
    {
        private ASTBodyStatementNode _body;
        private ASTArgListNode _args;

        public string Name { set; get; }

        public ASTBodyStatementNode Body
        {
            set
            {
                value.Parent = this;
                _body = value;
            }
            get => _body;
        }

        public ASTArgListNode Arguments
        {
            set
            {
                value.Parent = this;
                _args = value;
            }
            get => _args;
        }


        public ASTSubStatementNode(CmanParser.SubStatementContext context, ASTNode parent)
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
            return string.Format("{0}: [{1}]", base.ToString(), Name);
        }
    }
}
