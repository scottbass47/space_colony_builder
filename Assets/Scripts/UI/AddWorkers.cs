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

    public Button addButton;

    private Text workerCount;
    private GameObject oreText;

    private void Start()
    {
        addButton.onClick.AddListener(RequestWorker);
    }

    private void OnGUI()
    {
        if (rock != null)
        {
            oreText = transform.GetChild(1).gameObject;
            var rockProps = rock.GetComponent<RockProperties>();

            oreText.GetComponent<Text>().text = "Ore Left: " + rockProps.Ore;
        }
    }

    private void RequestWorker()
    {
        var client = Game.Instance.Client;
        var eo = rock.GetComponent<EntityObject>();
        var rockProps = rock.GetComponent<RockProperties>();

        client.SendRequestPacket(new AddWorkerRequest { EntityID = eo.ID, Slot = rockProps.workers});

        rockProps.workers++;
        GetComponentInChildren<Text>().text = "Workers: " + rockProps.workers;
    }

    public void Refresh()
    {
        var rockProps = rock.GetComponent<RockProperties>();
        GetComponentInChildren<Text>().text = "Workers: " + rockProps.workers;
    }
}
