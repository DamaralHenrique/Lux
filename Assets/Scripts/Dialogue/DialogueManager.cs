using System; // For Action
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    public event Action OnCubePuzzleComplete;

    // Add manualmente na cena, em GameController -> DialogueManager
    public List<int> showInputAtLine = new List<int> {};
    public List<int> showChoiceAtLine = new List<int> {};
    public List<ActionLine> doActionAtLine = new List<ActionLine> {};

    // List<int> cubePuzzleAnswers = new List<int> { 7, 2, 5, 4, 0, 1, 6, 3 };
    List<int> cubePuzzleAnswers = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0 }; // For testing

    int currentCubePuzzleIndex = 0;
    Dialogue dialogue;
    int currentLine = 0;
    bool isTyping;
    bool isActionLoop = false;
    bool playerWalkTutorialFirstTime = true;
    int selectedChoiceIndex = 0;
    Coroutine typingCoroutine;
    ActionLine currentActionLine;

    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        dialogueBox.SetActive(false);
        inputFieldAndButton.SetActive(false);
        choicePanel.SetActive(false);
        button.onClick.AddListener(OnButtonClicked);
    }

    public void HandleUpdate(bool forceUpdate = false)
    {
        if(!forceUpdate && isActionLoop){
            RunAction(currentActionLine.action);
            return;
        }
        
        if (forceUpdate || (Input.GetKeyDown(KeyCode.F) && !isTyping && !choicePanel.activeSelf))
        {
            ++currentLine;
            if (currentLine < dialogue.Lines.Count)
            {
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                }
                typingCoroutine = StartCoroutine(TypeDialogue(dialogue.Lines[currentLine]));
                if (showInputAtLine.Contains(currentLine))
                {
                    inputFieldAndButton.SetActive(true);
                    choicePanel.SetActive(false);
                }
                if (showChoiceAtLine.Contains(currentLine))
                {
                    choicePanel.SetActive(true);
                    inputFieldAndButton.SetActive(false);
                    selectedChoiceIndex = 0;
                    UpdateChoiceHighlight();
                }
                ActionLine actionLine = doActionAtLine.Find(i => i.line == currentLine);
                NPCController currentNPC = GetCurrentNPC();
                if (currentNPC != null && currentNPC.isInitialDialog && doActionAtLine.Find(i => i.line == currentLine) != null)
                {
                    currentActionLine = actionLine;
                    RunAction(currentActionLine.action);
                }
            }
            else
            {
                currentLine = 0;
                currentCubePuzzleIndex = 0;
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

    public IEnumerator ShowDialogue(Dialogue dialogue, List<int> inputLines = null, List<int> choiceLines = null, List<ActionLine> actionLines = null)
    {
        yield return new WaitForEndOfFrame();

        OnShowDialogue?.Invoke();

        this.dialogue = dialogue;
        this.showInputAtLine = inputLines;
        this.showChoiceAtLine = choiceLines;
        this.doActionAtLine = actionLines;
        
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
        // Debug.Log("User input: " + userInput);

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        if (userInput == "123")
        {
            typingCoroutine = StartCoroutine(TypeDialogue("Senha correta!"));
            OnPasswordCorrect?.Invoke();
            ChangeDialogue();
        }
        else
        {
            typingCoroutine = StartCoroutine(TypeDialogue("Senha incorreta. Verifique o puzzle e tente novamente."));
        }
    }

    private NPCController GetCurrentNPC()
    {
        return FindObjectOfType<NPCController>();
    }

    private void ChangeDialogue()
    {
        NPCController currentNPC = GetCurrentNPC();
        if (currentNPC != null)
        {
            currentNPC.ChangeToPuzzleCompleteDialogue();
            // Salva o estado de "diálogo de puzzle completo". Assim, mesmo trocando
            // de cena, o diálogo correto será exibido ao interagir com o NPC novamente
            SceneObjectsManager.Instance.SetObjectDialogueState(
                SceneManager.GetActiveScene().name,
                currentNPC.gameObject.name,
                false
            );
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
        int answer = cubePuzzleAnswers[currentCubePuzzleIndex];
        // Debug.Log("Choice selected: " + chosenText);
        // Debug.Log("Choice index: " + choiceIndex);
        // Debug.Log("Correct answer: " + cubePuzzleAnswers[currentCubePuzzleIndex]);

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        if (choiceIndex == answer)
        {
            typingCoroutine = StartCoroutine(TypeDialogue("Correto!"));
        }
        else
        {
            typingCoroutine = StartCoroutine(TypeDialogue("Incorreto. Verifique novamente as cores."));
            currentLine = 100; // Vai pro fim do diálogo
        }

        // Completou o puzzle
        if (choiceIndex == answer && currentCubePuzzleIndex == cubePuzzleAnswers.Count - 1)
        {
            OnCubePuzzleComplete?.Invoke();
            ChangeDialogue();
        }

        currentCubePuzzleIndex++;
        dialogueBox.SetActive(true);
    }

    public void RunAction(string actionName){
        switch(actionName) 
        {
        case "PlayerWalkTutorial":
            isActionLoop = true;
            PlayerController playerController = FindObjectOfType<PlayerController>();
            bool playerIsMoving = playerController.handleAutomaticMove(3, playerWalkTutorialFirstTime);
            if(playerWalkTutorialFirstTime){
                playerWalkTutorialFirstTime = false;
            }
            if(!playerIsMoving){
                isActionLoop = false;
                this.HandleUpdate(true);
            }
            break;
        case "ColorOnTutorial":
            isActionLoop = true;
            if (Input.GetKeyDown(KeyCode.Q)){
                isActionLoop = false;
                this.HandleUpdate(true);
            }
            break;
        case "ColorSwitchTutorial":
            isActionLoop = true;
            if (Input.GetKeyDown(KeyCode.E)){
                isActionLoop = false;
                this.HandleUpdate(true);
            }
            break;
        case "GetGreenTotemTutorial":
            InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
            inventoryManager.AddItem("GreenTotem");
            break;
        case "EndTutorial":
            ChangeDialogue();
            break;
        default:
            Debug.Log("Ação não reconhecida");
            break;
        }
    }
}
