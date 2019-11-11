using System;
using System.Collections.Generic;
using System.IO;
using eilang.Lexing;

namespace eilang.Imports
{
    public class ImportResolver
    {
        public IEnumerable<string> ResolveImportsFromFile(string pathToMain)
        {
            // step 1
            // read unique imports in code, add to list of new imports
            // recurse through all new imports and look for new unique imports
            // return an enumerable of unique imports
            var fileInfo = new FileInfo(pathToMain);
            var rootDirectory = fileInfo.DirectoryName;
            var fileName = fileInfo.Name;

            var check = new HashSet<string>();
            var mainPath = ResolveImportPath(fileName.EndsWith(".ei") ? fileName : $"{fileName}.ei",
                rootDirectory);
            var allImports = new HashSet<string> {mainPath};
            ResolveInner(mainPath);

            void ResolveInner(string path)
            {
                if (check.Contains(path))
                    return;
                check.Add(path);
                var code = GetCode(path);
                var reader = new ScriptReader(code);
                var lexer = new ImportLexer(reader, new CommonLexer(reader)); // maybe inject ImportResolver with a LexerFactory,
                // not really necessary at this point I feel
                var imports = lexer.GetImports();
                foreach (var import in imports)
                {
                    var canonicalized =
                        ResolveImportPath(import.EndsWith(".ei") ? import : $"{import}.ei", rootDirectory);
                    allImports.Add(canonicalized);
                    ResolveInner(canonicalized);
                }
            }

            return allImports;
        }

        private string GetCode(string path)
        {
            if(!File.Exists(path))
                throw new InvalidOperationException($"Could not find file to import: '{path}'");
            return File.ReadAllText(path); // TODO: could optimize this to only read until there are no more imports
        }

        private string ResolveImportPath(string import, string mainDirectory)
        {
            var fixedImport = import.Replace("\\", "/");
            var fixedMainDirectory = mainDirectory.Replace("\\", "/");
            return Path.Join(fixedMainDirectory, fixedImport);
            // TODO: if publishing eilang along with some standard library stuffs,
            // then check the executable folder if an identifier was supplied
            // i.e. "import std; # import standard library file" 
            // vs "import 'std'; # import file in local folder"
        }
    }
}