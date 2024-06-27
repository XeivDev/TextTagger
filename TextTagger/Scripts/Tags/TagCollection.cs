using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Xeiv.TextTaggerSystem
{
    [CreateAssetMenu(menuName = "Systems/TextTagger/Collection")]
    public class TagCollection : ScriptableObject
    {
        [SerializeField] private List<Tag> tagsFromCollection = new List<Tag>();
        public List<Tag> TagsFromCollection { get => tagsFromCollection; }
    }
}

