using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vindictive : Trader, ITrade, IConclude
{
    bool WasItCheated;

    private void OnEnable()
    {
        GM.StartingPlacement += MoveHome;
        GM.StartingPlacement += LookAtHome;
        GM.ViewCenter += LookAtCenter;
        GM.MoveToTrade += MoveToTheMarket;
        GM.ResetTheResultsForTheYear += PrepareData;
        GM.ResetTheResultsForTheYear += ResetData;
    }

    private void Start()
    {
        NumberOfPlannedDeals = (byte)Random.Range(5, 11);
        if (Type != TraderType.vindictive)
            Debug.LogError("Несовпадение типов");
    }

    public void SelectTheTypeOfDeal()
    {
        if(WasItCheated == false) Strategy = TraderStrategy.Honest;
        else Strategy = TraderStrategy.Fraud;

        MayChangeMyMind();
    }

    public void DrawConclusionsForYourself()
    {
        if (Companion.GetComponent<Trader>().Strategy == TraderStrategy.Fraud) WasItCheated = true;
    }

    void ResetData()
    {
        WasItCheated = false;
    }

    private void OnDisable()
    {
        GM.StartingPlacement -= MoveHome;
        GM.StartingPlacement -= LookAtHome;
        GM.ViewCenter -= LookAtCenter;
        GM.MoveToTrade -= MoveToTheMarket;
        GM.ResetTheResultsForTheYear -= PrepareData;
        GM.ResetTheResultsForTheYear -= ResetData;
    }
}
