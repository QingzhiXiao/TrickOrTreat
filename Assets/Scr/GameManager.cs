using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;
using Directory = System.IO.Directory;
using File = System.IO.File;
/// <summary>
/// 游戏逻辑管理
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    /// <summary>
    /// NPC数据
    /// </summary>
    private List<(Sprite, string)> NPCInf = new List<(Sprite, string)>();
    /// <summary>
    /// BGM列表
    /// </summary>
    private List<AudioClip> audioClips = new List<AudioClip>();

    #region 数据
    /// <summary>
    /// 糖果数量
    /// </summary>
    [HideInInspector] public int CurrentCandyQuantity;
    /// <summary>
    /// 当前轮次
    /// </summary>
    [HideInInspector] public int CurrentRound;
    /// <summary>
    /// 当前倒计时
    /// </summary>
    [HideInInspector] public int CurrentPhaseDuration;
    /// <summary>
    /// 是否在游戏中
    /// </summary>
    [HideInInspector] public bool WhetherToStart;
    /// <summary>
    /// 当前播放音乐的索引
    /// </summary>
    private int currentTrackIndex = 0;
    /// <summary>
    /// 下一轮是否触发恶作剧
    /// </summary>
    private bool WhetherTheNextRoundTriggersThePrank = false;
    #endregion
    #region 配置数据
    /// <summary>
    /// 初始糖果总量
    /// </summary>
    [Header("初始糖果总量")] public int TotalCandyQuantity;
    /// <summary>
    /// 游戏总轮次
    /// </summary>
    [Header("游戏总轮次")] public int TotalRoundsOftheGame;
    /// <summary>
    /// 阶段时长：秒
    /// </summary>
    [Header("阶段倒计时给定时长：秒")] public int TotalPhaseDuration;
    /// <summary>
    /// 阶段时长：秒
    /// </summary>
    [Header("恶作剧时扣除时长：秒")] public int DeductTheDurationOfThePrank;
    /// <summary>
    /// BGM音源
    /// </summary>
    public AudioSource audioSource;
    #endregion

    /// <summary>
    /// 倒计时协程
    /// </summary>
    Coroutine RoundCountdown;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        DataInit();//数据初始化
        GetNPCInf();//读取外部NPC剧本
        LoadMusicFiles();//读取外部所有mp3文件
        UIManager.Instance.ControlInit();//控件初始化
        StartCoroutine(PlayNextTrackNextFrame());
    }
    //获取外部NPC剧本信息
    private void GetNPCInf()
    {
        string streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, "列表文件");

        if (Directory.Exists(streamingAssetsPath))
        {
            // 获取所有子文件夹路径
            string[] directories = Directory.GetDirectories(streamingAssetsPath);

            foreach (string dir in directories)
            {
                // 获取文件夹内的档案.txt和画像.png文件路径
                string txtFile = Path.Combine(dir, "档案.txt");
                string pngFile = Path.Combine(dir, "画像.png");

                string textContent = "";
                if (File.Exists(txtFile))
                {
                    textContent = File.ReadAllText(txtFile);
                    //Debug.Log("读取到档案.txt: " + txtFile);
                }
                else
                {
                    Debug.LogWarning("没有找到档案.txt: " + txtFile);
                }
                Sprite sprite = null;
                if (File.Exists(pngFile))
                {
                    byte[] imageBytes = File.ReadAllBytes(pngFile);
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(imageBytes);
                    sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    //Debug.Log("读取到画像.png: " + pngFile);
                }
                else
                {
                    Debug.LogWarning("没有找到画像.png: " + pngFile);
                }
                NPCInf.Add((sprite, textContent));
            }
        }
        else
        {
            Debug.LogError("列表文件目录不存在");
        }
        foreach (var item in NPCInf)
        {
            Debug.Log("Sprite: " + (item.Item1 != null ? "存在" : "无") + ", Text: " + item.Item2);
        }
    }
    // 提取单个字段内容
    private static string ExtractContent(string input, string key)
    {
        int startIndex = input.IndexOf(key + "#");
        if (startIndex == -1) return string.Empty;

        int endIndex = input.IndexOf('\n', startIndex);
        if (endIndex == -1) endIndex = input.Length;

        return input.Substring(startIndex + key.Length + 1, endIndex - startIndex - key.Length - 1);
    }
    // 提取描述字段内容并合并
    private static string ExtractDescription(string input)
    {
        int startIndex = input.IndexOf("描述#");
        if (startIndex == -1) return string.Empty;

        startIndex += 5;  // 跳过 "描述#"
        string description = input.Substring(startIndex);

        // 合并描述内容
        return description.Replace("\n", "");
    }
    // 倒计时协程
    private IEnumerator CountdownCoroutine()
    {
        Debug.Log("倒计时开始");
        // 循环倒计时，直到时间为 0
        do
        {
            UIManager.Instance.ShowStageCountdown(CurrentPhaseDuration);
            yield return new WaitForSeconds(1);  // 等待 1 秒
            CurrentPhaseDuration--;  // 减少 1 秒
        }
        while (CurrentPhaseDuration > 0);
        UIManager.Instance.ShowStageCountdown(CurrentPhaseDuration);
        Debug.Log("倒计时结束");
        RoundOver();
    }
    /// <summary>
    /// 数据初始化
    /// </summary>
    private void DataInit()
    {
        if (TotalRoundsOftheGame <= 0)
        {
            TotalRoundsOftheGame = 8;
        }
        if (TotalCandyQuantity <= 0)
        {
            TotalCandyQuantity = 8;
        }
        CurrentRound = 1;
        if (TotalPhaseDuration <= 0)
        {
            TotalPhaseDuration = 90;
        }
        WhetherToStart = false;
    }
    /// <summary>
    /// 开启新一轮
    /// </summary>
    public void StartRound()
    {
        #region 确保逻辑正确的补充代码
        if (WhetherToStart)
        {
            Debug.Log("当前正在游戏中,请先结束本轮!");
        }
        #endregion
        UIManager.Instance.ClearSessionList();//每次开启新的一轮先清除聊天列表
        if (CurrentRound == 1)//如果是首轮展示糖果数量
        {
            CurrentCandyQuantity = TotalCandyQuantity;
            UIManager.Instance.ShowCandyQuantity(CurrentCandyQuantity);
        }
        //重置倒计时
        if (WhetherTheNextRoundTriggersThePrank)
        {
            CurrentPhaseDuration = TotalPhaseDuration - DeductTheDurationOfThePrank;
            UIManager.Instance.WhetherTheRoundWasPranked(true);
            WhetherTheNextRoundTriggersThePrank = false;
        }
        else
        {
            CurrentPhaseDuration = TotalPhaseDuration;
            UIManager.Instance.WhetherTheRoundWasPranked(false);
        }
        //提取当前轮的NPC信息
        string input = NPCInf[CurrentRound - 1].Item2;
        string role = ExtractContent(input, "角色");
        string identity = ExtractContent(input, "身份");
        string description = ExtractDescription(input);
        //Debug.Log("角色: " + role);
        //Debug.Log("身份: " + identity);
        //Debug.Log("描述: " + description);

        UIManager.Instance.ShowIdentityImage(NPCInf[CurrentRound - 1].Item1);//展示当前轮NPC身份信息图

        GlmConversation.Instance.InitRole(Convert.ToInt32(role), identity, description);//根据当前轮NPC信息初始化AI
        WhetherToStart = true;
        Debug.Log("游戏开始,当前轮次:" + CurrentRound);
        UIManager.Instance.StartRound();
        RoundCountdown = StartCoroutine(CountdownCoroutine());
        StartCoroutine(PlayNextTrackNextFrame());
        GlmConversation.Instance.SendPlayerResponse("Hello!");
    }
    /// <summary>
    /// 轮次结束
    /// </summary>
    public void RoundOver()
    {
        WhetherToStart = false;
        Debug.Log("本轮结束");
        if (RoundCountdown != null)
        {
            StopCoroutine(RoundCountdown);
            RoundCountdown = null;
        }
        if (CurrentPhaseDuration <= 0)
        {
            Debug.Log("超时-执行超时惩罚");
            #region 执行超时惩罚
            int ro = Convert.ToInt32(ExtractContent(NPCInf[CurrentRound - 1].Item2, "角色"));
            //0 善良，1 邪恶，2 人类
            if (ro == 0)
            {
                int price = UnityEngine.Random.Range(1, 4);
                if (CurrentCandyQuantity >= price)
                {
                    CurrentCandyQuantity = CurrentCandyQuantity - price;
                }
                else
                {
                    CurrentCandyQuantity = 0;
                }
            }
            else if (ro == 1)
            {
                if (CurrentCandyQuantity >= 3)
                {
                    CurrentCandyQuantity = CurrentCandyQuantity - 3;
                }
                else
                {
                    CurrentCandyQuantity = 0;
                }
            }
            else if (ro == 2)
            {

            }
            #endregion
            UIManager.Instance.ShowCandyQuantity(CurrentCandyQuantity);//展示糖果数量
        }
        if (CurrentCandyQuantity <= 0)
        {
            Debug.Log("游戏失败:糖果数量不足");
            UIManager.Instance.DisplayTips("游戏失败!再接再厉！");
        }
        else
        {
            if (CurrentRound < TotalRoundsOftheGame)
            {
                CurrentRound++;
                UIManager.Instance.RoundOver();
                //StartRound();
            }
            else
            {
                Debug.Log("游戏胜利");
                UIManager.Instance.DisplayTips("游戏成功!感谢试玩！");
            }
        }
    }
    /// <summary>
    /// NPC发言
    /// </summary>
    /// <param name="content"></param>
    public void NPCMakeStatement(string content)
    {
        if (!WhetherToStart)
        {
            return;
        }
        UIManager.Instance.NPCMakeaStatement(content);
    }
    /// <summary>
    /// 主角发言
    /// </summary>
    /// <param name="word"></param>
    public void PlayerMakeStatement(string word)
    {
        if (!WhetherToStart)
        {
            return;
        }
        if (word == null)
        {
            return;
        }
        else if (word == "")
        {
            return;
        }
        GlmConversation.Instance.SendPlayerResponse(word);
        UIManager.Instance.PlayerMakeaStatement(word);
    }
    private void LoadMusicFiles()
    {
        string folderPath = Path.Combine(Application.streamingAssetsPath, "音乐");
        if (Directory.Exists(folderPath))
        {
            string[] mp3Files = Directory.GetFiles(folderPath, "*.mp3");
            foreach (string filePath in mp3Files)
            {
                string fileName = Path.GetFileName(filePath);  // 获取文件名
                Debug.Log("找到文件: " + fileName);
                StartCoroutine(LoadAudio(filePath)); // 使用协程异步加载音频
            }
        }
        else
        {
            Debug.LogError("音乐文件夹不存在: " + folderPath);
        }
    }
    // 协程加载音频文件
    private IEnumerator LoadAudio(string filePath)
    {
        // 将文件路径转换为 URL
        string path = "file://" + filePath;  // 使用 file:// 协议来读取本地文件

        // 使用 UnityWebRequestMultimedia.GetAudioClip 来加载音频
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG);  // 指定音频类型为 MP3

        // 发送请求并等待响应
        yield return www.SendWebRequest();

        // 检查是否有错误
        if (www.result == UnityWebRequest.Result.Success)
        {
            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);  // 获取下载的音频内容
            if (clip != null)
            {
                audioClips.Add(clip);  // 将音频添加到列表中
                Debug.Log("加载音频成功: " + clip.name);
            }
            else
            {
                Debug.LogError("音频加载失败: " + filePath);
            }
        }
        else
        {
            Debug.LogError("音频加载错误: " + www.error);
        }
    }
    // 播放列表中的音频
    private IEnumerator PlayNextTrackNextFrame()
    {
        // 等待一帧
        yield return null;  // 等待一帧后继续执行

        if (audioClips.Count == 0)
        {
            Debug.LogWarning("没有音频可播放");
        }
        else
        {
            // 如果没有正在播放的音乐或音乐已经播放完毕，播放第一首
            if (!audioSource.isPlaying)
            {
                if (audioClips.Count > 0)
                {
                    audioSource.clip = audioClips[0];  // 设置为第一首音频
                    audioSource.Play();  // 播放
                    currentTrackIndex = 0;  // 设置当前索引为第一首
                }
            }
            else
            {
                // 如果当前有音乐在播放，播放下一首
                currentTrackIndex++;
                if (currentTrackIndex >= audioClips.Count)  // 如果到达列表末尾，循环到第一首
                {
                    currentTrackIndex = 0;
                }

                audioSource.clip = audioClips[currentTrackIndex];  // 设置为下一首音频
                audioSource.Play();  // 播放
            }
        }

    }
    /// <summary>
    /// 不给糖执行逻辑
    /// </summary>
    public void NoSugar()
    {

        if (!WhetherToStart)
        {
            return;
        }
        int ro = Convert.ToInt32(ExtractContent(NPCInf[CurrentRound - 1].Item2, "角色"));
        //0 善良，1 邪恶，2 人类
        if (ro == 0)
        {
            int price = UnityEngine.Random.Range(1, 4);
            Debug.Log("善鬼拿走了你" + price + "颗糖");
            if (CurrentCandyQuantity >= price)
            {
                CurrentCandyQuantity = CurrentCandyQuantity - price;
            }
            else
            {
                CurrentCandyQuantity = 0;
            }
            UIManager.Instance.DisplayTips("你没有给可爱鬼糖果，他抢走了你" + price + "颗糖!");
        }
        else if (ro == 1)
        {
            Debug.Log("恶鬼拿走了你3颗糖");
            if (CurrentCandyQuantity >= 3)
            {
                CurrentCandyQuantity = CurrentCandyQuantity - 3;
            }
            else
            {
                CurrentCandyQuantity = 0;
            }
            UIManager.Instance.DisplayTips("你没有给恶鬼糖果，他抢走了你3颗糖!");
        }
        else if (ro == 2)
        {

        }
        UIManager.Instance.ShowCandyQuantity(CurrentCandyQuantity);//展示糖果数量
        RoundOver();
    }
    /// <summary>
    /// 给糖执行逻辑
    /// </summary>
    public void GiveSugar()
    {
        if (!WhetherToStart)
        {
            return;
        }
        if (CurrentCandyQuantity > 0)//给糖
        {
            CurrentCandyQuantity--;
        }
        int ro = Convert.ToInt32(ExtractContent(NPCInf[CurrentRound - 1].Item2, "角色"));
        //0 善良，1 邪恶，2 人类
        if (ro == 0)
        {
            WhetherTheNextRoundTriggersThePrank = true;
            Debug.Log("被恶作剧啦！");
            UIManager.Instance.DisplayTips("你被捉弄了，时间限制变成了" + (TotalPhaseDuration - DeductTheDurationOfThePrank) + "s");
        }
        else if (ro == 1)
        {

        }
        else if (ro == 2)
        {

        }
        UIManager.Instance.ShowCandyQuantity(CurrentCandyQuantity);//展示糖果数量
        RoundOver();
    }
}
