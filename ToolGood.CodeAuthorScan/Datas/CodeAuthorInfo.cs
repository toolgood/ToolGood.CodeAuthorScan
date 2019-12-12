using System;
using System.Collections.Generic;
using System.Text;

namespace ToolGood.CodeAuthorScan.Datas
{
    public class CodeAuthorInfo
    {
        public Dictionary<string, CodeNamespaceInfo> NamespaceInfos { get; set; }

        public class CodeNamespaceInfo
        {
            public Dictionary<string, CodeClassInfo> ClassInfos { get; set; }
        }
        public class CodeClassInfo
        {
            public Dictionary<string, CodeMethodInfo> MethodInfos { get; set; }
        }
        public class CodeMethodInfo
        {
            public List<CodeAuthor> Authors { get; set; }
         }
        public class CodeAuthor
        {
            public string Author { get; set; }

            public DateTime CommitTime { get; set; }
        }

    }










}
