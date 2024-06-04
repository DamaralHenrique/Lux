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
    [SerializeField] GameObject choicePanel;
    [SerializeField] Image choiceImage;
    [SerializeField] List<TextMeshProUGUI> choiceTexts;

    public event Action OnShowDialogue;
    public event Action OnCloseDialogue;
    public event Action OnPasswordCorrect;

    public int showInputAtLine = 2;
    public int showChoiceAtLine = 5;

    Dialogue dialogue;
    int currentLine = 0;
    bool isTyping;
    int selectedChoiceIndex = 0;
    Coroutine typingCoroutine;

    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this; // In order to reference in any class
    }

    private void Start()
    {
        dialogueBox.SetActive(false);
        inputFieldAndButton.SetActive(false);
        choicePanel.SetActive(false);
        button.onClick.AddListener(OnButtonClicked);
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isTyping && !choicePanel.activeSelf)
        {
            ++currentLine;
            if (currentLine < dialogue.Lines.Count)
            {
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                }
                typingCoroutine = StartCoroutine(TypeDialogue(dialogue.Lines[currentLine]));
                if (currentLine == showInputAtLine)
                {
                    inputFieldAndButton.SetActive(true);
                    choicePanel.SetActive(false);
                }
                if (currentLine == showChoiceAtLine)
                {
                    choicePanel.SetActive(true);
                    inputFieldAndButton.SetActive(false);
                    selectedChoiceIndex = 0; // Reset choice index
                    UpdateChoiceHighlight();
                }
            }
            else
            {
                currentLine = 0;
                dialogueBox.SetActive(false);
                inputFieldAndButton.SetActive(false);
                choicePanel.SetActive(false);
                OnCloseDialogue?.Invoke();
            }
        }

        if (choicePanel.activeSelf)
        {
            HandleChoiceNavigation();
        }
    }

    public IEnumerator ShowDialogue(Dialogue dialogue)
    {
        yield return new WaitForEndOfFrame();

        OnShowDialogue?.Invoke();

        this.dialogue = dialogue;
        dialogueBox.SetActive(true);
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeDialogue(dialogue.Lines[0]));
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

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        if (userInput == "123")
        {
            typingCoroutine = StartCoroutine(TypeDialogue("Senha correta!"));
            OnPasswordCorrect?.Invoke();
        }
        else
        {
            typingCoroutine = StartCoroutine(TypeDialogue("Senha incorreta. Verifique o puzzle e tente novamente."));
        }
    }

    private void HandleChoiceNavigation()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            selectedChoiceIndex = (selectedChoiceIndex - 1 + choiceTexts.Count) % choiceTexts.Count;
            UpdateChoiceHighlight();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            selectedChoiceIndex = (selectedChoiceIndex + 1) % choiceTexts.Count;
            UpdateChoiceHighlight();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            OnChoiceSelected(selectedChoiceIndex);
        }
    }

    private void UpdateChoiceHighlight()
    {
        for (int i = 0; i < choiceTexts.Count; i++)
        {
            if (i == selectedChoiceIndex)
            {
                choiceTexts[i].color = Color.gray;
            }
            else
            {
                choiceTexts[i].color = Color.white;
            }
        }
    }

    public void OnChoiceSelected(int choiceIndex)
    {
        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }
        
        string chosenText = choiceTexts[choiceIndex].text;
        Debug.Log("Choice selected: " + chosenText);

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeDialogue("You chose: " + chosenText));

        dialogueBox.SetActive(true);
    }
}
