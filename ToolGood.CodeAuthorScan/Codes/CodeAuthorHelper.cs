using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ToolGood.CodeAuthorScan.Datas;
namespace ToolGood.CodeAuthorScan.Codes
{
    public class CodeAuthorHelper
    {
        public static void BuildCodeAuthor(CodeAuthorInfo codeAuthor, PdbFileInfo pdbFile, List<GitFileInfo> gitFileInfos)
        {
            var gitFiles = gitFileInfos.Where(q => q.File == pdbFile.File && q.Line >= pdbFile.LineStart && q.Line <= pdbFile.LineEnd);
            foreach (var file in gitFiles) {
                codeAuthor.AddAuthor(pdbFile.Namespace, pdbFile.Class, pdbFile.Method, file.Author, file.CommitTime);
            }
        }

        public static string GetOutFile(string file)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            var dir = Path.GetDirectoryName(file);
            return Path.Combine(dir, fileName + ".gca");
        }
    }
}
