using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreenUIHelper : MonoBehaviour
{
    public void Restart()
    {
        System.Diagnostics.Process.Start(Application.dataPath.Replace("_Data", ".exe")); //new program
        Application.Quit(); //kill current process
    }

    public void Quit()
    {
        Application.Quit();
    }
}
