using Client;
using Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultipleObjsUI : MonoBehaviour
{
    [HideInInspector]
    public ArrayList Entities { get; set; }

    public Button requestWorkButton;
    
    void Start()
    {
        requestWorkButton.onClick.AddListener(RequestWork);
    }

    private void RequestWork()
    {
        var client = Game.Instance.Client;

        Debug.Log("Length: " + Entities.Count);
        foreach(GameObject entity in Entities)
        {
            var eo = entity.GetComponent<EntityObject>();
            Debug.Log("ID" + eo.ID);
            client.SendRequestPacket(new AddWorkerRequest { EntityID = eo.ID, Slot = 0 });
            eo.GetComponent<RockProperties>().hasSlot[0] = true;
        }
    }
}
