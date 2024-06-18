using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Dialogue
{
    [SerializeField] List<string> lines;

    public List<string> Lines {
        get {return lines;}
    }

    public void AddLine(string line)
    {
        lines.Add(line);
    }

    public void ClearLines()
    {
        lines.Clear();
    }
}
