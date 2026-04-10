using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Base gameGraph;
    [SerializeField] private string startNodeName = "Start";
    
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text outcomeMessageText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text peopleText;
    [SerializeField] private TMP_Text armyText;
    [SerializeField] private TMP_Text churchText;
    [SerializeField] private Button undoButton;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverText;
    
    [SerializeField] private float swipeThreshold = 50f;
    
    [SerializeField] private float messageDisplayTime = 2f;
    
    private DecisionNode currentNode;
    private int gold = 50;
    private int people = 50;
    private int army = 50;
    private int church = 50;
    
    private Stack<GameState> history = new Stack<GameState>();
    
    private Vector2 startTouchPos;
    private bool isSwiping = false;
    private Coroutine messageCoroutine;
    
    [System.Serializable]
    public class GameState
    {
        public DecisionNode node;
        public int gold;
        public int people;
        public int army;
        public int church;
        
        public GameState(DecisionNode node, int gold, int people, int army, int church)
        {
            this.node = node;
            this.gold = gold;
            this.people = people;
            this.army = army;
            this.church = church;
        }
    }
    
    void Start()
    {
        InitializeGame();
        undoButton.onClick.AddListener(UndoLastMove);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (outcomeMessageText != null)
            outcomeMessageText.gameObject.SetActive(false);
    }
    
    void InitializeGame()
    {
        if (gameGraph != null)
        {
            foreach (var node in gameGraph.nodes)
            {
                if (node is DecisionNode decisionNode)
                {
                    if (decisionNode.question == startNodeName)
                    {
                        currentNode = decisionNode;
                        break;
                    }
                }
            }
            
            if (currentNode == null)
            {
                foreach (var node in gameGraph.nodes)
                {
                    if (node is DecisionNode decisionNode)
                    {
                        currentNode = decisionNode;
                        break;
                    }
                }
            }
        }
        
        UpdateUI();
        ShowCurrentCard();
    }
    
    void Update()
    {
        HandleSwipeInput();
    }
    
    void HandleSwipeInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPos = Input.mousePosition;
            isSwiping = true;
        }
        
        if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            Vector2 endPos = Input.mousePosition;
            float deltaX = endPos.x - startTouchPos.x;
            
            if (Mathf.Abs(deltaX) > swipeThreshold)
            {
                bool isLeftSwipe = deltaX < 0;
                MakeChoice(isLeftSwipe);
            }
            
            isSwiping = false;
        }
    }
    
    public void MakeChoice(bool isLeftSwipe)
    {
        if (currentNode == null) return;
        
        history.Push(new GameState(currentNode, gold, people, army, church));
        
        BaseNode nextNode = currentNode.GetNextNode(isLeftSwipe);
        
        if (nextNode is OutcomeNode outcome)
        {
            outcome.Execute(this);
            BaseNode afterOutcome = outcome.GetNextNode();
            
            if (afterOutcome is DecisionNode nextDecision)
            {
                currentNode = nextDecision;
            }
            else if (afterOutcome == null)
            {
                GameOver("The story ends...");
                return;
            }
        }
        else if (nextNode is DecisionNode nextDecision)
        {
            currentNode = nextDecision;
        }
        else if (nextNode == null)
        {
            GameOver("Your journey ends here...");
            return;
        }
        
        CheckGameOver();
        UpdateUI();
        ShowCurrentCard();
    }
    
    public void ModifyResources(int goldDelta, int peopleDelta, int armyDelta, int churchDelta)
    {
        gold += goldDelta;
        people += peopleDelta;
        army += armyDelta;
        church += churchDelta;
        
        gold = Mathf.Clamp(gold, 0, 100);
        people = Mathf.Clamp(people, 0, 100);
        army = Mathf.Clamp(army, 0, 100);
        church = Mathf.Clamp(church, 0, 100);
    }
    
    public void UndoLastMove()
    {
        if (history.Count == 0) return;
        
        GameState lastState = history.Pop();
        currentNode = lastState.node;
        gold = lastState.gold;
        people = lastState.people;
        army = lastState.army;
        church = lastState.church;
        
        UpdateUI();
        ShowCurrentCard();
        
        if (outcomeMessageText != null)
            outcomeMessageText.gameObject.SetActive(false);
    }
    
    public void ShowOutcomeMessage(string message)
    {
        if (messageCoroutine != null)
            StopCoroutine(messageCoroutine);
        
        if (outcomeMessageText != null)
        {
            outcomeMessageText.text = message;
            outcomeMessageText.gameObject.SetActive(true);
            messageCoroutine = StartCoroutine(HideMessageAfterDelay());
        }
    }
    
    private IEnumerator HideMessageAfterDelay()
    {
        yield return new WaitForSeconds(messageDisplayTime);
        
        if (outcomeMessageText != null)
        {
            outcomeMessageText.gameObject.SetActive(false);
        }
    }
    
    void CheckGameOver()
    {
        if (gold <= 0) GameOver("You have no gold left!\nYour kingdom falls into ruin...");
        else if (people <= 0) GameOver("Your people have abandoned you!\nThe throne is empty...");
        else if (army <= 0) GameOver("Your army has been defeated!\nInvaders take over...");
        else if (church <= 0) GameOver("The church has condemned you!\nYou are excommunicated...");
        else if (gold >= 100) GameOver("You have unlimited wealth!\nBut money can't buy happiness...");
        else if (people >= 100) GameOver("The kingdom is overpopulated!\nFamine strikes...");
        else if (army >= 100) GameOver("Your army is too powerful!\nThey overthrow you...");
        else if (church >= 100) GameOver("The church controls everything!\nYou become a puppet ruler...");
    }
    
    void GameOver(string reason)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (gameOverText != null)
                gameOverText.text = reason;
        }
        enabled = false;
    }
    
    void UpdateUI()
    {
        if (goldText != null) goldText.text = $"💰 {gold}";
        if (peopleText != null) peopleText.text = $"👥 {people}";
        if (armyText != null) armyText.text = $"⚔️ {army}";
        if (churchText != null) churchText.text = $"⛪ {church}";
    }
    
    void ShowCurrentCard()
    {
        if (questionText != null && currentNode != null)
        {
            questionText.text = currentNode.question;
        }
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}