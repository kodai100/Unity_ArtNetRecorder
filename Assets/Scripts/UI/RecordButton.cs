using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectBlue.ArtNetRecorder
{
    [RequireComponent(typeof(Button))]
    public class RecordButton : MonoBehaviour
    {

        [SerializeField] private Sprite stopImage;
        [SerializeField] private Sprite recImage;

        [SerializeField] private Image image;

        [SerializeField] private Color stoppedBackgroundColor = Color.gray;
        [SerializeField] private Color recordingBackgroundColor = Color.red;

        [SerializeField] private Button button;


        public Button Button => button;
        

        public void SetRecord()
        {
            image.sprite = stopImage;
            button.targetGraphic.color = recordingBackgroundColor;
        }


        public void SetStop()
        {
            image.sprite = recImage;
            button.targetGraphic.color = stoppedBackgroundColor;
        }
        
    }
}
