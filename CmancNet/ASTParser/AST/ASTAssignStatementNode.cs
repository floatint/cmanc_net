﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST
{
    class ASTAssignStatementNode : ASTStatementNode
    {
        public ASTExpressionNode Left { set; get; }
        public ASTExpressionNode Right { set; get; }

        public override IList<ASTNode> Children => new List<ASTNode> { Left, Right };

        public ASTAssignStatementNode(CmanParser.AssignStatementContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
        }

    }
}
