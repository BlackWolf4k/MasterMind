using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    public void gameScene()
    {
        SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex + 1 );
    }
}
