using System; // For Action
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // For TextMeshProUGUI

public class DialogueManager : MonoBehaviour
{
    [SerializeField] int lettersPerSecond = 45;
    [SerializeField] GameObject dialogueBox;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] GameObject inputFieldAndButton;
    [SerializeField] TMP_InputField inputFieldText;
    [SerializeField] Button button;

    public event Action OnShowDialogue;
    public event Action OnCloseDialogue;

    Dialogue dialogue;
    int currentLine = 0;
    bool isTyping;
    int showInputAtLine = 2;

    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this; // In order to reference in any class
    }

    private void Start()
    {
        dialogueBox.SetActive(false);
        inputFieldAndButton.SetActive(false);
        button.onClick.AddListener(OnButtonClicked);
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isTyping)
        {
            ++currentLine;
            if (currentLine < dialogue.Lines.Count)
            {
                dialogueText.text = dialogue.Lines[currentLine];
                // TODO: impedir usuário de avançar o diálogo antes de apertar o botão
                StartCoroutine(TypeDialogue(dialogueText.text));
                if (currentLine == showInputAtLine)
                {
                    inputFieldAndButton.SetActive(true);
                }
            }
            else
            {
                currentLine = 0;
                dialogueBox.SetActive(false);
                inputFieldAndButton.SetActive(false);
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

    public void OnButtonClicked()
    {
        inputFieldAndButton.SetActive(false);
        string userInput = inputFieldText.text;
        Debug.Log("User input: " + userInput);

        if (userInput == "123")
        {
            dialogueText.text = "Senha correta!";
        }
        else
        {
            dialogueText.text = "Senha incorreta. Verifique o puzzle e tente novamente.";
        }

        // StartCoroutine(TypeDialogue(dialogue.Lines[currentLine]));
    }
}
