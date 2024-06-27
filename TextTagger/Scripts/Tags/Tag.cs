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

        [SerializeField]private string tagName = "tagName";

        [SerializeField] private int tagPriority = 0;

        [SerializeField] private bool autoClosing = false;


        public string TagName { get => tagName; protected set => tagName = value; }
        public int TagPriority { get => tagPriority; protected set => tagPriority = value; }
        public bool AutoClosing { get => autoClosing; protected set => autoClosing = value; }

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
