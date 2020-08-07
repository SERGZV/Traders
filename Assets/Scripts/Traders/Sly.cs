using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sly : Trader, ITrade, IConclude
{
    TraderStrategy LastCompanionStrategy;

    private void Start()
    {
        NumberOfPlannedDeals = (byte)Random.Range(5, 11);
        if (Type != TraderType.sly)
            Debug.LogError("Несовпадение типов");
    }

    public void SelectTheTypeOfDeal()
    {
        if (DealNumber == 0)
            Strategy = TraderStrategy.Honest;
        else
            Strategy = LastCompanionStrategy;

        MayChangeMyMind();
    }

    public void DrawConclusionsForYourself()
    {
        LastCompanionStrategy = Companion.GetComponent<Trader>().Strategy;
    }
}
