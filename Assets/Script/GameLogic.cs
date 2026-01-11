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
    public float speed;
    private int pillarAdd;
    public float selectionWindow = 0f;
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

    void Start()
    {
        SetColor();
        SelectPillar();
        ControlingCoroutine(selectionWindow);
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
            SetColor();
            SelectPillar();
        }

    }
    void SelectPillar()
    {
        pillarAdd = Random.Range(0, Pillars.Count);
        Selected_Pillar = Pillars[pillarAdd];
        Debug.Log(Selected_Pillar);
        SR = Glow[pillarAdd].GetComponent<SpriteRenderer>();
        ControlingCoroutine(selectionWindow);
    }

    public void PlayerSeletion(string Player_Selected)
    {
        if (!canSelect) 
        {
            return;
        }
        canSelect = false;
        lastReactionTime = Time.time - roundStartTime;
        AddReactionTime(lastReactionTime);
        
        CheckSelection(Player_Selected);
    }


    void CheckSelection( string Player_Selected)
    {
        if(Player_Selected == Selected_Pillar)
        {
            Score++;
        }
        else
        {
            Score--;
            WrongClicks++;
        }
        ClickCount++;
        Accuracy=(ClickCount-WrongClicks)/ClickCount;
        Debug.Log("Player Selected" + Player_Selected);
        Debug.Log("Player Score" +Score);
        float avgReaction = GetAverageReactionTime();
        float targetWindow = Mathf.Lerp(1.8f,0.6f,avgReaction/selectionWindow);
        selectionWindow = Mathf.Clamp(selectionWindow, 0.5f, 2.5f);
        selectionWindow = Mathf.Lerp(selectionWindow , targetWindow,0.3f);
        SetColor();
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


}
