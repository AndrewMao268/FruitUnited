using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Difficulty : MonoBehaviour
{
    
    public Slider mainSlider;
	public TMP_Text EASYTEXT;
    public TMP_Text MEDIUMTEXT;
    public TMP_Text HARDTEXT;
    public TMP_Text IMPOSSIBLETEXT;

	public void Start()
	{


        EASYTEXT.enabled = true; 
        MEDIUMTEXT.enabled = false;
        HARDTEXT.enabled = false;
        IMPOSSIBLETEXT.enabled = false;
		//Adds a listener to the main slider and invokes a method when the value changes. THIS I GOT FROM UNITY WEBSITE*
        mainSlider.onValueChanged.AddListener(delegate (float value) {ValueChangeCheck(value);});


	}
	
	// Invoked when the value of the slider changes.
	void ValueChangeCheck(float a)
	{
		if (a == 0)
        {
        EASYTEXT.enabled = true; 
        MEDIUMTEXT.enabled = false;
        HARDTEXT.enabled = false;
        IMPOSSIBLETEXT.enabled = false;
        }
        else if (a == 1)
        {
        EASYTEXT.enabled = false; 
        MEDIUMTEXT.enabled = true;
        HARDTEXT.enabled = false;
        IMPOSSIBLETEXT.enabled = false;
        }
        else if (a == 2)
        {
        EASYTEXT.enabled = false; 
        MEDIUMTEXT.enabled = false;
        HARDTEXT.enabled = true;
        IMPOSSIBLETEXT.enabled = false;
        }
        else if (a == 3)
        {
        EASYTEXT.enabled = false; 
        MEDIUMTEXT.enabled = false;
        HARDTEXT.enabled = false;
        IMPOSSIBLETEXT.enabled = true;
        }
	}
}


