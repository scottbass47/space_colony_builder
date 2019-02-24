using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Client
{
    public class OreHUD : MonoBehaviour
    {
        private TextMeshProUGUI text;

        private void Start()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {
            if(Game.Instance.ClientPlayer != null)
            {
                var player = Game.Instance.ClientPlayer;
                var resources = player.GetComponent<Resources>();
                SetText(resources.Ore);
            }
        }

        private void SetText(int amount)
        {
            text.text = $"Ore: {amount}".ToUpper();
        }
    } 
}
