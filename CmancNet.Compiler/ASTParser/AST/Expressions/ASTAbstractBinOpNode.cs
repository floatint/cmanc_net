using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.Compiler.ASTParser.AST.Expressions
{
    
    /// <summary>
    /// Basic implementation for IASTBinOpNode
    /// </summary>
    abstract class ASTAbstractBinOpNode : ASTNode, IASTBinOpNode
    {
        protected IASTExprNode _left;
        protected IASTExprNode _right;

        protected ASTAbstractBinOpNode(ASTNode parent)
            : base(parent)
        { }

        public virtual IASTExprNode Left
        {
            set
            {
                ((ASTNode)value).Parent = this;
                _left = value;
            }
            get
            {
                return _left;
            }
        }

        public virtual IASTExprNode Right
        {
            set
            {
                ((ASTNode)value).Parent = this;
                _right = value;
            }
            get
            {
                return _right;
            }
        }

        public override IList<ASTNode> Children 
            => new List<ASTNode> { (ASTNode)Left, (ASTNode)Right};
    }
}
