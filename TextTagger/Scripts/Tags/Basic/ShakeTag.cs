
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Xeiv.TextTaggerSystem;

[CreateAssetMenu(menuName = "Systems/TextTagger/ShakeTag")]
public class ShakeTag : Tag
{


    [Header("Configuration")]
    public Vector2 defaultDisplacement = new Vector2(1, 1);
    public float defaultFrequency = 1.1f;


    public override WaitForSeconds ApplyEffect(TextTagger controller, List<ParameterData> data)
    {
        return new WaitForSeconds(0);
    }

    public override void UpdateEffect(TextTagger controller, Vector3[] textVertices, List<ParameterData> data)
    {
        Vector2 displacement = data[0].vector2Parameter;
        float frequency = data[1].floatParameter;

        Vector2Int areaOfAction = controller.currentAreaOfAction;


        ParameterData timerData = data[2];
        ParameterData arrayVertices = data[3];

        if(arrayVertices.arrayParameter == null)
        {
            int lenght = areaOfAction.y - areaOfAction.x;
            arrayVertices.arrayParameter = new object[lenght*4];
            data[3] = arrayVertices;
        }

        

        if (frequency > timerData.floatParameter)
        {
            timerData.floatParameter += Time.deltaTime;
            data[2] = timerData;

            for (int i = areaOfAction.x, x = 0; i < areaOfAction.y; i++, x+=4)
            {
                TMP_CharacterInfo charInfo = controller.tmpText.textInfo.characterInfo[i];

                if (charInfo.character == ' ')
                    continue;

                int index = charInfo.vertexIndex;
                for (byte corner = 0, x2= 0 ; corner < 4; corner++, x2++)
                {
                    
                    textVertices[index + corner] = (Vector3)arrayVertices.arrayParameter[x + x2];
                }
            }     
            return;
        }
        else
        {
            timerData.floatParameter = 0;
            data[2] = timerData;

        }
        
        for (int i = areaOfAction.x, x = 0; i < areaOfAction.y; i++, x+=4)
        {
            TMP_CharacterInfo charInfo = controller.tmpText.textInfo.characterInfo[i];
            int index = charInfo.vertexIndex;

            var offset = Shake(displacement, frequency);
            for (byte corner = 0, x2=0; corner < 4; corner++,x2++)
            {
                textVertices[index + corner] = controller.originalVertices[index + corner] + offset;
                arrayVertices.arrayParameter[x+x2] = textVertices[index + corner];
            }
        }
        data[3] = arrayVertices;

    }

    private Vector3 Shake(Vector2 displacement, float frequency)
    {

        return new Vector3(Random.Range(-displacement.x,displacement.x), Random.Range(-displacement.y, displacement.y));
    }

    protected override List<ParameterData> ParseParameters(string parameters)
    {
        List<ParameterData> data = new List<ParameterData>(3);
        ParameterData param1 = new ParameterData();
        ParameterData param2 = new ParameterData();
        ParameterData param3 = new ParameterData();
        ParameterData param4 = new ParameterData();
        if (parameters != null)
        {
            string[] parametersArray = parameters.Split(',');
            param1.vector2Parameter = new Vector2(float.Parse(parametersArray[0].Replace(".", ",")), float.Parse(parametersArray[1].Replace(".", ",")));
            param2.floatParameter = float.Parse(parametersArray[2].Replace(".", ","));
            param3.floatParameter = param2.floatParameter;
            param4.arrayParameter =null;
        }
        else
        {
            param1.vector2Parameter = defaultDisplacement;
            param2.floatParameter = defaultFrequency;
            param3.floatParameter = defaultFrequency;
            param4.arrayParameter = null;

        }

        data.Add(param1);
        data.Add(param2);
        data.Add(param3);
        data.Add(param4);
        return data;
    }
}
