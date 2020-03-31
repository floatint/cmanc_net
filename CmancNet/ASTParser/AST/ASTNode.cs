using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST
{
    class ASTNode
    {
        public ASTNode Parent { set; get; }

        public int StartLine { set; get; }
        public int EndLine { set;  get; }
        public int StartPos { set;  get; }
        public int EndPos { set;  get; }

        public ASTNode(ASTNode parent)
        {
            Parent = parent;
        }

        public void SetLocation(int startLine, int endLine, int startPos, int endPos)
        {
            StartLine = startLine;
            EndLine = endLine;
            StartPos = startPos;
            EndPos = endPos;
        }

    }
}
