using System.Collections;
using System.Collections.Generic;
using CodeStage.AdvancedFPSCounter;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestMode : MonoBehaviour
{
    private DrawCircle mDrawCircle;
    public static TestMode Instance;
    void Awake()
    {
        Instance = this;
        mDrawCircle = new DrawCircle();
    }
    void Update()
    {
        if (mDrawCircle.isGestureDone())
        {
            SceneManager.LoadScene(0);
        }
    }
}
