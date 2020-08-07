using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altruist : Trader, ITrade
{
    private void Start()
    {
        NumberOfPlannedDeals = (byte)Random.Range(5, 11);
        if (Type != TraderType.altruist)
            Debug.LogError("Несовпадение типов");
    }

    public void SelectTheTypeOfDeal()
    {
        Strategy = TraderStrategy.Honest;
        MayChangeMyMind();
    }
}
