using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Xeiv.TextTaggerSystem;

[CreateAssetMenu(menuName = "Systems/TextTagger/Tags/ActionTag")]


public class ActionTag : Tag
{
    [Header("Configuration")]
    public string defaultActionName = "action";

    [Space(50)]
    [CustomHelp("This tag calls an OnActionTagCall event with the ActionName as argument.\r\n\nHow to use it:\r\n \t<tagName> to use the default values.\r\n\t<tagName=actionName> to use the specified value.", MessageType.Info, 0, 1, 1)]
    public string comment = "Just an editor Variable";


    public override WaitForSeconds ApplyEffect(TextTagger controller, List<ParameterData> data)
    {
        controller.OnActionTagCall?.Invoke(data[0].stringParameter);
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
            param1.stringParameter = parametersArray[0];
        }
        else
            param1.stringParameter = defaultActionName;


        data.Add(param1);
        return data;
    }
}
