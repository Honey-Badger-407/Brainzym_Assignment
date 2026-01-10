using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cainos.PixelArtTopDown_Basic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameLogic : MonoBehaviour
{
    public List<string> Pillars;
    public List<GameObject> Glow;
    private int Score = 0;
    public string Selected_Pillar;
    private float Alpha = 0f;
    public float speed;
    private int pillarAdd;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetColor();
    }

    // Update is called once per frame
    void Update()
    {
        SelectPillar();
        while(Alpha < 254)
        {
            Alpha += speed * Time.deltaTime;
            SpriteRenderer SR;
            SR = Glow[pillarAdd].GetComponent<SpriteRenderer>();
            Color c = SR.color;
            c.a = Alpha;
            SR.color = c;
            Glow[pillarAdd].GetComponent<SpriteColorAnimation>().enabled = true;
        }
    }
    void SelectPillar()
    {
        pillarAdd = UnityEngine.Random.Range(0,Pillars.Count);
        Selected_Pillar = Pillars[pillarAdd];
        Debug.Log(Selected_Pillar);
    }
    public void PlayerSeletion(string Player_Selected)
    {
        Debug.Log(Player_Selected);
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
        }
        Debug.Log(Score);
        SelectPillar();
    }
    void SetColor()
    {
        SpriteRenderer SR;
        for(int i = 0;i<9;i++)
        {
            SR = Glow[i].GetComponent<SpriteRenderer>();
            Color c = SR.color;
            c.a = 0f;
            SR.color = c;
            Glow[i].GetComponent<SpriteColorAnimation>().enabled = false;
        }
    }
}
