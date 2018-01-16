using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LitJson;
using System.Xml;

public class GameManager : MonoBehaviour
{
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
        if (Input.GetKeyDown(KeyCode.Escape))
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

    private Save CreateSaveGo()
    {
        Save save = new Save();
        foreach (var item in targetGOs)
        {
            TargetManager targetManager = item.GetComponent<TargetManager>();
            if (targetManager.activeMonster != null)
            {
                save.livingTargetPosition.Add(targetManager.targetPosition);
                int type = targetManager.activeMonster.GetComponent<MonsterManager>().monsterType;
                save.livingMonsterTypes.Add(type);
            }
        }
        save.shootNum = UIManager._instance.shootNum;
        save.score = UIManager._instance.score;

        return save;
    }

    private void SetGame(Save save)
    {
        foreach (var item in targetGOs)
        {
            item.GetComponent<TargetManager>().UpdateMonsters();
        }

        for (int i = 0; i < save.livingTargetPosition.Count; i++)
        {
            int position = save.livingTargetPosition[i];
            int type = save.livingMonsterTypes[i];
            targetGOs[position].GetComponent<TargetManager>().ActiveMonsterByType(type);
        }
        UIManager._instance.shootNum = save.shootNum;
        UIManager._instance.score = save.score;
        UnPause();
    }

    //从暂停状态恢复到非暂停状态
    public void ContinueGame()
    {
        UnPause();
    }

    public void NewGame()
    {
        foreach (GameObject targetGO in targetGOs)
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

    /// <summary>
    /// 保存游戏
    /// </summary>
    public void SaveGame()
    {
        // SaveByBin();
        SaveByXML();
        //    SaveByJson();
    }

    /// <summary>
    /// 加载游戏
    /// </summary>
    public void LoadGame()
    {
        //  LoadByBin();
        LoadByXML();
        //  LoadByJson();
    }

    private void SaveByBin()
    {
        Save save = CreateSaveGo();

        BinaryFormatter bf = new BinaryFormatter();

        FileStream fileStream = File.Create(Application.dataPath + "/StreamingFile" + "/ByBin.txt");

        bf.Serialize(fileStream, save);

        fileStream.Close();
    }

    private void LoadByBin()
    {
        //反序列化
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Open(Application.dataPath + "/StreamingFile" + "/ByBin.txt", FileMode.Open);
        //反序列化方法
        Save save = (Save)bf.Deserialize(fileStream);
        fileStream.Close();

        SetGame(save);
    }

    private void SaveByXML()
    {
        Save save = CreateSaveGo();
        //创建XML文件的存储路径
        string filePath = Application.dataPath + "/StreamingFile" + "/byXML.txt";
        //创建XML文档
        XmlDocument xmlDoc = new XmlDocument();
        //创建根节点，即最上层节点
        XmlElement root = xmlDoc.CreateElement("save");
        //设置根节点中的值
        root.SetAttribute("name", "saveFile1");

        //创建XmlElement
        XmlElement target;
        XmlElement targetPosition;
        XmlElement monsterType;

        //遍历save中存储的数据，将数据转换成XML格式
        for (int i = 0; i < save.livingTargetPosition.Count; i++)
        {
            target = xmlDoc.CreateElement("target");
            targetPosition = xmlDoc.CreateElement("targetPosition");
            //设置InnerText值
            targetPosition.InnerText = save.livingTargetPosition[i].ToString();
            monsterType = xmlDoc.CreateElement("monsterType");
            monsterType.InnerText = save.livingMonsterTypes[i].ToString();

            //设置节点间的层级关系 root -- target -- (targetPosition, monsterType)
            target.AppendChild(targetPosition);
            target.AppendChild(monsterType);
            root.AppendChild(target);
        }

        //设置射击数和分数节点并设置层级关系  xmlDoc -- root --(target-- (targetPosition, monsterType), shootNum, score)
        XmlElement shootNum = xmlDoc.CreateElement("shootNum");
        shootNum.InnerText = save.shootNum.ToString();
        root.AppendChild(shootNum);

        XmlElement score = xmlDoc.CreateElement("score");
        score.InnerText = save.score.ToString();
        root.AppendChild(score);

        xmlDoc.AppendChild(root);
        xmlDoc.Save(filePath);
    }

    private void LoadByXML()
    {
        string filePath = Application.dataPath + "/StreamingFile" + "/byXML.txt";
        if (File.Exists(filePath))
        {
            Save save = new Save();
            //加载XML文档
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            //通过节点名称来获取元素，结果为XmlNodeList类型
            XmlNodeList targets = xmlDoc.GetElementsByTagName("target");
            //遍历所有的target节点，并获得子节点和子节点的InnerText
            if (targets.Count != 0)
            {
                foreach (XmlNode target in targets)
                {
                    XmlNode targetPosition = target.ChildNodes[0];
                    int targetPositionIndex = int.Parse(targetPosition.InnerText);
                    //把得到的值存储到save中
                    save.livingTargetPosition.Add(targetPositionIndex);

                    XmlNode monsterType = target.ChildNodes[1];
                    int monsterTypeIndex = int.Parse(monsterType.InnerText);
                    save.livingMonsterTypes.Add(monsterTypeIndex);
                }
            }

            //得到存储的射击数和分数
            XmlNodeList shootNum = xmlDoc.GetElementsByTagName("shootNum");
            int shootNumCount = int.Parse(shootNum[0].InnerText);
            save.shootNum = shootNumCount;

            XmlNodeList score = xmlDoc.GetElementsByTagName("score");
            int scoreCount = int.Parse(score[0].InnerText);
            save.score = scoreCount;

            SetGame(save);
        }
    }

    private void SaveByJson()
    {
        Save save = CreateSaveGo();
        string filePath = Application.dataPath + "/StreamingFile" + "/ByJson.json";

        string saveJsonStr = JsonMapper.ToJson(save);

        StreamWriter sw = new StreamWriter(filePath);
        sw.Write(saveJsonStr);
        sw.Close();
    }

    private void LoadByJson()
    {
        string filePath = Application.dataPath + "/StreamingFile" + "/ByJson.json";
        if (File.Exists(filePath))
        {
            StreamReader sr = new StreamReader(filePath);

            string jsonStr = sr.ReadToEnd();

            sr.Close();

            Save save = JsonMapper.ToObject<Save>(jsonStr);

            SetGame(save);
        }
    }
}