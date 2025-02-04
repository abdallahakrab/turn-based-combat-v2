﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationBarManager : MonoBehaviour
{
    // Start is called before the first frame update
    Text informationText;
    public bool trainingMode;
    private void Awake()
    {
        informationText = GetComponent<Text>();
    }

    // Update is called once per frame
    public IEnumerator UpdateText(string inputText)
    {
        /*informationText.text = inputText + "\n"*/;
        if(trainingMode)
        {
            yield return new WaitForSeconds(0);
        }
        else
        {
            gameObject.SetActive(true);
            informationText.text += inputText + "\n";
            yield return new WaitForSeconds(3);
            informationText.text = "";
            gameObject.SetActive(false);
        }
        
        
    }
}
