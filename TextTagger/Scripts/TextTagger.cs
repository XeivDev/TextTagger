using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using static UnityEditor.Progress;

namespace Xeiv.TextTaggerSystem
{
    public class TextTagger : MonoBehaviour
    {
        [Header("Configuration")]
        public float textSpeed = 10;
        public ShowMode mode;

        [Header("Tags")]
        [SerializeField] private List<Tag> availableTags = new List<Tag>();
        [SerializeField] private List<TagCollection> availableCollections = new List<TagCollection>();

        [Header("References")]
        public TMP_Text tmpText;
        public AudioSource lettersAudioSource;
        public AudioSource effectsAudioSource;

        [Header("Events")]
        public Action OnStartReading;
        public Action OnEndReading;
        public Action OnCharacterAppear;
        public Action OnWordAppear;
        public Action<string> OnActionTagCall;


        /// <summary>
        /// Accesible variables
        /// </summary>

        public int CurrentCharacterIndex { get; private set; }
        public int CurrentWordIndex { get; private set; }
        public Vector2Int CurrentAreaOfAction { get; private set; }
        public Vector3[] OriginalVertices { get; private set; }
        public AudioClip[] LettersAudioClips { get; set; }
        public bool IsRandomLetterAudioClip { get; set; }
        public bool IsReading { get; private set; } = false;


        /// <summary>
        /// Not accesible variables
        /// </summary>
        private Dictionary<string, Tag> tags = new Dictionary<string, Tag>();
        private Mesh textMesh;
        private Vector3[] textVertices;
        

        

        struct TagRequieredData
        {
            public Tag tag;
            public Vector2Int bounds;
            public List<ParameterData> requieredParameters;
            public TagRequieredData(Tag tag, Vector2Int bounds, List<ParameterData> requieredParameters)
            {
                this.tag = tag;
                this.bounds = bounds;
                this.requieredParameters = requieredParameters;
            }
        }

        public enum ShowMode
        {
            ByWord,
            ByCharacter
        }

        List<TagRequieredData> tagsData = new List<TagRequieredData>();
        List<string> wordsData = new List<string>();

        private void Awake()
        {
            tags = new Dictionary<string, Tag>();
            for (int i = 0; i < availableTags.Count; i++)
            {
                if (!tags.ContainsKey(availableTags[i].TagName))
                    tags.Add(availableTags[i].TagName, availableTags[i]);
                else
                {
                    Debug.LogWarning($"Some tags are already added\nTag: <{availableTags[i].TagName}>", this);
                }
            }

            foreach (var collection in availableCollections)
            {
                for (int i = 0; i < collection.TagsFromCollection.Count; i++)
                {
                    if(!tags.ContainsKey(collection.TagsFromCollection[i].TagName))
                        tags.Add(collection.TagsFromCollection[i].TagName, collection.TagsFromCollection[i]);
                    else
                    {
                        Debug.LogWarning($"Some tags are already added\nTag: <{collection.TagsFromCollection[i].TagName}>", this);
                    }
                }
            }

            SetText(tmpText.text);
            StartReading();
        }
        private void Parse()
        {
            var textProcessingResults = TagParser.ProcessText(tmpText.text, availableTags);
            var pairsData = TagParser.PairTags(textProcessingResults.openingTags, textProcessingResults.closingTags);

            tmpText.text = textProcessingResults.processedText;
            tmpText.ForceMeshUpdate();

            tagsData.Clear();


            foreach (var pair in pairsData.pairedTags)
            {
                Vector2Int boundaries = new Vector2Int(pair.openingTag.index, pair.closingTag.index);
                Tag tag = tags[pair.openingTag.name];
                List<ParameterData> requieredParameters = tag.GetParameters(pair.openingTag.fullTag);
                TagRequieredData requieredData = new TagRequieredData(tag, boundaries, requieredParameters);
                tagsData.Add(requieredData);
            }

            foreach (var pair in pairsData.unpairedTags)
            {
                Vector2Int boundaries = new Vector2Int(pair.index, pair.index);
                Tag tag = tags[pair.name];
                
                if (!tag.IsSingleTag)
                    continue;
                List<ParameterData> requieredParameters = tag.GetParameters(pair.fullTag);
                TagRequieredData requieredData = new TagRequieredData(tag, boundaries, requieredParameters);
                tagsData.Add(requieredData);
            }



            tagsData.Sort((a, b) => b.tag.TagPriority.CompareTo(a.tag.TagPriority));
            OriginalVertices = tmpText.mesh.vertices;
            wordsData = ExtractWords(tmpText.GetParsedText());

        }

        private void Update()
        {
            tmpText.maxVisibleCharacters = CurrentCharacterIndex;
            tmpText.ForceMeshUpdate();
            textMesh = tmpText.mesh;
            textVertices = textMesh.vertices;

            for (int i = 0; i < tagsData.Count; i++)
            {
                Tag tagToUpdate = tagsData[i].tag;
                if (tagToUpdate.IsSingleTag)
                    continue;
                if (tagsData[i].bounds.x <= CurrentCharacterIndex)
                {
                    CurrentAreaOfAction = tagsData[i].bounds;
                    tagToUpdate.UpdateEffect(this, textVertices, tagsData[i].requieredParameters);
                }
            }
            textMesh.vertices = textVertices;
            tmpText.canvasRenderer.SetMesh(textMesh);


        }

