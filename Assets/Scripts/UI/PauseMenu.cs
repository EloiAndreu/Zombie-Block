using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    bool paused = false;
    public GameObject pausePanel;

    public void SetTimeScale(float timeScale){
        Time.timeScale = timeScale;
    }

    void Start()
    {
        pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !paused)
        {
            paused = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SetTimeScale(0f);
            pausePanel.SetActive(true);
        }
    }

    public void Continue()
    {
        paused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SetTimeScale(1f);
        pausePanel.SetActive(false);
    
    
    }
}
