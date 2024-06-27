
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;


namespace Xeiv.TextTaggerSystem
{
    [CreateAssetMenu(menuName = "Systems/TextTagger/Tags/WobbleTag")]
    public class WobbleTag : Tag
    {
        [System.Serializable]
        public enum Mode
        {
            VertexWobble,
            CharacterWobble,
            // WordWoble
        }


        [Header("Configuration")]
        public Vector2 defaultStrength = new Vector2(1, 1);
        public float defaultSpeed = 3.1f;
        public Mode defaultMode;

        [Space(50)]
        [CustomHelp("This tag modifies the position of each character encapsulated by the tag by moving each character or vertex by a predefined offset.\r\n\nHow to use it:\r\n\t<tagName> to use the default values.\r\n\t<tagName=strengthX,strengthY,speed,mode> to use the specified values.\r\n\nModes:\r\n\t0 --> By Vertex\r\n\t1 --> By Character", MessageType.Info, 0, 1, 1)]
        public string comment = "Just an editor Variable";

        private WobbleTag()
        {
            AutoClosing = false;
        }

        public override WaitForSeconds ApplyEffect(TextTagger controller, List<ParameterData> data)
        {
            return new WaitForSeconds(0);
        }

        public override void UpdateEffect(TextTagger controller, Vector3[] textVertices, List<ParameterData> data)
        {
            Vector2 strength = data[0].vector2Parameter;
            float speed = data[1].floatParameter;
            Mode mode = (Mode)data[2].intParameter;

            Vector2Int areaOfAction = controller.CurrentAreaOfAction;

            for (int i = areaOfAction.x; i < areaOfAction.y; i++)
            {
                TMP_CharacterInfo charInfo = controller.tmpText.textInfo.characterInfo[i];
                int index = charInfo.vertexIndex;

                var offset = Vector3.zero;

                if (charInfo.character == ' ')
                    continue;

                switch (mode)
                {
                    case Mode.VertexWobble:

                        for (byte corner = 0; corner < 4; corner++)
                        {
                            offset = Wobble(strength, speed, i * corner);
                            textVertices[index + corner] += offset;
                        }
                        break;
                    case Mode.CharacterWobble:
                        offset = Wobble(strength, speed, index);
                        for (byte corner = 0; corner < 4; corner++)
                        {
                            textVertices[index + corner] += offset;
                        }
                        break;
                    //case Mode.WordWoble:

                    //    break;
                    default:
                        break;
                }


            }
        }

        private Vector3 Wobble(Vector2 amplitude, float speed, float offset)
        {

            return new Vector3(Mathf.Sin(((Time.time * speed) + offset)) * amplitude.x, Mathf.Cos(((Time.time * speed) + offset)) * amplitude.y);
        }

        protected override List<ParameterData> ParseParameters(string parameters)
        {
            List<ParameterData> data = new List<ParameterData>(3);
            ParameterData param1 = new ParameterData();
            ParameterData param2 = new ParameterData();
            ParameterData param3 = new ParameterData();
            if (parameters != null)
            {
                string[] parametersArray = parameters.Split(',');
                param1.vector2Parameter = new Vector2(float.Parse(parametersArray[0].Replace(".", ",")), float.Parse(parametersArray[1].Replace(".", ",")));
                param2.floatParameter = float.Parse(parametersArray[2].Replace(".", ","));
                param3.intParameter = int.Parse(parametersArray[3]);
            }
            else
            {
                param1.vector2Parameter = defaultStrength;
                param2.floatParameter = defaultSpeed;
                param3.intParameter = (int)defaultMode;
            }

            data.Add(param1);
            data.Add(param2);
            data.Add(param3);
            return data;
        }
    }
}

