using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HouseUI : MonoBehaviour
{
    [HideInInspector]
    public GameObject house;

    public Image[] images = new Image[4];

    void Start()
    {
        var houseProps = house.GetComponent<HouseProperties>();
        var residents = houseProps.Residents;

        for (int i = 0; i < 4; i++)
        {
            if (i < residents.Length)
            {
                images[i].enabled = true;
                images[i].sprite = Game.Instance.EntityManager.GetEntity(residents[i]).GetComponent<SpriteRenderer>().sprite;
                images[i].color = houseProps.houseColor;
            }
            else images[i].enabled = false;
        }
    }

    void OnGUI()
    {
       if(house != null)
        {
            var houseProps = house.GetComponent<HouseProperties>();
            var residents = houseProps.Residents;
            
            for(int i = 0; i < 4; i++)
            {
                if (i < residents.Length)
                {
                    images[i].enabled = true;
                    images[i].sprite = Game.Instance.EntityManager.GetEntity(residents[i]).GetComponent<SpriteRenderer>().sprite;
                    images[i].color = houseProps.houseColor;
                }
                else images[i].enabled = false;
            }
        } 
    }
}
