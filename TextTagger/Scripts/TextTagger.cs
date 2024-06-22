using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Xeiv.TextTaggerSystem
{
    public class TextTagger : MonoBehaviour
    {
        public float textSpeed = 10;
        public TMP_Text tmpText;
        [SerializeField] private List<Tag> availableTags = new List<Tag>();
        private Dictionary<string, Tag> tags = new Dictionary<string, Tag>();

        public int currentCharacter { get; private set; }
        public Vector2Int currentAreaOfAction { get; private set; }


        private Mesh textMesh;
        private Vector3[] textVertices;
        public Vector3[] originalVertices { get; private set; }

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

        List<TagRequieredData> tagsData = new List<TagRequieredData>();


        private void Awake()
        {
            for (int i = 0; i < availableTags.Count; i++)
            {
                tags.Add(availableTags[i].tagName, availableTags[i]);
            }

            Parse();
            StartReading();
        }

        [ContextMenu("Parse")]
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
                
                if (!tag.isSingleTag)
                    continue;
                List<ParameterData> requieredParameters = tag.GetParameters(pair.fullTag);
                TagRequieredData requieredData = new TagRequieredData(tag, boundaries, requieredParameters);
                tagsData.Add(requieredData);
            }



            tagsData.Sort((a, b) => b.tag.tagPriority.CompareTo(a.tag.tagPriority));
            originalVertices = tmpText.mesh.vertices;
        }

        private void Update()
        {
            tmpText.maxVisibleCharacters = currentCharacter;
            tmpText.ForceMeshUpdate();
            textMesh = tmpText.mesh;
            textVertices = textMesh.vertices;

            for (int i = 0; i < tagsData.Count; i++)
            {
                Tag tagToUpdate = tagsData[i].tag;
                if (tagToUpdate.isSingleTag)
                    continue;
                if (tagsData[i].bounds.x <= currentCharacter)
                {
                    currentAreaOfAction = tagsData[i].bounds;
                    tagToUpdate.UpdateEffect(this, textVertices, tagsData[i].requieredParameters);
                }
            }
            textMesh.vertices = textVertices;
            tmpText.canvasRenderer.SetMesh(textMesh);


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
           yield return new WaitForEndOfFrame();
            while (currentCharacter < maxCharacters)
            {
                for (int i = 0; i < tagsData.Count; i++)
                {
                    Tag tagToUpdate = tagsData[i].tag;
                    if (tagsData[i].bounds.x == currentCharacter)
                    {
                        yield return tagToUpdate.ApplyEffect(this, tagsData[i].requieredParameters);
                    }
                }
                currentCharacter += 1;
                yield return new WaitForSeconds(1/textSpeed); 
            }
            yield return null; 
        }

        public void CompleteReading()
        {
            TMP_TextInfo textInfo = tmpText.textInfo;
            currentCharacter = textInfo.characterCount;
            tmpText.maxVisibleCharacters = textInfo.characterCount;
        }



    }
}
