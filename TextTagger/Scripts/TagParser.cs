using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Xeiv.TextTaggerSystem
{
    public class TagParser 
    {
        public struct TagData
        {
            public int index;
            public string name;
            public string fullTag;

            public TagData(int index, string name, string fullTag)
            {
                this.index = index;
                this.name = name;
                this.fullTag = fullTag;
            }
        }

        private static bool FindTagByName(string tagName, List<Tag> tags)
        {
            foreach (var item in tags)
            {
                if(item.tagName == tagName)
                    return true;
            }
            return false;
        }

        public static (string processedText, int realTotalLenght, List<TagData> openingTags, List<TagData> closingTags) ProcessText(string textToParse, List<Tag> tags)
        {
            List<TagData> openingTags = new List<TagData>();
            List<TagData> closingTags = new List<TagData>();
            string processedText = textToParse;

            // Expresión regular para encontrar las etiquetas XML
            Regex tagRegex = new Regex(@"<(/?)(\w+)[^>]*>");
            int indexOffset = 0;

            // Reemplazar las etiquetas XML por espacios vacíos y almacenar las etiquetas en las listas con sus índices
            processedText = tagRegex.Replace(processedText, match =>
            {
                int index = match.Index - indexOffset;
                string tagContent = match.Groups[0].Value;
                string tagType = match.Groups[1].Value;
                string tagName = match.Groups[2].Value;

                if (!FindTagByName(tagName, tags))
                {
                    indexOffset += match.Length;
                    return match.Value;
                }

                if (tagType == "/")
                {
                    closingTags.Add(new TagData(index, tagName, tagContent));
                }
                else
                {
                    openingTags.Add(new TagData(index, tagName, tagContent));
                }
                indexOffset += match.Length;
                return "";
            });
            return (processedText, textToParse.Length - indexOffset, openingTags, closingTags);
        }

        public static (List<(TagData openingTag, TagData closingTag)> pairedTags, List<TagData> unpairedTags) PairTags(List<TagData> openingTags, List<TagData> closingTags)
        {
            var pairedTags = new List<(TagData openingTag, TagData closingTag)>();
            var unpairedOpeningTags = new List<TagData>();

            // Dictionary to keep track of unmatched opening tags by their names
            var unmatchedOpeningTags = new Dictionary<string, List<TagData>>();

            // Populate unmatchedOpeningTags with lists for each tag name
            foreach (var openingTag in openingTags)
            {
                if (!unmatchedOpeningTags.ContainsKey(openingTag.name))
                {
                    unmatchedOpeningTags[openingTag.name] = new List<TagData>();
                }
                unmatchedOpeningTags[openingTag.name].Add(openingTag);
            }

            // Iterate through closing tags to find matches
            foreach (var closingTag in closingTags)
            {
                if (unmatchedOpeningTags.TryGetValue(closingTag.name, out var openingTagList) && openingTagList.Count > 0)
                {
                    // Find the closest opening tag that is not after the closing tag
                    var closestOpeningTag = openingTagList.LastOrDefault(t => t.index <= closingTag.index);

                    if (closestOpeningTag.fullTag != null)
                    {
                        // Add the pair to pairedTags
                        TagData first = new TagData(closestOpeningTag.index, closestOpeningTag.name, closestOpeningTag.fullTag);
                        TagData second = new TagData(closingTag.index, closingTag.name, closingTag.fullTag);
                        pairedTags.Add((first, second));

                        // Remove the opening tag from the unmatched list
                        openingTagList.Remove(closestOpeningTag);

                        // If the list of unmatched opening tags for this name is empty, remove the key
                        if (openingTagList.Count == 0)
                        {
                            unmatchedOpeningTags.Remove(closingTag.name);
                        }
                    }
                    else
                    {
                        // No matching opening tag found, add the closing tag to unpairedOpeningTags
                        unpairedOpeningTags.Add(closingTag);
                    }
                }
                else
                {
                    // No matching opening tag found, add the closing tag to unpairedOpeningTags
                    unpairedOpeningTags.Add(closingTag);
                }
            }

            // Any remaining unmatched opening tags are added to unpairedOpeningTags
            foreach (var unmatched in unmatchedOpeningTags.Values.SelectMany(x => x))
            {
                unpairedOpeningTags.Add(unmatched);
            }

            // Sort pairedTags by opening index for consistency
            pairedTags.Sort((x, y) => x.openingTag.index.CompareTo(y.openingTag.index));

            return (pairedTags, unpairedOpeningTags);
        }

    }
}
