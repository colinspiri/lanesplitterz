using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TextAreaVariable", menuName = "Text Area Variable")]
public class TextAreaVariable : ScriptableObject
{
    [TextArea]
    public string text;
}
