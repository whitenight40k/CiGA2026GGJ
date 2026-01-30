using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    public void QuitGame()
    {
        print("成功");
        Application.Quit();
    }
}
