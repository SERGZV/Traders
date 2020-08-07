using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quirky : Trader, ITrade, IConclude
{
    bool WasItCheated;
    TraderStrategy LastCompanion;

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
        if (Type != TraderType.quirky)
            Debug.LogError("Несовпадение типов");
    }

    public void SelectTheTypeOfDeal()
    {
        switch (DealNumber)
        {
            case 0:
                Strategy = TraderStrategy.Honest;
                break;
            case 1:
                Strategy = TraderStrategy.Fraud;
                break;
            case 2:
                Strategy = TraderStrategy.Honest;
                break;
            case 3:
                Strategy = TraderStrategy.Honest;
                break;
            default:
                HelperMethod();
                break;
        }

        MayChangeMyMind();
    }

    void HelperMethod()
    {
        if (!WasItCheated)                      // Если к 5 ходу его не разу не обманули играет как хитрец
            Strategy = LastCompanion;
        else
            Strategy = TraderStrategy.Fraud;   // Если к 5 ходу его обманули играет как кидала


    }

    public void DrawConclusionsForYourself()
    {
        LastCompanion = Companion.GetComponent<Trader>().Strategy;

        if (LastCompanion == TraderStrategy.Fraud)
            WasItCheated = true;
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
