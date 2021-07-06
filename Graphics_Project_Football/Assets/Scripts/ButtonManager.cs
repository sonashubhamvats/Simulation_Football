using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ButtonManager : MonoBehaviour
{
    public GameObject PausePanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void setActivePanel()
    {
        PausePanel.SetActive(true);
        AudioManager.instance.playButtonClick();
    }
    public void StartSimualtion()
    {
        AudioManager.instance.playButtonClick();
        SceneManager.LoadScene("Simulation");
        
    }
    public void BackToMenu()
    {
        AudioManager.instance.playButtonClick();
        SceneManager.LoadScene("mainMenu");
    }
}
