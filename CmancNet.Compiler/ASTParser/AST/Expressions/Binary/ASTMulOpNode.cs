﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.Compiler.ASTParser.AST.Expressions.Binary
{
    class ASTMulOpNode : ASTAbstractBinOpNode
    {
        public ASTMulOpNode(CmanParser.MulOrDivOpContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
        }
    }
}