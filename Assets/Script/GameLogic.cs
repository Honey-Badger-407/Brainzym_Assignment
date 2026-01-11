using System.Collections;
using System.Collections.Generic;
using Cainos.PixelArtTopDown_Basic;
using UnityEngine;
using TMPro;
using Unity.Mathematics;


public class GameLogic : MonoBehaviour
{
    public List<string> Pillars;
    public List<GameObject> PillarsObjects;
    public List<GameObject> Glow;
    private int Score = 0;
    public string Selected_Pillar;
    private int pillarAdd;
    public float selectionWindow = 4f;
    private bool canSelect = false;
    private Coroutine ActionWindow;
    private Queue<float> reactionTimeQueue;
    private const int Queue_Size =10;
    private float roundStartTime;
    private float lastReactionTime,Accuracy,MaxWindowSize = 4f,MinWindowSize=0.16f;
    private int WrongClicks=0,ClickCount=0;
    private Vector3 obj,RandomPosition;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text wrongClicksText;
    [SerializeField] private TMP_Text AccuracyText;
    private OddRule currentRule;
    enum OddRule
    {
        RotatePillar,movePillar
    }
    void Start()
    {
        reactionTimeQueue = new Queue<float>();
        SetColor();
        SelectPillar();
    }
    public void ControlingCoroutine(float duration)
    {
        if(ActionWindow!=null)
        {
            StopCoroutine(ActionWindow);
        }
        ActionWindow = StartCoroutine(SelectionWindow(duration));
    }
    IEnumerator SelectionWindow(float duration)
    {
        Debug.Log(duration);
        canSelect = true;
        roundStartTime =Time.time;
        float elapsedTime=0f;
        while (elapsedTime < duration)
        {
            elapsedTime +=Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / duration);
            for(int i=0;i<Glow.Count;i++)
            {
                SpriteRenderer sr;
                if(Glow[i]==Glow[pillarAdd])
                    continue;

                normalizedTime = Mathf.Clamp01(elapsedTime / duration);
                sr = Glow[i].GetComponent<SpriteRenderer>();
                Color c = sr.color;
                c.a = math.lerp(1f,0f,normalizedTime);
                sr.color = c;
                Glow[i].GetComponent<SpriteColorAnimation>().enabled = false;
            }
            switch (currentRule)
            {
                case OddRule.RotatePillar:
                {
                    Quaternion startRotation = Quaternion.Euler(0f,0f,0f);
                    Quaternion targetRotation = Quaternion.Euler(0f, 0f, 90f);
                    PillarsObjects[pillarAdd].transform.rotation = Quaternion.Lerp(startRotation, targetRotation, normalizedTime);
                    break;
                }                
                case OddRule.movePillar:
                    PillarsObjects[pillarAdd].transform.position = Vector3.Lerp(obj, RandomPosition, normalizedTime);
                    break;
            }
                yield return null;
        }   
        if(canSelect)
        {
            canSelect=false;
            Score--;
            WrongClicks++;
            AddReactionTime(selectionWindow);
            AdjustDifficulty();
            Reset();
            SetColor();
            SelectPillar();
        }

    }
    void SelectPillar()
{
    pillarAdd = UnityEngine.Random.Range(0, Pillars.Count);
    Selected_Pillar = Pillars[pillarAdd];
    currentRule = (OddRule)UnityEngine.Random.Range(0, 2);
    
    obj = PillarsObjects[pillarAdd].transform.position;
    ApplyRule();                     

    UpdateUI();
    ControlingCoroutine(selectionWindow);
}


    public void PlayerSeletion(string Player_Selected)
    {
        Debug.Log("Player "+Player_Selected);
        if (!canSelect) 
        {
            return;
        }
        canSelect = false;
        lastReactionTime = Time.time - roundStartTime;
        AddReactionTime(lastReactionTime);
        Reset();
        CheckSelection(Player_Selected);
    }


    void CheckSelection(string Player_Selected)
    {
        ClickCount++;

        if (Player_Selected == Selected_Pillar)
        {
            float normalized = Mathf.Clamp01(lastReactionTime / selectionWindow);
            int roundScore = Mathf.RoundToInt(Mathf.Lerp(100, 20, normalized));
            Score += roundScore;
        }
        else
        {
            Score -= 30;
            WrongClicks++;
        }

        Accuracy = ((float)(ClickCount - WrongClicks) / ClickCount) * 100f;

        AdjustDifficulty();
        SelectPillar();
    }

    void SetColor()
    {
        SpriteRenderer sr;
        for (int i = 0; i < Glow.Count; i++)
        {
            sr = Glow[i].GetComponent<SpriteRenderer>();
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;
            Glow[i].GetComponent<SpriteColorAnimation>().enabled = true;
        }
    }
    void AddReactionTime(float reactionTime)
    {
        if (reactionTimeQueue.Count >= Queue_Size)
        {
            reactionTimeQueue.Dequeue();
        }

        reactionTimeQueue.Enqueue(reactionTime);
    }
    float GetAverageReactionTime()
    {
        if (reactionTimeQueue.Count == 0)
            return selectionWindow;

        float sum = 0f;
        foreach (float QueueMembers in reactionTimeQueue)
            sum += QueueMembers;

        return sum / reactionTimeQueue.Count;
    }
    void UpdateUI()
    {
        scoreText.text = "Score: " + Score;
        wrongClicksText.text = "Wrong: " + WrongClicks;
        AccuracyText.text = $"Accuracy: {Accuracy:F1}%";
    }

    void ApplyRule()
    {
        SetColor(); // reset visuals first

        switch (currentRule)
        {
            case OddRule.RotatePillar:
                Applyrotation();
                break;
            case OddRule.movePillar:
                ApplyMovement();
                break;
        }
    }
    void AdjustDifficulty()
    {
        float avgReaction = GetAverageReactionTime();

        float normalized = Mathf.Clamp01(avgReaction / MaxWindowSize);

        float targetWindow = Mathf.Lerp(MinWindowSize, MaxWindowSize, normalized);

        selectionWindow = Mathf.Lerp(selectionWindow, targetWindow, 0.5f);
    }


    void Applyrotation()
    {
    PillarsObjects[pillarAdd].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }
    void ApplyMovement()
    {
        RandomPosition = new Vector3(
            UnityEngine.Random.Range(obj.x-10, obj.x+10),
            UnityEngine.Random.Range(obj.y-10,obj.y+10 ),0f);
        
    }
    void Reset()
    {
        PillarsObjects[pillarAdd].transform.rotation=Quaternion.Euler(0f,0f,0f);
        PillarsObjects[pillarAdd].transform.position= obj;
    }
}
