using Client;
using Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddWorkers : MonoBehaviour
{
    [HideInInspector]
    public GameObject rock;

    public Button addButton0;
    public Button addButton1;
    public Button addButton2;
    public Button addButton3;

    public Sprite rockState0;
    public Sprite rockState1;
    public Sprite rockState2;
    public Sprite rockState3;

    private Text workerCount;
    private RockProperties rockProps;

    private void Start()
    {
        addButton0.onClick.AddListener(RequestWorker0);
        addButton1.onClick.AddListener(RequestWorker1);
        addButton2.onClick.AddListener(RequestWorker2);
        addButton3.onClick.AddListener(RequestWorker3);
    }

    private void OnGUI()
    {
        var rockImage = transform.GetChild(0).gameObject;
        var image = rockImage.GetComponent<Image>();
        if (rock != null)
        {
            image.enabled = true;
            rockProps = rock.GetComponent<RockProperties>();

            float rockPercentage = (float)rockProps.Ore / rockProps.originalOre;

            if (rockPercentage < .25f) image.sprite = rockState3;
            else if (rockPercentage < .50f) image.sprite = rockState2;
            else if (rockPercentage < .75f) image.sprite = rockState1;
            else image.sprite = rockState0;
        }
        else
        {
            image.enabled = false;
            addButton0.interactable = false;
            addButton1.interactable = false;
            addButton2.interactable = false;
            addButton3.interactable = false;
        }

    }

    private void RequestWorker(int slot)
    {
        var client = Game.Instance.Client;
        var eo = rock.GetComponent<EntityObject>();
        //rockProps = rock.GetComponent<RockProperties>();

        client.SendRequestPacket(new AddWorkerRequest { EntityID = eo.ID, Slot = slot});

        rockProps.workers++;
    }

    private void RequestWorker0()
    {
        RequestWorker(0);
        addButton0.interactable = false;
        rockProps.hasSlot0 = true;
    }
    private void RequestWorker1()
    {
        RequestWorker(1);
        addButton1.interactable = false;
        rockProps.hasSlot1 = true;
    }
    private void RequestWorker2()
    {
        RequestWorker(2);
        addButton2.interactable = false;
        rockProps.hasSlot2 = true;
    }
    private void RequestWorker3()
    {
        RequestWorker(3);
        addButton3.interactable = false;
        rockProps.hasSlot3 = true;
    }

    public void Refresh()
    {
        var rockProps = rock.GetComponent<RockProperties>();
        if (!rockProps.hasSlot0) addButton0.interactable = true;
        else addButton0.interactable = false;

        if (!rockProps.hasSlot1) addButton1.interactable = true;
        else addButton1.interactable = false;

        if (!rockProps.hasSlot2) addButton2.interactable = true;
        else addButton2.interactable = false;

        if (!rockProps.hasSlot3) addButton3.interactable = true;
        else addButton3.interactable = false;
    }
}
