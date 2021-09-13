using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void SkipSequence()
    {
        SceneManager.LoadScene(2);
    }
    public void CloseGame()
    {
        Application.Quit();
    }
    
}
