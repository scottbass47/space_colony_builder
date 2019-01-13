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
        
    }

    void OnGUI()
    {
       if(house != null)
        {
            var houseProps = house.GetComponent<HouseProperties>();
            var residents = houseProps.Residents;
            
            for(int i = 0; i < residents.Length; i++)
            {
                images[i].sprite = Game.Instance.EntityManager.GetEntity(residents[i]).GetComponent<SpriteRenderer>().sprite;
            }
        } 
    }
}
