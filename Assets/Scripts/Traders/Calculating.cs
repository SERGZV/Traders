using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculating : Trader, ITrade
{   // Свой класс "Расчётливый". Торгует честно только с альтруистом остальных обманывает без зазрения совести.
    private void Start()
    {
        NumberOfPlannedDeals = (byte)Random.Range(5, 11);
        if (Type != TraderType.calculating)
            Debug.LogError("Несовпадение типов");
    }

    public void SelectTheTypeOfDeal()
    {
        if (Companion.GetComponent<Trader>().Type == TraderType.altruist) Strategy = TraderStrategy.Honest;
        else Strategy = TraderStrategy.Fraud;

        MayChangeMyMind();
    }
}
