using System;
using TMPro;
using UnityEngine;

namespace AuthoringAndMono
{
    public class uiUpdate : MonoBehaviour
    {
        public GameObject timerText;
        private TMP_Text _timerText;

        private float _timeMax = 55;
        private float _time = 0;

        private void Awake()
        {
            _timerText = timerText.GetComponent<TMP_Text>();
        }

        private void Update()
        {
            _time = _timeMax - GameValuesMono.timerSingleton;
            _timerText.text = "Sun Burst ETA: " + Mathf.Round(_time * 10.0f) * 0.1f;

        }
    }
}