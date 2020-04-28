using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTInfo
{
    class SymbolTable
    {
        private Dictionary<string, ISymbol> _symbols;

        public SymbolTable()
        {
            _symbols = new Dictionary<string, ISymbol>();
        }

        public bool HasNativeEnvironment { private set; get; }

        public ISymbol FindSymbol(string name)
        {
            ISymbol ret;
            if (_symbols.TryGetValue(name, out ret))
                return ret;
            return null;
        }

        public void ConnectNativeTable(SystemEnvironment se)
        {
            JoinTable(se);
            HasNativeEnvironment = true;
        }


        //for register compiler functions
        public void JoinTable(SymbolTable st)
        {
            foreach (var c in st._symbols)
            {
                _symbols.Add(c.Key, c.Value);
            }
        }

        public virtual bool AddSymbol(string name, ISymbol symbol)
        {
            if (_symbols.ContainsKey(name))
                return false;
            _symbols.Add(name, symbol);
            return true;
        }
    }
}
