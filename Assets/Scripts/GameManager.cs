using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Trader;

public class GameManager : MonoBehaviour
{
    public GameObject NpcCat;                               // Категория с торговцами
    public UIScript UIS;
    public List<Trader> ActiveTraders = new List<Trader>(); // Список торговцев у которых ещё остались очки торговли
    public int Year = 0;
    public Trader ActiveTrader;
    public delegate void Standart();
    public event Standart StartingPlacement;                // Первоначальное размещение торговцев по домам
    public event Standart ViewCenter;                       // Все смотрят в центр
    public event Standart MoveToTrade;                      // Все выходят из дома
    public event Standart ResetTheResultsForTheYear;
    float Percent = 20;                                     // Процент торговцев который мы будем заменять каждый год
    public int PercentageOfTraders;                         // Количество торговцев уоторе надо заменить
    Trader[] Winners;
    Trader[] Losers;

    void Start()
    {
        PercentageOfTraders = Mathf.RoundToInt(((float)NpcCat.transform.childCount / 100) * Percent);
        UIS.CreateAnEmptyListOfWinners();
        StartCoroutine(StartingProcedure(2f));          // Отсчитываем время до старта
    }

    // Отсчитывает время и вызывает событие первоначальной расстановки торговцев
    public IEnumerator StartingProcedure(float Time)    
    {
        yield return new WaitForSeconds(Time);
        StartingPlacement();
        yield return new WaitForSeconds(2);
        ViewCenter();
        yield return new WaitForSeconds(2f);
        FindAllTraders();
        StartCoroutine(TradeAnotherYear());
    }

    // Используется для запуска очередного года 
    public IEnumerator TradeAnotherYear()
    {
        Year++;
        Debug.Log("Начинаем " + Year +  " год ");
        ResetTheResultsForTheYear();
        yield return new WaitForSeconds(1f);
        MoveToTrade();
        yield return new WaitForSeconds(3f);
        CallAnotherMerchant();
    }


    public void EndTheYear()
    {
        FindAllTraders();
        ReplaceTheLosers();
        UIS.PopulateTheTableWithResults(Winners);
        UIS.Panels[1].gameObject.SetActive(true);
    }

    // Метод помещает всех торговцев в список торговцев готовых торговать
    void FindAllTraders()
    {
        for (int i = 0; NpcCat.transform.childCount > ActiveTraders.Count ; i++)
        {
            ActiveTraders.Add(NpcCat.transform.GetChild(i).GetComponent<Trader>());
        }
    }

    // Вызывает для торговли случайного торговца у которого есть очки торговли 
    public void CallAnotherMerchant()
    {
        int RandomTrader = 0;

        RandomTrader = Random.Range(0, ActiveTraders.Count);
        ActiveTrader = ActiveTraders[RandomTrader];
        StartCoroutine(ActiveTraders[RandomTrader].Trade());

    }

    // В конце года метод заменяет самых неуспешных торговцев самыми успешными
    void ReplaceTheLosers()
    {
        Trader CreatedMerchant;

        Winners = new Trader[PercentageOfTraders];
        Losers = new Trader[PercentageOfTraders];

        CalculateTheMostSuccessful(Winners);
        CalculateTheMostUnsuccessful(Losers);

        for (int a = 0; a < PercentageOfTraders ; a++)
        {
            CreatedMerchant = Instantiate(Winners[a], Losers[a].transform.position, Losers[a].transform.rotation, NpcCat.transform);
            CreatedMerchant.name = DetermineTheNameOfTheMerchant(CreatedMerchant, Losers[a]);
            CreatedMerchant.home = Losers[a].home;
            ActiveTraders.Add(CreatedMerchant);

            for (int b = 0; b < ActiveTraders.Count; b++)
            {
                if (Losers[a] == ActiveTraders[b])
                {
                    ActiveTraders.RemoveAt(b);
                    Destroy(Losers[a].gameObject);
                }
            }
        }
    }

