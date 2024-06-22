using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Xeiv.TextTaggerSystem
{
    [CreateAssetMenu(menuName = "Systems/TextTagger/CharacterSoundTag")]
    public class CharacterSoundTag : Tag
    {
        [Header("Configuration")]
        public bool selectRandomSound=true;
        public int defaultSoundIndex = 0;
        public AudioClip[] sounds;

        [Space(50)]
        [CustomHelp("This tag modifies the sound of the character when it appears on the screen.\r\n\nHow to use it:\r\n\t<tagName> to use the default values.\r\n\t<tagName=-1> remove sound.\r\n\t<tagName=-2> play random sound.\r\n\t<tagName=value> play sound by index.", MessageType.Info, 0, 1, 1)]
        public string comment="Just an editor Variable";



        public override WaitForSeconds ApplyEffect(TextTagger controller, List<ParameterData> data)
        {
            if (data[1].intParameter == 1)
            {
                controller.randomLetterAudioClip = true;
                controller.lettersAudioClips = sounds;
            }
            else
            {
                controller.randomLetterAudioClip = false;
                if (data[0].intParameter != -1)
                {
                    if (sounds.Length == 0)
                        controller.lettersAudioSource.clip = null;
                    else
                        controller.lettersAudioSource.clip = sounds[data[0].intParameter];
                }
                else
                    controller.lettersAudioSource.clip = null;
            }
            return new WaitForSeconds(0);
        }

        public override void UpdateEffect(TextTagger controller, Vector3[] textVertices, List<ParameterData> data)
        {
            return;
        }

        protected override List<ParameterData> ParseParameters(string parameters)
        {
            ParameterData param1 = new ParameterData();
            ParameterData param2 = new ParameterData();
            List<ParameterData> data = new List<ParameterData>(1);

            if (parameters != null)
            {
                string[] parametersArray = parameters.Split(',');

                int value = int.Parse(parametersArray[0]);

                param2.intParameter = 0;
                if (value == -1)
                {
                    param1.intParameter = -1;
                }
                else if(value == -2)
                {
                    param2.intParameter = 1;
                }
                else
                {
                    param1.intParameter = value;
                }
                
            }
            else
            {
                param1.intParameter = defaultSoundIndex;
                param2.intParameter = selectRandomSound ? 1 : 0;

            }


            data.Add(param1);
            data.Add(param2);
            return data;
        }
    }
}