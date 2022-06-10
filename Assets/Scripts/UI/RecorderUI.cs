using System;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectBlue.ArtNetRecorder
{


    public class RecorderUI : MonoBehaviour
    {

        [SerializeField] private Tab tab;

        [SerializeField] private Text timeCodeText;

        [SerializeField] private RecordButton recordButton;


        [SerializeField] private IndicatorUI indicatorUI;


        public IObservable<int> TabChangedAsObservable => tab.OnSelected;

        public RecordButton RecordButton => recordButton;

        public Tab Tab => tab;

        public IndicatorUI IndicatorUI => indicatorUI;

        public Text TimeCodeText => timeCodeText;

    }

}