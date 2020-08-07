using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Home : MonoBehaviour
{
    public Vector3 House;
    public Vector3 NextToTheHouse;
    Ray ray;

    private void Start()
    {
        // Определяем ключевые точки дома
        House = gameObject.transform.position;

        ray.origin = transform.position;
        ray.direction = transform.forward;
        NextToTheHouse = ray.GetPoint(-50);
    }
}
