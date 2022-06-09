using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorUI : MonoBehaviour
{

    [SerializeField] private List<Image> images;

    [SerializeField] private Color oddColor = Color.cyan;
    [SerializeField] private Color evenColor = Color.yellow;

    public int IndicatorCount => images.Count;

    private int scale;

    private bool isOdd = true;
    
    public void SetScale(int numUniverse)
    {
        scale = numUniverse * 512 * 256;
    }
    
    public void ResetIndicator()
    {
        images.ForEach(image =>
        {
            image.transform.localScale = new Vector3(1,0,1);
        });
    }

    public void Set(int index, int value)
    {

        if (index % IndicatorCount == 0)
        {
            isOdd = !isOdd;
        }
        
        index %= IndicatorCount;

        images[index].transform.localScale = new Vector3(1, (float) value / scale, 1);
        images[index].color = isOdd ? oddColor : evenColor;

    }


}
