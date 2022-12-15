using System;
using TMPro;
using UnityEngine;

namespace AuthoringAndMono
{
    public class uiUpdate : MonoBehaviour
    {
        [SerializeField]private GameObject timerText;
        private TMP_Text _timerText;
        private float _timeMax = 55;
        private float _time = 0;

        [SerializeField]private GameObject sunHealthText;
        private TMP_Text _sunHealthText;
        private int _sunIntegrity;
        private int _sunHealth;
        
        [SerializeField]private GameObject endText;
        private TMP_Text _endText;

        [SerializeField] private ParticleSystem sunBurst;

        private void Awake()
        {
            _timerText = timerText.GetComponent<TMP_Text>();
            _sunHealthText = sunHealthText.GetComponent<TMP_Text>();
            _endText = endText.GetComponent<TMP_Text>();

            //_sunHealth = 100;
            _sunIntegrity = 100;
            _sunHealthText.text = "Sun integrity: " + _sunIntegrity + "%";
            _endText.text = "";
        }

        private void Update()
        {
            _time = _timeMax - GameValuesMono.timerSingleton;
            _timerText.text = "Sun Burst ETA: " + Mathf.Round(_time * 10.0f) * 0.1f;
            if (_time < 0)
            {
                if (!sunBurst.isPlaying)
                {
                    sunBurst.Play();
                }
            }

            // this is whack
            _sunIntegrity = 100 - GameValuesMono.sunHealth;
            _sunHealthText.text = "Sun integrity: " + _sunIntegrity + "%";
            if (_sunIntegrity <= 0)
            {
                _endText.text = "So the sun exploded or whatever, so now the galaxy is doomed and you are dead";
            }
        }
        
        public void UpdateUI(int sunHealth) //Issue calling method from ApplySunDamageSystem in Build, nullref error
        {
            _sunHealth = sunHealth;
            _sunIntegrity = 100 - _sunHealth;
            _sunHealthText.text = "Sun integrity: " + _sunIntegrity + "%";
            if (_sunIntegrity <= 0)
            {
                _endText.text = "So the sun exploded or whatever, so now the galaxy is doomed and you are dead";
            }
        }
    }
}