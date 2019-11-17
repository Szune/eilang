using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using eilang.Lexing;

namespace eilang.Imports
{
    public class ImportMerger
    {
        public List<ImportedScript> Merge(IEnumerable<string> importPaths)
        {
            if(!importPaths.Any())
                throw new InvalidOperationException("ImportMerger requires at least one path to import.");
            // step 2
            // merge the code in such a way that it can be parsed and interpreted (what a novel idea..)
            
            // TODO: handle imports that cannot be read directly (maybe external libraries in the form of dlls or w/e)
            var importedScripts = new List<ImportedScript>();
            foreach(var importPath in importPaths)
            {
                var source = File.ReadAllText(importPath);
                var reader = new ScriptReader(source);
                var importLexer = new ImportLexer(reader, new CommonLexer(reader));
                var afterImportsPosition = importLexer.GetIndexAfterImports();
                var codeWithoutImports = source.Substring(afterImportsPosition);
                var imported = new ImportedScript(importPath, codeWithoutImports, reader.Line);
                importedScripts .Add(imported);
            }

            return importedScripts;
        }
    }
}