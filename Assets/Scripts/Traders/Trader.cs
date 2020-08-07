using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trader : MonoBehaviour, IMovable, IRotatable
{
    public GameManager GM;
    public Home home;                                   // Дом к которому привязан торговец
    Vector3 Target;
    Vector3 TraderHight = new Vector3 (0, 1, 0);
    public enum TraderType { altruist, thrower, sly, unpredictable, vindictive, quirky, calculating }    // Альтруист, Кидала, Хитрец, Непредсказуемый, Злопамятный, Ушлый, Расчётливый(свой тип)
    public TraderType Type;
    public enum TraderStrategy { Unspecified, Honest, Fraud }
    public TraderStrategy Strategy;
    public GameObject Companion;                        // Текущий компаньон для торговли
   
    float MovingSpeed = 2;
    float RotationSpeed = 4;
    public byte NumberOfPlannedDeals = 0;               // Количество очков запланированных сделок (тип байт используется специально для обнаружения ошибки выхода значения в минус)
    public int DealNumber;
    public int AnnualProfit;

    bool moveHome = false;
    bool moveToTheMarket = false;
    bool moveToTheCenter = false;
    bool moveToCompanion = false;
    bool lookAtHome = false;
    bool lookAtCenter = false;
    bool lookAtCompanion = false;

    private void OnEnable()
    {
        GM.StartingPlacement += MoveHome;
        GM.StartingPlacement += LookAtHome;
        GM.ViewCenter += LookAtCenter;
        GM.MoveToTrade += MoveToTheMarket;
        GM.ResetTheResultsForTheYear += PrepareData;
    }


    void Update()
    {
        if (moveHome == true) MoveHome();               
        if (moveToTheMarket == true) MoveToTheMarket();
        if (moveToTheCenter == true) MoveToTheCenter();
        if (moveToCompanion == true) MoveToCompanion();
        if (lookAtHome == true) LookAtHome();           
        if (lookAtCenter == true) LookAtCenter();
        if (lookAtCompanion == true) LookAtСompanion();
    }
// -------------------------------------------------------------------------------------------- Методы движения -----------------------------------------------------------------------------------------------
    // Заставляем торговца двигатся к дому
    public void MoveHome()
    {
        if(!moveHome) moveHome = true;

        if (Vector3.Distance(home.House, transform.position) > 1.1f)
        {
            Vector3 direction = home.House - transform.position + TraderHight;
            transform.Translate(direction * MovingSpeed * Time.deltaTime, relativeTo: Space.World);
        }
        else moveHome = false;
    }

    // В качестве рынка выступает центр карты
    public void MoveToTheMarket()
    {
        if (!moveToTheMarket) moveToTheMarket = true;

        if (Vector3.Distance(home.NextToTheHouse, transform.position) > 1.1f)
        {
            Vector3 direction = home.NextToTheHouse - transform.position + TraderHight;
            transform.Translate(direction * MovingSpeed * Time.deltaTime, relativeTo: Space.World);
        }
        else moveToTheMarket = false;
    }

    // Двигаем торговца в центр
    public void MoveToTheCenter()
    {
        if (!moveToTheCenter) moveToTheCenter = true;

        if (Vector3.Distance(Vector3.zero, transform.position) > 1.1f)
        {
            Vector3 direction = Vector3.zero - transform.position + TraderHight;
            transform.Translate(direction * MovingSpeed * Time.deltaTime, relativeTo: Space.World);
        }
        else moveToTheCenter = false;
    }


    public void MoveToCompanion()
    {
        if (!moveToCompanion) moveToCompanion = true;

        if (Vector3.Distance(Companion.transform.position + (Companion.transform.forward * 3), transform.position) > 1.1f)
        {
            Vector3 direction = Companion.transform.position + (Companion.transform.forward * 3) - transform.position;
            transform.Translate(direction * MovingSpeed * Time.deltaTime, relativeTo: Space.World);
        }
        else moveToCompanion = false;
    }
// ----------------------------------------------------------------------------------------------- Методы поворота ------------------------------------------------------------------------------------------
  
    // Застваляем торговца смотреть на дом
    public void LookAtHome()
    {
        if (lookAtHome == false)
        {
            Target = home.House;
            lookAtHome = true;
        }

        if (Vector3.Distance(home.House, transform.position) > 1.1f)
        {
            Vector3 direction = Target - transform.position + TraderHight;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, RotationSpeed * Time.deltaTime);
        }
        else lookAtHome = false;
    }

    // Застваляет торговца смотреть в центр
    public void LookAtCenter()
    {
        if (!lookAtCenter) lookAtCenter = true;

        Vector3 direction = Vector3.zero - transform.position + TraderHight;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, RotationSpeed * Time.deltaTime);

        float angle = Vector3.Angle(direction, transform.forward);
        if (angle <= 0.25)
            lookAtCenter = false;
    }

    // Метод поворачивает торговца к компаньону с которым он будет торговать
    public void LookAtСompanion()
    {
        if (!lookAtCompanion) lookAtCompanion = true;

        Vector3 direction = Companion.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, RotationSpeed * Time.deltaTime);
    }

    // -------------------------------------------------------------------------------------------------- Прочие методы -----------------------------------------------------------------------------------------

    // Метод поготавливает результаты торговли к следующему году
    public void PrepareData()
    {
        if (AnnualProfit > 0)
        {
            NumberOfPlannedDeals = (byte)Random.Range(5, 11);
            DealNumber = 0;
            AnnualProfit = 0;
        }
    }

    // Метод заставляет торговать активного торговца пока у него есть очки торговли
    public IEnumerator Trade()
    {
        int companionNumber = 0;

        if (GM.ActiveTraders.Count > 1)                 // Проверяем есть ли активные торговцы в начале его торговли
        {
            MoveToTheCenter();
            yield return new WaitForSeconds(1);

            for (; DealNumber < NumberOfPlannedDeals;)
            {
                if (GM.ActiveTraders.Count > 1)         // Проверяем есть ли активные торговцы перед каждой торговлей
                {
                    for (int a = 0; a < 100; a++)
                    {
                        companionNumber = Random.Range(0, GM.ActiveTraders.Count);
                        Companion = GM.ActiveTraders[companionNumber].gameObject;

                        if (transform.name != Companion.name)
                            break;
                    }

                    Trader traderComp = Companion.GetComponent<Trader>();

                    if (!lookAtCompanion)
                        LookAtСompanion();

                    yield return new WaitForSeconds(1);
                    MoveToCompanion();
                    yield return new WaitForSeconds(1);
                    ConductADeal();
                }
                else
                    break;

            }
        }
        else
            StartCoroutine(SendHomeAnActiveTrader());


    }

    // Проводит сделку с другим торговцем
    public void ConductADeal()
    {
        Trader passiveTrader = Companion.GetComponent<Trader>();
        passiveTrader.Companion = gameObject;

        switch (passiveTrader.Type)
        {
            case TraderType.altruist:
                Companion.GetComponent<Altruist>().SelectTheTypeOfDeal();
                break;
            case TraderType.thrower:
                Companion.GetComponent<Thrower>().SelectTheTypeOfDeal();
                break;
            case TraderType.sly:
                Companion.GetComponent<Sly>().SelectTheTypeOfDeal();
                break;
            case TraderType.unpredictable:
                Companion.GetComponent<Unpredictable>().SelectTheTypeOfDeal();
                break;
            case TraderType.vindictive:
                Companion.GetComponent<Vindictive>().SelectTheTypeOfDeal();
                break;
            case TraderType.quirky:
                Companion.GetComponent<Quirky>().SelectTheTypeOfDeal();
                break;
            case TraderType.calculating:
                Companion.GetComponent<Calculating>().SelectTheTypeOfDeal();
                break;
        }

        switch (Type)
        {
            case TraderType.altruist:
                GetComponent<Altruist>().SelectTheTypeOfDeal();
                break;
            case TraderType.thrower:
                GetComponent<Thrower>().SelectTheTypeOfDeal();
                break;
            case TraderType.sly:
                GetComponent<Sly>().SelectTheTypeOfDeal();
                break;
            case TraderType.unpredictable:
                GetComponent<Unpredictable>().SelectTheTypeOfDeal();
                break;
            case TraderType.vindictive:
                GetComponent<Vindictive>().SelectTheTypeOfDeal();
                break;
            case TraderType.quirky:
                GetComponent<Quirky>().SelectTheTypeOfDeal();
                break;
            case TraderType.calculating:
                GetComponent<Calculating>().SelectTheTypeOfDeal();
                break;
        }

        CalculateTradeResults(Strategy, passiveTrader.Strategy);

        switch (passiveTrader.Type)
        {
            case TraderType.sly:
                Companion.GetComponent<Sly>().DrawConclusionsForYourself();
                break;
            case TraderType.vindictive:
                Companion.GetComponent<Vindictive>().DrawConclusionsForYourself();
                break;
            case TraderType.quirky:
                Companion.GetComponent<Quirky>().DrawConclusionsForYourself();
                break;
        }

        switch (Type)
        {
            case TraderType.sly:
                GetComponent<Sly>().DrawConclusionsForYourself();
                break;
            case TraderType.vindictive:
                GetComponent<Vindictive>().DrawConclusionsForYourself();
                break;
            case TraderType.quirky:
                GetComponent<Quirky>().DrawConclusionsForYourself();
                break;
        }

        StartCoroutine(SendHomeAPassiveTrader());
        StartCoroutine(SendHomeAnActiveTrader());
    }

    // Этот метод определяет кто сколько заработал благодаря его стратегии
    public void CalculateTradeResults(TraderStrategy ThisTrader, TraderStrategy ThatTrader)
    {
        Trader passiveTrader = Companion.GetComponent<Trader>();
        if (ThisTrader == TraderStrategy.Honest && ThatTrader == TraderStrategy.Honest)
        {
            AnnualProfit += 4;
            passiveTrader.AnnualProfit += 4;
        }
        else if (ThisTrader == TraderStrategy.Fraud && ThatTrader == TraderStrategy.Fraud)
        {
            AnnualProfit += 2;
            passiveTrader.AnnualProfit += 2;
        }
        else if (ThisTrader == TraderStrategy.Honest && ThatTrader == TraderStrategy.Fraud)
        {
            AnnualProfit += 1;
            passiveTrader.AnnualProfit += 5;
        }
        else if (ThisTrader == TraderStrategy.Fraud && ThatTrader == TraderStrategy.Honest)
        {
            AnnualProfit += 5;
            passiveTrader.AnnualProfit += 1;
        }

        DealNumber++;
        passiveTrader.DealNumber++;
    }


    IEnumerator SendHomeAPassiveTrader()
    {
        Trader PassiveTrader = Companion.GetComponent<Trader>();
        if (PassiveTrader.DealNumber >= PassiveTrader.NumberOfPlannedDeals)
        {
            for (int i = 0; i < GM.ActiveTraders.Count; i++)
            {
                if (GM.ActiveTraders[i].name == PassiveTrader.name)
                {
                    GM.ActiveTraders.RemoveAt(i);
                }
            }

            PassiveTrader.LookAtHome();
            yield return new WaitForSeconds(1);
            PassiveTrader.MoveHome();
            yield return new WaitForSeconds(2);
            PassiveTrader.LookAtCenter();
        }
    }


    IEnumerator SendHomeAnActiveTrader()
    {
        if (GM.ActiveTraders.Count >= 2)
        {
            if (DealNumber >= NumberOfPlannedDeals)
            {
                lookAtCompanion = false;
                moveToCompanion = false;

                for (int i = 0; i < GM.ActiveTraders.Count; i++)
                {
                    if (GM.ActiveTraders[i].name == name)
                    {
                        GM.ActiveTrader = null;
                        GM.ActiveTraders.RemoveAt(i);
                    }
                }

                LookAtHome();
                yield return new WaitForSeconds(1);
                MoveHome();
                yield return new WaitForSeconds(2);
                LookAtCenter();

                if (GM.ActiveTrader == null && GM.ActiveTraders.Count > 0)
                {
                    GM.CallAnotherMerchant();
                }
            }
        }
        else
        {
            lookAtCompanion = false;
            moveToCompanion = false;

            for (int i = 0; i < GM.ActiveTraders.Count; i++)
            {
                if (GM.ActiveTraders[i].name == name)
                {
                    GM.ActiveTrader = null;
                    GM.ActiveTraders.RemoveAt(i);
                }
            }

            LookAtHome();
            yield return new WaitForSeconds(1);
            MoveHome();
            yield return new WaitForSeconds(2);
            LookAtCenter();
            yield return new WaitForSeconds(3);

            GM.EndTheYear();
//            StartCoroutine(GM.TradeAnotherYear());
        }
    }

    // 5% шанс торговца на то что он изменит своё решение
    public void MayChangeMyMind()
    {
        int RandomNumber = Random.Range(1, 101);

        if (RandomNumber < 6)
        {
            if (Strategy == TraderStrategy.Honest) Strategy = TraderStrategy.Fraud;
            else if (Strategy == TraderStrategy.Fraud) Strategy = TraderStrategy.Honest;
        }
    }

    private void OnDisable()
    {
        GM.StartingPlacement -= MoveHome;
        GM.StartingPlacement -= LookAtHome;
        GM.ViewCenter -= LookAtCenter;
        GM.MoveToTrade -= MoveToTheMarket;
        GM.ResetTheResultsForTheYear -= PrepareData;
    }
}
