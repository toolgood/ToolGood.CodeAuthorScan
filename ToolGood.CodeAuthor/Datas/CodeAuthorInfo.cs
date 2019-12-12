using System;
using System.Collections.Generic;
using System.Text;

namespace ToolGood.CodeAuthor.Datas
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



        public List<CodeAuthor> GetCodeAuthor(string namespaceName,string className,string methodName)
        {
            if (NamespaceInfos.TryGetValue(namespaceName,out CodeNamespaceInfo codeNamespaceInfo)) {
                if (codeNamespaceInfo.ClassInfos.TryGetValue(className,out CodeClassInfo codeClassInfo)) {
                    if (codeClassInfo.MethodInfos.TryGetValue(methodName,out CodeMethodInfo codeMethodInfo)) {
                        return codeMethodInfo.Authors;
                    }
                }
            }
            return new List<CodeAuthor>();
        }
    }

}
