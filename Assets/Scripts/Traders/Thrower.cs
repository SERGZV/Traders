using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thrower : Trader, ITrade
{
    private void Start()
    {
        NumberOfPlannedDeals = (byte)Random.Range(5, 11);
        if (Type != TraderType.thrower)
            Debug.LogError("Несовпадение типов");
    }

    public void SelectTheTypeOfDeal()
    {
        Strategy = TraderStrategy.Fraud;
        MayChangeMyMind();
    }
}
