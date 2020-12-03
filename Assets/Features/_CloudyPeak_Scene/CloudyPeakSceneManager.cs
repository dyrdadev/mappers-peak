using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CloudyPeakSceneManager : MonoBehaviour
{
    void Awake()
    {
        SceneManager.LoadScene("OrreryHall", LoadSceneMode.Additive);
    }

}
