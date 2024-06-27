using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Xeiv.TextTaggerSystem
{
    [CreateAssetMenu(menuName = "Systems/TextTagger/Tags/EffectSoundTag")]
    public class EffectSoundTag : Tag
    {
        [Header("Configuration")]
        public bool selectRandomSound = true;
        public int defaultSoundIndex = 0;
        public AudioClip[] sounds;

        [Space(50)]
        [CustomHelp("This tag plays a sound when it appears on the screen.\r\n\nHow to use it:\r\n\t<tagName> to use the default values.\r\n\t<tagName=-1> play random sound.\r\n\t<tagName=value> play sound by index.", MessageType.Info, 0, 1, 1)]
        public string comment = "Just an editor Variable";


        public override WaitForSeconds ApplyEffect(TextTagger controller, List<ParameterData> data)
        {
            if (controller.effectsAudioSource != null)
            {
                if (data[1].intParameter == 1)
                {
                    if (sounds.Length == 0)
                        controller.effectsAudioSource.clip = null;
                    else
                    {
                        controller.effectsAudioSource.clip = sounds[Random.Range(0, sounds.Length)];
                        controller.effectsAudioSource.Play();
                    }

                }
                else
                {
                    if (sounds.Length == 0)
                        controller.effectsAudioSource.clip = null;
                    else
                    {
                        controller.effectsAudioSource.clip = sounds[data[0].intParameter];
                        controller.effectsAudioSource.Play();
                    }

                }
            }
            else
            {
                //Debug.LogWarning("No EffectsAudioSource Reference, sound will not be played", this);
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