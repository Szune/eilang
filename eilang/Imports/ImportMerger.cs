using System.Collections.Generic;
using System.IO;
using System.Text;

namespace eilang.Imports
{
    public class ImportMerger
    {
        public string Merge(IEnumerable<string> importPaths)
        {
            // step 2
            // merge the code in such a way that it can be parsed and interpreted (what a novel idea..)
            
            // TODO: handle imports that cannot be read directly (maybe external libraries in the form of dlls or w/e)
            var sb = new StringBuilder();
            foreach(var import in importPaths)
            {
                var source = File.ReadAllText(import);
                var importLexer = new ImportLexer(source);
                importLexer.SkipImports();
                var withoutImports = source.Substring(importLexer.AbsolutePosition);
                sb.AppendLine(withoutImports);
            }
            
            return sb.ToString();
        }
    }
}