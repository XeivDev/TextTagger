using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Xeiv.TextTaggerSystem
{
    [CreateAssetMenu(menuName = "Systems/TextTagger/Tags/PauseTag")]
    public class PauseTag : Tag
    {
        [Header("Configuration")]
        public float defaultTime = 1;

        [Space(50)]
        [CustomHelp("This tag adds a time pause at the place where the tag is placed. Once the time has passed, the text will continue.\r\n\nHow to use it:\r\n\t<tagName> to use the default values.\r\n\t<tagName=value> to use the specified value.", MessageType.Info, 0, 1, 1)]
        public string comment = "Just an editor Variable";

        public override WaitForSeconds ApplyEffect(TextTagger controller, List<ParameterData> data)
        {
            return new WaitForSeconds(data[0].floatParameter);
        }

        public override void UpdateEffect(TextTagger controller, Vector3[] textVertices, List<ParameterData> data)
        {
            return;
        }

        protected override List<ParameterData> ParseParameters(string parameters)
        {
            
            ParameterData param1 = new ParameterData();
            List<ParameterData> data = new List<ParameterData>(1);

            if (parameters!=null)
            {
                string[] parametersArray = parameters.Split(',');
                param1.floatParameter = float.Parse(parametersArray[0].Replace(".", ","));
            }
            else
                param1.floatParameter = defaultTime;

            data.Add(param1);
            return data;
        }
    }
}
