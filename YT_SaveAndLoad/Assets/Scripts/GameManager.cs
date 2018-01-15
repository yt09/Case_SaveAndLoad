using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager _instance;

    //是否是暂停状态
    public bool isPaused = true;
    public GameObject menuGO;

    public GameObject[] targetGOs;

    private void Awake()
    {
        _instance = this;
        //游戏开始时是暂停的状态
        Pause();      
    }

    private void Update()
    {
        //判断是否按下ESC键，按下的话，调出Menu菜单，并将游戏状态更改为暂停状态
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    //暂停状态
    private void Pause()
    {
        isPaused = true;
        menuGO.SetActive(true);
        Time.timeScale = 0;
        Cursor.visible = true;
    }
    //非暂停状态
    private void UnPause()
    {
        isPaused = false;
        menuGO.SetActive(false);
        Time.timeScale = 1;
        Cursor.visible = false;
    }
    //从暂停状态恢复到非暂停状态
    public void ContinueGame()
    {
        UnPause();
    }

    public void NewGame()
    {
        foreach(GameObject targetGO in targetGOs)
        {
            targetGO.GetComponent<TargetManager>().UpdateMonsters();
        }
        UIManager._instance.shootNum = 0;
        UIManager._instance.score = 0;

        UnPause();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
}
