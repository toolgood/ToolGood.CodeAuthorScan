using System;
using System.Collections.Generic;
using System.Text;
namespace ToolGood.CodeAuthorScan.Datas
{
    public class GitFileInfo
    {
        public string File { get; set; }
        public int Line { get; set; }
        public string Author { get; set; }
        public DateTime CommitTime { get; set; }
    }
}
