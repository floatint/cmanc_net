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
        public string SourcePath { set; get; }

        public ASTNode(ASTNode parent)
        {
            Parent = parent;
        }

        public void SetLocation(ParserRuleContext context)
        {
            StartLine = context.Start.Line;
            StartPos = context.Start.Column + 1;
            //if no have stop token. Start == Stop
            if (context.Stop != null)
            {
                EndLine = context.Stop.Line;
                EndPos = context.Stop.Column + 1;
            }
            else
            {
                EndLine = StartLine;
                EndPos = StartPos;
            }
            SourcePath = context.Start.InputStream.SourceName;
        }

        public new virtual string ToString()
        {
            return string.Format("{0}({1},{2})", GetType().Name, StartLine, StartPos);//GetType().Name;
        }

    }
}
