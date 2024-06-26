using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Xeiv.TextTaggerSystem
{

    [CreateAssetMenu(menuName = "Systems/TextTagger/Tags/SpeedTag")]
    
    
    public class SpeedTag : Tag
    {
        [Header("Configuration")]
        public float defaultSpeed = 1;

        [Space(50)]
        [CustomHelp("This tag modifies the number of characters per second to be displayed on the screen.\r\n\nHow to use it:\r\n \t<tagName> to use the default values.\r\n\t<tagName=number> to use the specified value.", MessageType.Info, 0, 1, 1)]
        public string comment = "Just an editor Variable";

        private SpeedTag()
        {
            AutoClosing = true;
        }

        public override WaitForSeconds ApplyEffect(TextTagger controller, List<ParameterData> data)
        {
            controller.textSpeed = data[0].floatParameter;
            return new WaitForSeconds(0);
        }

        public override void UpdateEffect(TextTagger controller, Vector3[] textVertices, List<ParameterData> data)
        {
            return;
        }

        protected override List<ParameterData> ParseParameters(string parameters)
        {
            ParameterData param1 = new ParameterData();
            List<ParameterData> data = new List<ParameterData>(1);

            if (parameters != null)
            {
                string[] parametersArray = parameters.Split(',');
                param1.floatParameter = float.Parse(parametersArray[0].Replace(".", ","));
            }
            else
                param1.floatParameter = defaultSpeed;

            
            data.Add(param1);
            return data;
        }
    }
}