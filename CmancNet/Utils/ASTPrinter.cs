using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmancNet.ASTParser.AST;

namespace CmancNet.Utils
{
    class ASTPrinter
    {
        public static string Print(ASTNode node)
        {
            return PrintNode(node, 0, "\t");
        }

        public static string Print(ASTNode node, string space)
        {
            return PrintNode(node, 0, space);
        }

        private static string PrintNode(ASTNode node, int depth, string space)
        {
            if (node == null)
                return null;
            string result = "";
            for (int i = 0; i < depth; i++)
                result += space;
            result += node.ToString() + "\n";
            foreach (var c in node.Children)
                result += PrintNode(c, depth + 1, space);
            return result;

        }
    }
}
