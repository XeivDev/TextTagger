using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Xeiv.TextTaggerSystem;
using static Xeiv.TextTaggerSystem.TextTagger;

[CreateAssetMenu(menuName = "Systems/TextTagger/Tags/ReadingModeTag")]


public class ReadingModeTag : Tag
{
    [Header("Configuration")]
    public ShowMode defaultMode = ShowMode.ByCharacter;

    [Space(50)]
    [CustomHelp("This tag changes the read mode of the TextTagger.\r\n\nHow to use it:\r\n \t<tagName> to use the default values.\r\n\t<tagName=modeIndex> to use the specified value.\r\n\nModes:\r\n\t0 --> By Words\r\n\t1 --> By Characters", MessageType.Info, 0, 1, 1)]
    public string comment = "Just an editor Variable";

    private ReadingModeTag()
    {
        AutoClosing = true;
    }

    public override WaitForSeconds ApplyEffect(TextTagger controller, List<ParameterData> data)
    {
        controller.mode = (ShowMode)data[0].intParameter;
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
            param1.intParameter = int.Parse(parametersArray[0]);
        }
        else
            param1.intParameter = (int)defaultMode;


        data.Add(param1);
        return data;
    }
}