    // Метод вычисляет самых успешных торговцев
    void CalculateTheMostSuccessful(Trader[] winners)
    {
        int TheMostUnsuccessful = 0;                    // Номер наименее успешного торговца в списке победителей

        for (int a = 0; a < ActiveTraders.Count; a++)
        {
            if (winners[winners.Length - 1] == null)
            {
                for (int b = 0; b < winners.Length; b++)
                {
                    if (winners[b] == null)
                    {
                        winners[b] = ActiveTraders[a];
                        break;
                    }
                }
            }
            else if (winners[winners.Length - 1] != null)
            {
                for (int b = 0; b < winners.Length; b++)
                {
                    if (b == 0)
                        TheMostUnsuccessful = 0;
                    else
                    {
                        if (winners[b].AnnualProfit < winners[TheMostUnsuccessful].AnnualProfit)
                            TheMostUnsuccessful = b;
                    }
                }

                if (winners[TheMostUnsuccessful].AnnualProfit < ActiveTraders[a].AnnualProfit)
                {
                    winners[TheMostUnsuccessful] = ActiveTraders[a];
                }
            }
        }
    }

    // Метод вычисляет самых неуспешных торговцев
    void CalculateTheMostUnsuccessful(Trader[] losers)
    {
        int TheMostSuccessful = 0;                      // Номер наиболее успешного торговца в списке неудачников

        for (int a = 0; a < ActiveTraders.Count; a++)
        {
            if (losers[losers.Length - 1] == null)
            {
                for (int b = 0; b < losers.Length; b++)
                {
                    if (losers[b] == null)
                    {
                        losers[b] = ActiveTraders[a];
                        break;
                    }
                }
            }
            else if (losers[losers.Length - 1] != null)
            {
                for (int b = 0; b < losers.Length; b++)
                {
                    if (b == 0)
                        TheMostSuccessful = 0;
                    else
                    {
                        if (losers[b].AnnualProfit > losers[TheMostSuccessful].AnnualProfit)
                            TheMostSuccessful = b;
                    }
                }

                if (losers[TheMostSuccessful].AnnualProfit > ActiveTraders[a].AnnualProfit)
                    losers[TheMostSuccessful] = ActiveTraders[a];
            }
        }
    }

    // Определяет имя созданного торговца
    string DetermineTheNameOfTheMerchant(Trader Substitute, Trader Replaceable)
    {
        char[] Symbols;                         // Имя заменяемого торговца разбитое на символы
        List<int> nombers = new List<int>();    // Числа извлечённые из имени торговца
        TraderType type = Substitute.Type;
        string TraderNombers = "";
        string TraderName = "";

        switch (type)
        {
            case TraderType.altruist:
                TraderName = "TraderAltruist";
                break;
            case TraderType.thrower:
                TraderName = "TraderThrower";
                break;
            case TraderType.sly:
                TraderName = "TraderSly";
                break;
            case TraderType.unpredictable:
                TraderName = "TraderUnpredictable";
                break;
            case TraderType.vindictive:
                TraderName = "TraderVindictive";
                break;
            case TraderType.quirky:
                TraderName = "TraderQuirky";
                break;
            case TraderType.calculating:
                TraderName = "TraderCalculating"; 
                break;
        }

        string Symbol;
        Symbols = Replaceable.name.ToCharArray();

        for (int a = 0; a < Symbols.Length; a++)
        {
            Symbol = Symbols[a].ToString();
            if (int.TryParse(Symbol, out int nomber))
            {
                nombers.Add(nomber);
            }
        }

        for (int i = 0; i < nombers.Count; i++)
            TraderNombers += nombers[i].ToString();

        TraderName = TraderName + " (" + TraderNombers + ")";
        return TraderName;
    }
    // ----------------------------------------------------------------------------------------------- События --------------------------------------------------------------------------------------------------


}
