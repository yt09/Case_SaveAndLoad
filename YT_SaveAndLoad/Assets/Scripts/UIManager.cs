using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager _instance;

    //获得2个Text组件
    public Text shootNumText;

    public Text scoreText;

    public int shootNum = 0;
    public int score = 0;

    public Toggle musicToggle;
    public AudioSource musicAudio;

    private void Awake()
    {
        _instance = this;
        if (PlayerPrefs.HasKey("MusicOn"))
        {
            if (PlayerPrefs.GetInt("MusicOn") == 1)
            {
                musicToggle.isOn = true;
                musicAudio.enabled = true;
            }
            else
            {
                musicToggle.isOn = false;
                musicAudio.enabled = false;
            }
        }
        else
        {
            musicToggle.isOn = true;
        }
    }

    private void Update()
    {
        //更新Text组件的显示内容
        shootNumText.text = shootNum.ToString();
        scoreText.text = score.ToString();

        MusicSwitch();
    }

    public void MusicSwitch()
    {
        if (musicToggle.isOn == false)
        {
            musicAudio.enabled = false;
            PlayerPrefs.SetInt("MusicOn", 0);
        }
        else
        {
            musicAudio.enabled = true;
            PlayerPrefs.SetInt("MusicOn", 1);
        }
        PlayerPrefs.Save();
    }

    //增加射击数（当开枪时）
    public void AddShootNum()
    {
        shootNum += 1;
    }

    //增加得分（当射中怪物时）
    public void AddScore()
    {
        score += 1;
    }
}