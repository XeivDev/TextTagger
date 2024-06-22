---

# TextTagger for Unity

## Table of Contents
1. [Introduction](#introduction)
2. [Installation](#installation)
3. [Getting Started](#getting-started)
    - [Adding the TextTagger Component](#adding-the-texttagger-component)
    - [Assigning TMP_Text](#assigning-tmp_text)
    - [Assigning Tags](#assigning-tags)
4. [Creating Custom Tags](#creating-custom-tags)
    - [Inheriting from Tag Main Class](#inheriting-from-tag-main-class)
5. [Advanced Usage](#advanced-usage)
    - [Combining Multiple Tags](#combining-multiple-tags)
    - [Tag Priorities](#tag-priorities)
6. [Troubleshooting](#troubleshooting)
7. [FAQs](#faqs)

---

## Introduction

Welcome to the TextTagger for Unity. This toolset allows you to create and manage custom tags for TextMeshPro components, enabling enhanced text formatting and functionality within your Unity projects.

---

## Installation

1. **Download the Asset:**
   - Obtain the TextTagger asset from your preferred source.

---

## Getting Started

### Adding the TextTagger Component

1. **Select the GameObject:**
   - In your Unity scene, select the GameObject that contains the `TMP_Text` component you want to tag.

2. **Add Component:**
   - In the Inspector window, click `Add Component`.
   - Search for `TextTagger` and add it to the GameObject.

### Assigning TMP_Text

1. **Locate TMP_Text Component:**
   - Ensure the selected GameObject has a `TMP_Text` component. If not, add a `TextMeshPro - Text` component.

2. **Assign TMP_Text:**
   - In the TextTagger component, drag the `TMP_Text` component into the `TMP_Text` field of the TextTagger.

### Assigning Tags

1. **Create Tag (ScriptableObject):**
   - Right-click in the Project window and select `Create -> Systems -> TextTagger -> YourTag`.
   - Configure the tag as needed.

2. **Assign Tag:**
   - In the TextTagger component, drag the created Tag (ScriptableObject) into the `AvailableTags` field.

---

## Creating Custom Tags

### Inheriting from Tag Main Class

1. **Create a New Tag Script:**
   - In the Project window, create a new C# script that inherits from the `Tag` main class and add the namespace `Xeiv.TextTaggerSystem`.

```csharp
using UnityEngine;
using Xeiv.TextTaggerSystem;

[CreateAssetMenu(fileName = "NewCustomTag", menuName = "Systems/TextTagger/CustomTag")]
public class CustomTag : Tag
{
    public override WaitForSeconds ApplyEffect(TextTagger controller, List<ParameterData> data)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateEffect(TextTagger controller, Vector3[] textVertices, List<ParameterData> data)
    {
        throw new System.NotImplementedException();
    }

    protected override List<ParameterData> ParseParameters(string parameters)
    {
        throw new System.NotImplementedException();
    }
}
```

2. **Implement Custom Behavior:**
   - Override necessary methods and define the custom behavior for your tag.

## Advanced Usage

### Combining Multiple Tags

1. **Multiple Tags in Text:**
   - Use multiple tags within the same `TMP_Text` component by specifying them in the text string.
   - Combine multiple tags nesting them.

```csharp
string text = "<custom>Custom Tag Text</custom> <another>Another Tag</another>";
tmpTextComponent.text = text;
```

### Tag Priorities

1. **Define Tag Priority:**
   - In the ScriptanbleObject asset, define the priority if needed to handle tag conflicts.

## Troubleshooting

### Common Issues

- **Tag Not Appearing:**
  - Ensure the tag ScriptableObject is correctly assigned to the TextTagger component.
  - Verify that the tag name in the text string matches the tag name defined in the ScriptableObject.

- **Text Formatting Issues:**
  - Check for conflicts between multiple tags and adjust priorities as needed.

---

## FAQs

1. **How do I create a new tag?**
   - Follow the steps in the "Creating Custom Tags" section.

2. **Can I use multiple tags in the same text?**
   - Yes, you can combine multiple tags in the same `TMP_Text` component.

3. **What if my tag isn't working?**
   - Refer to the "Troubleshooting" section for common issues and solutions.

---

