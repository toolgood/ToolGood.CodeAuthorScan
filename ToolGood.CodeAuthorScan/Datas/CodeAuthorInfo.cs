using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace ToolGood.CodeAuthorScan.Datas
{
    public class CodeAuthorInfo
    {
        public Dictionary<string, CodeNamespaceInfo> NamespaceInfos { get; set; } = new Dictionary<string, CodeNamespaceInfo>();
        public class CodeNamespaceInfo
        {
            public Dictionary<string, CodeClassInfo> ClassInfos { get; set; } = new Dictionary<string, CodeClassInfo>();
        }
        public class CodeClassInfo
        {
            public Dictionary<string, CodeMethodInfo> MethodInfos { get; set; } = new Dictionary<string, CodeMethodInfo>();
        }
        public class CodeMethodInfo
        {
            public List<CodeAuthor> Authors { get; set; } = new List<CodeAuthor>();
        }
        public class CodeAuthor
        {
            public string Author { get; set; }
            public DateTime CommitTime { get; set; }
        }

        public void AddAuthor(string namespaceName, string className, string methodName, string author, DateTime commitTime)
        {
            CodeNamespaceInfo codeNamespaceInfo;
            CodeClassInfo codeClassInfo;
            CodeMethodInfo codeMethodInfo;
            if (NamespaceInfos.TryGetValue(namespaceName, out codeNamespaceInfo) == false) {
                codeNamespaceInfo = new CodeNamespaceInfo();
                NamespaceInfos[namespaceName] = codeNamespaceInfo;
            }
            if (codeNamespaceInfo.ClassInfos.TryGetValue(className, out codeClassInfo) == false) {
                codeClassInfo = new CodeClassInfo();
                codeNamespaceInfo.ClassInfos[className] = codeClassInfo;
            }
            if (codeClassInfo.MethodInfos.TryGetValue(methodName, out codeMethodInfo) == false) {
                codeMethodInfo = new CodeMethodInfo();
                codeClassInfo.MethodInfos[className] = codeMethodInfo;
            }

            CodeAuthor codeAuthor = codeMethodInfo.Authors.Where(q => q.Author == author && q.CommitTime == commitTime).FirstOrDefault();
            if (codeAuthor==null) {
                codeAuthor = new CodeAuthor() {
                    Author = author,
                    CommitTime = commitTime
                };
                codeMethodInfo.Authors.Add(codeAuthor);
            }
        }
    }





}
