using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Xeiv.TextTaggerSystem
{
    [CreateAssetMenu(menuName = "Systems/TextTagger/SpeedTag")]
    public class SpeedTag : Tag
    {
        [Header("Configuration")]
        public float defaultSpeed = 1;


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