        public void SetText(string text)
        {
            tmpText.text = text;
            Parse();
        }
        public void StartReading()
        {
            StopAllCoroutines();
            StartCoroutine(Read());
        }

        IEnumerator Read()
        {
            
            TMP_TextInfo textInfo = tmpText.textInfo;
            int maxCharacters = textInfo.characterCount;
            tmpText.maxVisibleCharacters = 0;
            CurrentCharacterIndex=0;
            CurrentWordIndex = 0;
            IsReading = true;
            OnStartReading?.Invoke();


            yield return new WaitForEndOfFrame();



           
            while(true)
            {
                int fixedIndex = 0;
                int counter = 0;


                switch (mode)
                {
                    case ShowMode.ByWord:
                        //// Word ReadingMode
                        ///

                        int startIndex = 0;
                        for (; counter < CurrentWordIndex; counter++)
                        {
                            startIndex += wordsData[counter].Length + 1;
                        }
                        counter = 0;
                        Vector2Int boundaries = new Vector2Int(startIndex, wordsData[CurrentWordIndex].Length);
                        for (int i = 0; i < tagsData.Count; i++)
                        {
                            Tag tagToUpdate = tagsData[i].tag;
                            if (tagsData[i].bounds.x >= boundaries.x && tagsData[i].bounds.x < boundaries.x + boundaries.y)
                            {
                                yield return tagToUpdate.ApplyEffect(this, tagsData[i].requieredParameters);
                            }
                        }

                        /// Play Sound
                        PlaySound();

                        CurrentWordIndex += 1;
                        OnWordAppear?.Invoke();
                      
                        /// FixCurrentLetterTrack
                        for (; counter < CurrentWordIndex; counter++)
                        {
                            fixedIndex += wordsData[counter].Length + 1;
                        }
                        CurrentCharacterIndex = fixedIndex;
                        break;
                    case ShowMode.ByCharacter:
                        //// Letter ReadingMode
                        ///

                        for (int i = 0; i < tagsData.Count; i++)
                        {
                            Tag tagToUpdate = tagsData[i].tag;
                            if (tagsData[i].bounds.x == CurrentCharacterIndex)
                            {
                                yield return tagToUpdate.ApplyEffect(this, tagsData[i].requieredParameters);
                            }
                        }

                        /// Play Sound
                        if (textInfo.characterInfo.Length > CurrentCharacterIndex && textInfo.characterInfo[CurrentCharacterIndex].character != ' ')
                        {
                            PlaySound();
                        }

                        CurrentCharacterIndex += 1;
                        OnCharacterAppear?.Invoke();                       

                        /// FixCurrentWordTrack
                        foreach (string word in wordsData)
                        {
                            if (counter + word.Length >= CurrentCharacterIndex)
                            {
          
                                break;
                            }
                            fixedIndex++;
                            counter += word.Length + 1;
                        }
                        if (CurrentWordIndex != fixedIndex)
                            OnWordAppear?.Invoke();
                        CurrentWordIndex = fixedIndex;
                        break;
                    default:
                        break;
                }
                
                yield return new WaitForSeconds(1 / textSpeed);

     
                if (CurrentWordIndex >= wordsData.Count || CurrentCharacterIndex >= maxCharacters)
                    break;
            }

            CompleteReading();
            yield return null;
        }

        public void CompleteReading()
        {
            if (IsReading)
            {
                StopAllCoroutines();
                TMP_TextInfo textInfo = tmpText.textInfo;
                CurrentCharacterIndex = textInfo.characterCount;
                tmpText.maxVisibleCharacters = textInfo.characterCount;

                lettersAudioSource.clip = null;
                effectsAudioSource.clip = null;
                LettersAudioClips = new AudioClip[0];

                IsRandomLetterAudioClip = false;
                IsReading = false;

                OnEndReading?.Invoke();
            }   
        }

        private AudioClip SelectLetterAudioClip()
        {
            if (LettersAudioClips.Length == 0)
                return null;
            return LettersAudioClips[UnityEngine.Random.Range(0, LettersAudioClips.Length)];
        }

        private List<string> ExtractWords(string text)
        {
            List<string> splitText = text.Split(' ').ToList();

            

            for (int i = 0; i < splitText.Count; i++)
            {
                splitText[i] = splitText[i].Replace("\n", "");
                if (splitText[i] == "")
                {
                    splitText.RemoveAt(i);
                    i--;
                }
            }

            return splitText;
        }

        private void PlaySound()
        {
            if (lettersAudioSource == null)
            {
                //Debug.LogWarning("No LettersAudioSource Reference, sound will not be played", this);
            }
            else if (IsRandomLetterAudioClip)
            {
                lettersAudioSource.clip = SelectLetterAudioClip();
            }
            lettersAudioSource.Play();
        }
    }
}
