using System; // For Action
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;
using TMPro; // For TextMeshProUGUI

public class DialogueManager : MonoBehaviour
{
    [SerializeField] int lettersPerSecond = 45;
    [SerializeField] GameObject dialogueBox;
    [SerializeField] TextMeshProUGUI dialogueText;

    public event Action OnShowDialogue;
    public event Action OnCloseDialogue;

    Dialogue dialogue;
    int currentLine = 0;
    bool isTyping;

    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this; // In order to reference in any class
    }

    private void Start()
    {
        dialogueBox.SetActive(false);
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isTyping)
        {
            ++currentLine;
            if (currentLine < dialogue.Lines.Count)
            {
                StartCoroutine(TypeDialogue(dialogue.Lines[currentLine]));
            }
            else
            {
                currentLine = 0;
                dialogueBox.SetActive(false);
                OnCloseDialogue?.Invoke();
            }
        }
    }

    public IEnumerator ShowDialogue(Dialogue dialogue)
    {
        yield return new WaitForEndOfFrame();

        OnShowDialogue?.Invoke();

        this.dialogue = dialogue;
        dialogueBox.SetActive(true);
        StartCoroutine(TypeDialogue(dialogue.Lines[0]));
    }

    public IEnumerator TypeDialogue(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
    }
}
