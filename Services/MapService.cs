using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DGP.Genshin.DataViewer.Services
{
    public class MapService
    {
        public static Dictionary<string, string>? TextMap;
        public static Lazy<Dictionary<string, string>>? NPCMap;
        public static string GetMappedTextBy(JProperty p)
        {
            return GetMappedTextBy(p.Value.ToString());
        }

        public static string GetMappedTextBy(string str)
        {
            return TextMap != null && TextMap.TryGetValue(str, out string? result) ? RemoveHtmlFormat(result) : str;
        }
        public static string GetMappedNPCBy(string id)
        {
            return NPCMap != null && NPCMap.Value.TryGetValue(id, out string? result) ? GetMappedTextBy(result) : id;
        }

        public static string RemoveHtmlFormat(string s)
        {
            //match the format required string
            if (s.StartsWith("#"))
            {
                s = s.Remove(0, 1);
            }
            //color
            s = new Regex(@"<color=.*?>").Replace(s, "");
            s = s.Replace("</color>", "");
            //important mark
            s = s.Replace("<i>", "").Replace("</i>", "");
            //nickname
            s = s.Replace("{NICKNAME}", "[!:玩家昵称]");
            //apply \n & \r char
            s = s.Replace(@"\n", "\n").Replace(@"\r", "\r");
            return s;
        }

    }
}
