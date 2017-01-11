﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace EncodingNormalior.Model
{
    ///// <summary>
    ///// 后缀
    ///// </summary>
    //public class Suffix
    //{
    //    public Suffix()
    //    {

    //    }

    //    /// <summary>
    //    /// 图片后缀
    //    /// </summary>
    //    public List<string> ImageSuffix { set; get; } = new List<string>()
    //    {
    //        "*.bmp","*.jpg","*.gif","*.png","*.dxf","*.cdr"
    //    };

    //    /// <summary>
    //    /// 压缩文件后缀
    //    /// </summary>
    //    public List<string> CompressFileSuffix { set; get; }=new List<string>()
    //    {
    //        "*.zip","*.rar","*.7z",
    //        //"*.",
    //    };

    //}

    /// <summary>
    ///     文件检测白名单设置
    /// </summary>
    public class InspectFileWhiteListSetting : ISetting
    {
        static InspectFileWhiteListSetting()
        {
            DefaultWhiteList = new List<string>();
            var file = "WhiteList.txt";
            string[] whiteList;
            if (File.Exists(file))
            {
                using (StreamReader stream=new StreamReader(
                    new FileStream(file,FileMode.Open)))
                {
                    whiteList = stream.ReadToEnd().Split('\n');
                }
            }
            else
            {
                whiteList = Resource.TextFileSuffix.WhiteList.Split('\n');
            }

            foreach (var temp in whiteList.Select(temp=>temp.Replace("\r","").Trim()))
            {
                DefaultWhiteList.Add(temp);
            }
        }

        public InspectFileWhiteListSetting(List<string> whiteList)
        {
            //if (_folderRegex == null)
            //{
            //    string folderRegex = "\\s+\\\\";
            //    _folderRegex=new Regex(folderRegex);
            //}
            foreach (var temp in whiteList)
            {
                Parse(temp);
            }
        }

        public static List<string> DefaultWhiteList { set; get; }

        public void Add(string whiteList)
        {
            Parse(whiteList);
        }

        public void Remove(string whiteList)
        {
            var folderWhiteList = ((List<string>)FolderWhiteList);

            Remove(whiteList, folderWhiteList);
            folderWhiteList = (List<string>)FileWhiteList;
            Remove(whiteList, folderWhiteList);
        }

        private static void Remove(string whiteList, List<string> folderWhiteList)
        {
            for (int i = 0; i < folderWhiteList.Count; i++)
            {
                if (string.Equals(folderWhiteList[i], whiteList))
                {
                    folderWhiteList.RemoveAt(i);
                }
            }
        }

        public void Add(List<string> whiteList)
        {
            foreach (var temp in whiteList)
            {
                Parse(temp);
            }
        }

        private void Parse(string whiteList)
        {
            if (_folderRegex.IsMatch(whiteList))
            {
                ((List<string>)FolderWhiteList).Add(whiteList.Substring(0, whiteList.Length - 1));
            }
            else
            {
                if (whiteList.Contains("\\") || whiteList.Contains("//"))
                {
                    throw new ArgumentException("不支持指定文件夹中的文件");
                }
                ((List<string>)FileWhiteList).Add(whiteList);
                ((List<Regex>)FileRegexWhiteList).Add(new Regex(GetWildcardRegexString(whiteList)));
            }
        }

        private static Regex _folderRegex = new Regex("\\w+\\\\");

        public IReadOnlyList<string> FileWhiteList { get; } = new List<string>();
        public IReadOnlyList<string> FolderWhiteList { get; } = new List<string>();
        public IReadOnlyList<Regex> FileRegexWhiteList { get; } = new List<Regex>();

        ///// <summary>
        /////     设置或获取白名单
        ///// </summary>
        //public List<string> WhiteList { set; get; } = new List<string>();
        //忽略文件夹      文件夹\
        //忽略文件        文件
        //忽略后缀        *.后缀

        static string GetWildcardRegexString(string wildcardStr)
        {
            Regex replace = new Regex("[.$^{\\[(|)*+?\\\\]");
            return replace.Replace(wildcardStr,
                 delegate (Match m)
                 {
                     switch (m.Value)
                     {
                         case "?":
                             return ".?";
                         case "*":
                             return ".*";
                         default:
                             return "\\" + m.Value;
                     }
                 });
        }
    }
}