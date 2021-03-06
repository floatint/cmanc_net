﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.Compiler.ASTParser.AST.Expressions.Unary
{
    class ASTNotOpNode : ASTNode, IASTUnarOpNode, IASTLogicOpNode
    {
        public IASTExprNode Expression { set; get; }

        public override IList<ASTNode> Children => new List<ASTNode> { (ASTNode)Expression };

        public ASTNotOpNode(CmanParser.UnarOpContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
        }
    }
}
