using Microsoft.DiaSymReader.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using ToolGood.CodeAuthorScan.Datas;

namespace ToolGood.CodeAuthorScan.Codes
{
    public class PdbFileHelper
    {
        public static List<string> GetPdbFiles(string folderPath)
        {
            Console.WriteLine($"获取{folderPath}目录下所有pdb文件。" );
            return Directory.GetFiles(folderPath, "*.pdb").ToList();
        }
        public static List<PdbFileInfo> GetPdbInfos(string file)
        {
            List<PdbFileInfo> infos = new List<PdbFileInfo>();
            var dllFile = Path.ChangeExtension(file, ".dll");
            var exeFile = Path.ChangeExtension(file, ".exe");
            if (File.Exists(dllFile)) {
                Console.WriteLine("读取pdb文件："+ file);
                var xml = GenXmlFromPdb(dllFile, file);
                GetPdbInfo(infos, xml);
            } else if (File.Exists(exeFile)) {
                Console.WriteLine("读取pdb文件：" + file);
                var xml = GenXmlFromPdb(exeFile, file);
                GetPdbInfo(infos, xml);
            }
            return infos;
        }


        public static string GenXmlFromPdb(string exePath, string pdbPath)
        {
            using (var peStream = new FileStream(exePath, FileMode.Open, FileAccess.Read))
            using (var pdbStream = new FileStream(pdbPath, FileMode.Open, FileAccess.Read))
            using (var ms = new MemoryStream())
            using (var sw = new StreamWriter(ms, Encoding.UTF8)) {
                PdbToXmlConverter.ToXml(sw, pdbStream, peStream, PdbToXmlOptions.ResolveTokens | PdbToXmlOptions.IncludeTokens);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
        private static void GetPdbInfo(List<PdbFileInfo> infos, string xml)
        {
            XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.LoadXml(xml.Replace("﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>", ""));

            var fileNodes = xmlDocument.SelectNodes("/symbols/files/file");
            Dictionary<string, string> srcFiles = new Dictionary<string, string>();
            foreach (XmlNode node in fileNodes) {
                var key = node.Attributes["id"].Value;
                var value = node.Attributes["name"].Value;
                srcFiles[key] = value;
            }
            var methodNodes = xmlDocument.SelectNodes("/symbols/methods/method");
            foreach (XmlNode node in methodNodes) {
                var cname = node.Attributes["containingType"].Value;
                var @namespace = cname.Substring(0, cname.LastIndexOf('.'));
                var className = cname.Substring(cname.LastIndexOf('.') + 1);
                var mname = node.Attributes["name"].Value;

                var entrys = node.SelectNodes("sequencePoints/entry");
                int startLine = int.MaxValue;
                int endLine = 0;
                var file = "";
                foreach (XmlNode entry in entrys) {
                    bool isHidden = false;
                    int sl = 0, el = 0;
                    foreach (XmlAttribute att in entry.Attributes) {
                        if (att.Name == "hidden") {
                            isHidden = true;
                        } else if (att.Name == "startLine") {
                            sl = int.Parse(att.Value);
                        } else if (att.Name == "endLine") {
                            el = int.Parse(att.Value);
                        } else if (att.Name == "document") {
                            var fileId = entry.Attributes["document"].Value;
                            file = srcFiles[fileId];
                        }
                    }
                    if (isHidden) { continue; }
                    if (startLine > sl) { startLine = sl; }
                    if (endLine < el) { endLine = el; }
                }
                if (endLine > 0) {
                    var info = new PdbFileInfo() {
                        Namespace = @namespace,
                        Class = className,
                        Method = mname,
                        File = file,
                        LineStart = startLine,
                        LineEnd = endLine
                    };
                    infos.Add(info);
                }
            }
        }
        public static List<string> GetFiles(List<PdbFileInfo> infos)
        {
            return infos.Select(q => q.File).Distinct().ToList();
        }

    }
}
