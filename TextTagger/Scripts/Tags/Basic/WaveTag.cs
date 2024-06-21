using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Xeiv.TextTaggerSystem
{
    [CreateAssetMenu(menuName = "Systems/TextTagger/WaveTag")]
    public class WaveTag : Tag
    {
        [Header("Configuration")]
        public Vector2 defaultAmplitude = new Vector2(1,1);
        public float defaultFrequency = 1.1f;


        public override WaitForSeconds ApplyEffect(TextTagger controller, List<ParameterData> data)
        {
            return new WaitForSeconds(0);
        }

        public override void UpdateEffect(TextTagger controller, Vector3[] textVertices, List<ParameterData> data)
        {
            Vector2 amplitude = data[0].vector2Parameter;
            float frequency = data[1].floatParameter;

            Vector2Int areaOfAction = controller.currentAreaOfAction;
            
            for (int i = areaOfAction.x; i < areaOfAction.y; i ++)
            {
                TMP_CharacterInfo charInfo = controller.tmpText.textInfo.characterInfo[i];

                if (charInfo.character == ' ')
                    continue;

                int index = charInfo.vertexIndex;
                
                var offset = Wave(amplitude, frequency, Time.time+i);

                for (byte corner = 0; corner < 4; corner++)
                {
                    textVertices[index + corner] += offset;
                }
            }

            Debug.Log("Waving");
        }

        private Vector3 Wave(Vector2 amplitude, float frequency, float offset)
        {
            float x = amplitude.x * Mathf.Sin(2 * Mathf.PI * frequency * (Time.time + offset));
            float y = amplitude.y * Mathf.Cos(2 * Mathf.PI * frequency * (Time.time + offset));
            return new Vector3(x, y);
        }

        protected override List<ParameterData> ParseParameters(string parameters)
        {
            List<ParameterData> data = new List<ParameterData>(2);
            ParameterData param1 = new ParameterData();
            ParameterData param2 = new ParameterData();
            if (parameters != null)
            {
                string[] parametersArray = parameters.Split(',');
                param1.vector2Parameter = new Vector2(float.Parse(parametersArray[0].Replace(".", ",")), float.Parse(parametersArray[1].Replace(".", ",")));
                param2.floatParameter = float.Parse(parametersArray[2].Replace(".", ","));
            }
            else
            {
                param1.vector2Parameter = defaultAmplitude;
                param2.floatParameter = defaultFrequency;
            }

            data.Add(param1);
            data.Add(param2);
            return data;
        }
    }
}
