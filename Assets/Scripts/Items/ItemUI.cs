using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemUI : MonoBehaviour
{
    public TMP_Text munitionText, munitionText_aim;
    Gun gunController;
    public bool itemPickedUp = false;

    void Start(){
        munitionText.gameObject.SetActive(false);
        munitionText_aim.gameObject.SetActive(false);

        gunController = GetComponent<Gun>();
    }

    public void ShowAmunitionText(bool enabled)
    {
        if(enabled && !itemPickedUp) return;
        munitionText.gameObject.SetActive(enabled);
    }

    public void ShowAmunitionText_Aim(bool enabled)
    {
        if(enabled && !itemPickedUp) return;
        munitionText_aim.gameObject.SetActive(enabled);
    }
    
    void Update()
    {
        if(gunController != null){ 
            float maxBales = gunController.maxBales;
            float currentBales = gunController.currentBales;
            munitionText.text = currentBales + " / " + maxBales;
            munitionText_aim.text = currentBales + " / " + maxBales;
        }
    }


}
