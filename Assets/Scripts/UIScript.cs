using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public GameManager GM;
    public GameObject[] Panels;

    // Этот метод при старте создаёт достаточное количество строк в UI для отображения результатов победителей
    public void CreateAnEmptyListOfWinners()
    {
        GameObject TraderResultPanel = Panels[0].transform.GetChild(0).gameObject;

        for (int i = 1; i < GM.PercentageOfTraders; i++)
        {
            GameObject AnotherPanel = Instantiate(TraderResultPanel);
            AnotherPanel.transform.SetParent(Panels[0].transform);
            AnotherPanel.name = "Trader_" + i;
            AnotherPanel.transform.GetChild(0).GetComponent<Text>().text = (i + 1).ToString();
        }
    }


    public void PopulateTheTableWithResults(Trader[] _winners)
    {
        for (int i = 0; i < _winners.Length; i++)
        {
            Panels[0].transform.GetChild(i).GetChild(1).GetComponent<Text>().text = _winners[i].name;
            Panels[0].transform.GetChild(i).GetChild(2).GetComponent<Text>().text = _winners[i].Type.ToString();
            Panels[0].transform.GetChild(i).GetChild(3).GetComponent<Text>().text = _winners[i].AnnualProfit.ToString();
        }
    }

    // ----------------------------------------------------------------------------------------------- Методы кнопок ---------------------------------------------------------------------------------------------


    public void NextYear()
    {
        Panels[1].SetActive(false);
        StartCoroutine(GM.TradeAnotherYear());
    }

    public void TimeChange (bool SpeedUp)
    {
        if (SpeedUp)
        {
            if (Time.timeScale < 20) Time.timeScale++;
        }
        else
        {
            if (Time.timeScale > 0) Time.timeScale--;
        }

        Panels[2].GetComponent<Text>().text = Time.timeScale.ToString();
    }

    public void QuitGameButton()
    {
        Application.Quit();
    }
}
