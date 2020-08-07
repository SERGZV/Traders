using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unpredictable : Trader, ITrade
{
    byte _strategy;
    private void Start()
    {
        NumberOfPlannedDeals = (byte)Random.Range(5, 11);
        if (Type != TraderType.unpredictable)
            Debug.LogError("Несовпадение типов");
    }

    public void SelectTheTypeOfDeal()
    {
        _strategy = (byte)Random.Range(0, 2);

        if (_strategy == 0) Strategy = TraderStrategy.Honest;
        else Strategy = TraderStrategy.Fraud;

        MayChangeMyMind();
    }
}
