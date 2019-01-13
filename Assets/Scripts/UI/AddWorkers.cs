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

    public Button[] addButtons = new Button[4];
    public Sprite[] rockStates = new Sprite[4];

    private Sprite origButtonSprite;
    private Text workerCount;
    private RockProperties rockProps;

    private void Start()
    {
        origButtonSprite = addButtons[0].image.sprite;
        addButtons[0].onClick.AddListener(RequestWorker0);
        addButtons[1].onClick.AddListener(RequestWorker1);
        addButtons[2].onClick.AddListener(RequestWorker2);
        addButtons[3].onClick.AddListener(RequestWorker3);
    }

    private void OnGUI()
    {
        var rockImage = transform.GetChild(0).gameObject;
        var image = rockImage.GetComponent<Image>();
        if (rock != null)
        {
            foreach (Button button in addButtons) button.image.enabled = true;
            
            image.enabled = true;
            rockProps = rock.GetComponent<RockProperties>();

            float rockPercentage = (float)rockProps.Ore / rockProps.originalOre;

            if (rockPercentage < .25f) image.sprite = rockStates[3];
            else if (rockPercentage < .50f) image.sprite = rockStates[2];
            else if (rockPercentage < .75f) image.sprite = rockStates[1];
            else image.sprite = rockStates[0];
        }
        else
        {
            image.enabled = false;
            foreach (Button button in addButtons)
            {
                button.interactable = false;
                button.image.enabled = false;
            }
        }

    }

    private void RequestWorker(int slot)
    {
        var client = Game.Instance.Client;
        var eo = rock.GetComponent<EntityObject>();

        client.SendRequestPacket(new AddWorkerRequest { EntityID = eo.ID, Slot = slot});

        rockProps.workers++;
    }

    private void RequestWorker0()
    {
        RequestWorker(0);
        addButtons[0].animator.enabled = true;
        addButtons[0].interactable = false;
        rockProps.hasSlot[0] = true;
    }
    private void RequestWorker1()
    {
        RequestWorker(1);
        addButtons[1].animator.enabled = true;
        addButtons[1].interactable = false;
        rockProps.hasSlot[1] = true;
    }
    private void RequestWorker2()
    {
        RequestWorker(2);
        addButtons[2].animator.enabled = true;
        addButtons[2].interactable = false;
        rockProps.hasSlot[2] = true;
    }
    private void RequestWorker3()
    {
        RequestWorker(3);
        addButtons[3].animator.enabled = true;
        addButtons[3].interactable = false;
        rockProps.hasSlot[3] = true;
    }

    public void Refresh()
    {
        rockProps = rock.GetComponent<RockProperties>();
        for (int i = 0; i < addButtons.Length; i++) RefreshButton(addButtons[i], i);
    }

    public void RefreshButton(Button button, int slot)
    {
        rockProps = rock.GetComponent<RockProperties>();

        if (!rockProps.hasSlot[slot])
        {
           addButtons[slot].interactable = true;
           addButtons[slot].animator.enabled = false;
           addButtons[slot].image.sprite = origButtonSprite;
        }
        else
        {
            addButtons[slot].interactable = false;
            addButtons[slot].animator.enabled = true;
        }
    }
}
