using System;
using System.Collections.Generic;
using System.Text;
namespace ToolGood.CodeAuthorScan.Datas
{
    public class PdbFileInfo
    {
        public string File { get; set; }
        public string Namespace { get; set; }
        public string Class  { get; set; }
        public string Method { get; set; }
        public int LineStart { get; set; }
        public int LineEnd { get; set; }

        public string GetFullName()
        {
            return $"{File}.{Namespace}.{Class}.{Method}";
        }
    }
}
