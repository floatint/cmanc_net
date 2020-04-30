using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace CmancNet.Driver
{
    sealed class Options
    {
        [Option('c', "compile", Required = true, HelpText = "compile file")]
        public string SourceFileName { set; get; }

        [Option('o', "out", Required = false, HelpText = "save compiled file with specific name")]
        public string OutputFileName { set; get; }
    }
}
