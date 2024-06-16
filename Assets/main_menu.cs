using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class main_menu : MonoBehaviour
{

    public void exitButton(){
        Application.Quit();
        Debug.Log("game closed");
    }

    public void startButton(){
        SceneManager.LoadScene("Simulator");
    }
}
