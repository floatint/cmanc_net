using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace CmancNet.ASTParser.AST
{
    abstract class ASTNode
    {
        public ASTNode Parent { set; get; }

        public abstract IList<ASTNode> Children { get; }

        public int StartLine { set; get; }
        public int EndLine { set;  get; }
        public int StartPos { set;  get; }
        public int EndPos { set;  get; }

        public ASTNode(ASTNode parent)
        {
            Parent = parent;
        }

        public void SetLocation(ParserRuleContext context)
        {
            StartLine = context.Start.Line;
            StartPos = context.Start.Column + 1;
            EndLine = context.Stop.Line;
            EndPos = context.Stop.Column + 1;
        }

        public void SetLocation(int startLine, int endLine, int startPos, int endPos)
        {
            StartLine = startLine;
            EndLine = endLine;
            StartPos = startPos;
            EndPos = endPos;
        }

        public new virtual string ToString()
        {
            return GetType().Name;
        }

    }
}
