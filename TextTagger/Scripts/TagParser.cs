using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Xeiv.TextTaggerSystem
{
    public class TagParser 
    {
        public static string ParseTags(string textNotParsed, List<Tag> tags)
        {
            string textParsed = textNotParsed;

            const string start = "(<)";

            const string argumentsStart = "(=)";
            const string arguments = "([^>]+)";
            const string end = "(>)";
            const string closingStart = "(</)";

            foreach (var tag in tags)
            {
                string name = "(" + tag.tagName + ")";

                if (tag.isSingleTag) 
                {
                    textParsed = Regex.Replace(textParsed, start + name + argumentsStart + arguments + end, "<link=$2|$4></link>");
                    textParsed = Regex.Replace(textParsed, start + name + end, "<link=$2></link>");
                }
                else
                {
                    textParsed = Regex.Replace(textParsed, start + name + argumentsStart + arguments + end, "<link=$2|$4>");
                    textParsed = Regex.Replace(textParsed, start + name + end, "<link=$2>");
                }
                textParsed = Regex.Replace(textParsed, closingStart + name + end, "</link>");
            }

            return textParsed;
        }

    }
}
