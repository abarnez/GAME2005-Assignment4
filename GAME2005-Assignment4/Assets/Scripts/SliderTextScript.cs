using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderTextScript : MonoBehaviour
{
    // Start is called before the first frame update

    public string textPrefix;
    public string textSuffix;
    public Text sliderText;
    void Start()
    {
        //sliderText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateText(float value)
    {
        sliderText.text = textPrefix + " " + value.ToString("F2") + " " + textSuffix;
    }
}
