using System.Collections;
using System.Collections.Generic;
using Cainos.PixelArtTopDown_Basic;
using UnityEngine;
using TMPro;


public class GameLogic : MonoBehaviour
{
    public List<string> Pillars;
    public List<GameObject> Glow;
    private int Score = 0;
    public string Selected_Pillar;
    private int pillarAdd;
    public float selectionWindow = 2f;
    private bool canSelect = false;
    private Coroutine ActionWindow;
    private SpriteRenderer SR;
    private Queue<float> reactionTimeQueue;
    private const int Queue_Size =10;
    private float roundStartTime;
    private float lastReactionTime,Accuracy;
    private int WrongClicks=0,ClickCount=0;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text wrongClicksText;
    [SerializeField] private TMP_Text AccuracyText;
    private OddRule currentRule;
    enum OddRule
    {
        GlowOnly,RotationOnly
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
        canSelect = true;
        roundStartTime =Time.time;
        float elapsedTime=0f;
        while (elapsedTime < duration)
        {
            elapsedTime +=Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / duration);
            Color c = SR.color;
            c.a = Mathf.Lerp(0f,1f,normalizedTime);
            SR.color = c;
            yield return null;
        }   
        if(canSelect)
        {
            canSelect=false;
            Score--;
            WrongClicks++;
            AddReactionTime(selectionWindow);
            AdjustDifficulty();
            SetColor();
            SelectPillar();
        }

    }
    void SelectPillar()
{
    pillarAdd = Random.Range(0, Pillars.Count);
    Selected_Pillar = Pillars[pillarAdd];
    currentRule = (OddRule)Random.Range(0, 2);
    

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
        Debug.Log(Player_Selected);
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
            c.a = 0f;
            sr.color = c;
            Glow[i].GetComponent<SpriteColorAnimation>().enabled = false;
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
            case OddRule.GlowOnly:
                ApplyGlowRule();
                break;

            case OddRule.RotationOnly:
                ApplyRotationRule();
                break;
        }
    }
    void AdjustDifficulty()
    {
        float avgReaction = GetAverageReactionTime();

        float normalized = Mathf.Clamp01(avgReaction / 2f);

        float targetWindow = Mathf.Lerp(2f, 0.6f, normalized);

        selectionWindow = Mathf.Lerp(selectionWindow, targetWindow, 0.1f);
    }


    void ApplyGlowRule()
    {
        SR = Glow[pillarAdd].GetComponent<SpriteRenderer>();
    }
    void ApplyRotationRule()
    {
        Glow[pillarAdd].transform.rotation = Quaternion.identity;
        Glow[pillarAdd].transform.rotation = Quaternion.Euler(0f, 0f, 45f);
        SR = Glow[pillarAdd].GetComponent<SpriteRenderer>();
    }
}
