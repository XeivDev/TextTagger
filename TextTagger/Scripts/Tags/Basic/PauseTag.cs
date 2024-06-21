using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Xeiv.TextTaggerSystem
{
    [CreateAssetMenu(menuName = "Systems/TextTagger/PauseTag")]
    public class PauseTag : Tag
    {
        [Header("Configuration")]
        public float defaultTime = 1;


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
