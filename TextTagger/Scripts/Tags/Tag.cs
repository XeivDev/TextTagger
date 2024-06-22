using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Xeiv.TextTaggerSystem {

    public struct ParameterData
    {
        public string stringParameter;
        public int intParameter;
        public float floatParameter;
        public Vector2 vector2Parameter;
        public Vector3 vector3Parameter;
        public object[] arrayParameter;
    }
    public abstract class Tag : ScriptableObject
    {

        [field: SerializeField] public string TagName { get; private set; } = "tagName";
        [field: SerializeField] public int TagPriority { get; private set; } = 0;
        [field: SerializeField] public bool IsSingleTag { get; private set; } = false;


        public List<ParameterData> GetParameters(string tagData)
        {
            Regex valueRegex = new Regex(@"<\w+=(.+?)>");
            Match match = valueRegex.Match(tagData);

            if (match.Success)
            {
                return ParseParameters(match.Groups[1].Value);
            }
            else
            {
                return ParseParameters(null);
            }
        }

        protected abstract List<ParameterData> ParseParameters(string parameters);
        public abstract WaitForSeconds ApplyEffect(TextTagger controller, List<ParameterData> data);
        public abstract void UpdateEffect(TextTagger controller,Vector3[] textVertices, List<ParameterData> data);
    }
}
