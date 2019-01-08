using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

namespace Client {
    public class DragWindow : MonoBehaviour
    {
        private EventSystem FindEvent;
        private Event Find2;
        private SelectTile selectTile;

        private void Start()
        {
            selectTile = Game.Instance.GetComponent<SelectTile>();
        }

        public void OnDrag(BaseEventData EventData)
        {  
            if(EventData.selectedObject != null) EventData.selectedObject.transform.position = Input.mousePosition;
        }

        public void SetPivot(BaseEventData EventData)
        {
            if (EventData.selectedObject != null)
            {
                var rt = gameObject.GetComponent<RectTransform>();

                Vector3 difference = Input.mousePosition - rt.transform.position;
                //Set pivot to where the mouse clicked on the window to prevent jumpy behavior
                rt.pivot = new Vector2(difference.x / rt.rect.width + rt.pivot.x, difference.y / rt.rect.height + rt.pivot.y);
            }
        }

        public void EnableTileSelection(BaseEventData EventData)
        {
            selectTile.enabled = true;
        }

        public void DisableTileSelection(BaseEventData EventData)
        {
            selectTile.enabled = false;
        }
    }
}