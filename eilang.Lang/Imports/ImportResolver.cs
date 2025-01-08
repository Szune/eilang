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
                var reader = new ScriptReader(code, path);
                var lexer = new ImportLexer(reader,
                    new CommonLexer(reader)); // maybe inject ImportResolver with a LexerFactory,
                // not really necessary at this point I feel
                var imports = lexer.GetImports();
                foreach (var import in imports)
                {
                    var canonicalized =
                        ResolveImportPath(import.EndsWith(".ei") ? import : $"{import}.ei", path);
                    allImports.Add(canonicalized);
                    ResolveInner(canonicalized);
                }
            }

            return allImports;
        }

        private string GetCode(string path)
        {
            if (!File.Exists(path))
                throw new InvalidOperationException($"Could not find file to import: '{path}'");
            return File.ReadAllText(path); // TODO: could optimize this to only read until there are no more imports
        }

        private string ResolveImportPath(string import, string mainDirectory)
        {
            var fixedImport = import.Replace("\\", "/");
            if (!fixedImport.StartsWith("/") && fixedImport.Contains("/"))
            {
                fixedImport = "/" + fixedImport;
            }

            var fixedMainDirectory = mainDirectory.Replace("\\", "/");
            if (fixedMainDirectory.EndsWith(".ei"))
            {
                // TODO: could use some improvement to check if it's a folder instead of checking what the path ends with
                fixedMainDirectory = fixedMainDirectory.Substring(0, fixedMainDirectory.LastIndexOf('/'));
            }

            var absolutePath = import.Contains(":") 
                ? import 
                : Path.Join(fixedMainDirectory, fixedImport).Replace("\\", "/");
#if DEBUG
            Console.WriteLine($"Importing {import} from {mainDirectory}");
            Console.WriteLine($"Resulting absolute path: {absolutePath}");
#endif
            return absolutePath;
        }
    }
}