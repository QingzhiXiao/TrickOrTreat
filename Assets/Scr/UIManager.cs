using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// UI全局管理
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    #region 控件
    #region 教程页相关
    /// <summary>
    /// 新手教程Button
    /// </summary>
    Button Tutorial_Button;
    /// <summary>
    /// 新手教程页面Image
    /// </summary>
    GameObject TutorialBackground_Image;
    /// <summary>
    /// 确认
    /// </summary>
    Button Affirm_Button;
    #endregion
    #region 提示页相关
    /// <summary>
    /// 提示页GOj
    /// </summary>
    GameObject HintBackground_Image;
    /// <summary>
    /// 提示页Text
    /// </summary>
    Text Hint_Text;
    /// <summary>
    /// 提示页返回Button
    /// </summary>
    Button HintReturn_Button;
    /// <summary>
    /// 提示页返回2Button
    /// </summary>
    Button HintReturn2_Button;
    #endregion
    /// <summary>
    /// NPC画像Image
    /// </summary>
    Image NPCSpawnPoint_Image;
    /// <summary>
    /// 历史对话列表ScrollRect
    /// </summary>
    ScrollRect SessionList_ScrollRect;
    /// <summary>
    /// 主角发言输入框InputField
    /// </summary>
    InputField Player_InputField;
    /// <summary>
    /// 当前糖果数量展示Text
    /// </summary>
    Text CandyQuantity_Text;
    /// <summary>
    /// 给糖逻辑执行Button
    /// </summary>
    Button GiveSugar_Button;
    /// <summary>
    /// 不给糖逻辑执行Button
    /// </summary>
    Button NoSugar_Button;
    /// <summary>
    /// 当前阶段倒计时剩余时长展示Text
    /// </summary>
    Text StageCountdown_Text;
    /// <summary>
    /// 提交主角发言内容Button
    /// </summary>
    Button SendText_Button;
    /// <summary>
    /// 执行查看文档内容Button
    /// </summary>
    Button ViewDocument_Button;
    /// <summary>
    /// 当前轮开始(鬼敲门)Button
    /// </summary>
    Button GhostKnock_Button;
    /// <summary>
    /// 确认退出当前程序Button
    /// </summary>
    Button ExitProgram_Button;
    /// <summary>
    /// NPC文档载体GOj
    /// </summary>
    GameObject Document_GOj;
    /// <summary>
    /// NPC文档Image
    /// </summary>
    Image DocumentContent_Image;
    /// <summary>
    /// 返回主页Button
    /// </summary>
    Button ReurnBack_Button;
    #endregion
    #region 预制体
    /// <summary>
    /// NPC发言预制体
    /// </summary>
    GameObject NPC_SessionList_Child;
    /// <summary>
    /// 主角发言预制体
    /// </summary>
    GameObject Player_SessionList_Child;

    #endregion
    private void Awake()
    {
        Instance = this;

        NPCSpawnPoint_Image = this.transform.Find("Background_Image/NPCPhotoFrame_Image/NPCSpawnPoint_Image").GetComponent<Image>();
        SessionList_ScrollRect = this.transform.Find("Background_Image/SessionList_ScrollRect").GetComponent<ScrollRect>();
        Player_InputField = this.transform.Find("Background_Image/Player_InputField").GetComponent<InputField>();
        CandyQuantity_Text = this.transform.Find("Background_Image/CandyQuantity_Text").GetComponent<Text>();
        GiveSugar_Button = this.transform.Find("Background_Image/Interaction_Image/GiveSugar_Button").GetComponent<Button>();

        NoSugar_Button = this.transform.Find("Background_Image/Interaction_Image/NoSugar_Button").GetComponent<Button>();
        StageCountdown_Text = this.transform.Find("Background_Image/StageCountdown_Text").GetComponent<Text>();
        SendText_Button = this.transform.Find("Background_Image/SendText_Button").GetComponent<Button>();
        ViewDocument_Button = this.transform.Find("Background_Image/ViewDocument_Button").GetComponent<Button>();
        GhostKnock_Button = this.transform.Find("Background_Image/GhostKnock_Button").GetComponent<Button>();
        ExitProgram_Button = this.transform.Find("Background_Image/ExitProgram_Button").GetComponent<Button>();
        Document_GOj = this.transform.Find("Background_Image/Document_GOj").gameObject;
        DocumentContent_Image = this.transform.Find("Background_Image/Document_GOj/DocumentContent_Image").GetComponent<Image>();
        ReurnBack_Button = this.transform.Find("Background_Image/Document_GOj/ReurnBack_Button").GetComponent<Button>();

        Tutorial_Button = this.transform.Find("Background_Image/Tutorial_Button").GetComponent<Button>();
        TutorialBackground_Image = this.transform.Find("Background_Image/TutorialBackground_Image").gameObject;
        Affirm_Button = this.transform.Find("Background_Image/TutorialBackground_Image/Affirm_Button").GetComponent<Button>();

        HintBackground_Image = this.transform.Find("Background_Image/HintBackground_Image").gameObject;
        Hint_Text = this.transform.Find("Background_Image/HintBackground_Image/Hint_Image/Text").GetComponent<Text>();
        HintReturn_Button = this.transform.Find("Background_Image/HintBackground_Image/Return_Button").GetComponent<Button>();
        HintReturn2_Button = this.transform.Find("Background_Image/HintBackground_Image/Return2_Button").GetComponent<Button>();

        NPC_SessionList_Child = Resources.Load<GameObject>("Pre/NPC_SessionList_Child");
        Player_SessionList_Child = Resources.Load<GameObject>("Pre/Player_SessionList_Child");

    }
    /// <summary>
    /// 控件初始化
    /// </summary>
    public void ControlInit()
    {
        Document_GOj.SetActive(false);
        Clear(SessionList_ScrollRect.content);
        CandyQuantity_Text.text = "糖果数量：" + GameManager.Instance.CurrentCandyQuantity;
        StageCountdown_Text.text = "本轮剩余：0s";
        GhostKnock_Button.interactable = true;
        GiveSugar_Button.interactable = false;
        NoSugar_Button.interactable = false;
        SendText_Button.interactable = false;
        TutorialBackground_Image.SetActive(false);
        HintBackground_Image.SetActive(false);

        GiveSugar_Button.onClick.AddListener(GiveSugar_Listener);
        NoSugar_Button.onClick.AddListener(NoSugar_Listener);
        SendText_Button.onClick.AddListener(SendText_Listener);
        ViewDocument_Button.onClick.AddListener(ViewDocument_Listener);
        GhostKnock_Button.onClick.AddListener(GhostKnock_Listener);
        ExitProgram_Button.onClick.AddListener(ExitProgram_Listener);
        ReurnBack_Button.onClick.AddListener(ReurnBack_Listener);
        Tutorial_Button.onClick.AddListener(Tutorial_Listener);
        Affirm_Button.onClick.AddListener(Affirm_Listener);
        HintReturn_Button.onClick.AddListener(HintReturn_Listener);
        HintReturn2_Button.onClick.AddListener(HintReturn_Listener);
    }

    private void HintReturn_Listener()
    {
        HintBackground_Image.SetActive(false);
    }

    private void Affirm_Listener()
    {
        TutorialBackground_Image.SetActive(false);
    }

    private void Tutorial_Listener()
    {
        TutorialBackground_Image.SetActive(true);
    }
    /// <summary>
    /// 展示提示
    /// </summary>
    public void DisplayTips(string tips)
    {
        HintBackground_Image.SetActive(true);
        Hint_Text.text = tips;
    }
    /// <summary>
    /// 清除聊天内容
    /// </summary>
    public void ClearSessionList()
    {
        Clear(SessionList_ScrollRect.content);
    }
    /// <summary>
    /// 展示当前糖果数量
    /// </summary>
    /// <param name="num"></param>
    public void ShowCandyQuantity(int num)
    {
        CandyQuantity_Text.text = "糖果数量:" + num;
    }
    /// <summary>
    /// 展示当前倒计时
    /// </summary>
    /// <param name="price"></param>
    public void ShowStageCountdown(int price)
    {
        StageCountdown_Text.text = "本轮剩余:" + price + "s";
    }
    /// <summary>
    /// 退回主页逻辑
    /// </summary>
    private void ReurnBack_Listener()
    {
        Document_GOj.SetActive(false);
    }

    /// <summary>
    /// 退出程序逻辑
    /// </summary>
    private void ExitProgram_Listener()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
    Coroutine RoundCountdown;
    /// <summary>
    /// 开启当前轮逻辑
    /// </summary>
    private void GhostKnock_Listener()
    {
        GameManager.Instance.StartRound();
    }

    /// <summary>
    /// 查看文档执行逻辑
    /// </summary>
    private void ViewDocument_Listener()
    {
        Document_GOj.SetActive(true);
    }

    /// <summary>
    /// 提交主角发言逻辑
    /// </summary>
    private void SendText_Listener()
    {
        GameManager.Instance.PlayerMakeStatement(Player_InputField.text);
    }

    /// <summary>
    /// 不给糖执行逻辑
    /// </summary>
    private void NoSugar_Listener()
    {


        GameManager.Instance.NoSugar();
    }
    /// <summary>
    /// 本轮是否被恶作剧
    /// </summary>
    /// <param name="isOrNo"></param>
    public void WhetherTheRoundWasPranked(bool isOrNo)
    {
        if (isOrNo)
        {
            Player_InputField.characterLimit = 7;
        }
        else
        {
            Player_InputField.characterLimit = 1000;
        }
    }
    /// <summary>
    /// 给糖执行逻辑
    /// </summary>
    private void GiveSugar_Listener()
    {


        GameManager.Instance.GiveSugar();
    }

    /// <summary>
    /// 主角发言
    /// </summary>
    /// <param name="contentOfSpeech">发言内容</param>
    public void PlayerMakeaStatement(string contentOfSpeech)
    {
        Player_InputField.text = "";
        GameObject child = InstantiateControl(Player_SessionList_Child, SessionList_ScrollRect.content);
        Text text = child.transform.Find("Text").GetComponent<Text>();
        text.text = "Player:" + contentOfSpeech;
        StartCoroutine(WaitSetScrollbarValue());
    }
    /// <summary>
    /// NPC发言
    /// </summary>
    /// <param name="contentOfSpeech">发言内容</param>
    public void NPCMakeaStatement(string contentOfSpeech)
    {
        GameObject child = InstantiateControl(NPC_SessionList_Child, SessionList_ScrollRect.content);
        Text text = child.transform.Find("Text").GetComponent<Text>();
        text.text = "NPC:" + contentOfSpeech;
        StartCoroutine(WaitSetScrollbarValue());
    }
    /// <summary>
    /// 保证列表显示在最下方的协程
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitSetScrollbarValue()
    {
        //等待两帧
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        SessionList_ScrollRect.verticalScrollbar.value = 0;
        //StopCoroutine(WaitSetScrollbarValue_Coroutine);
    }
    /// <summary>
    /// 实例化物体
    /// </summary>
    /// <param name="pfb"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    private GameObject InstantiateControl(GameObject pfb, Transform parent)
    {
        GameObject prefabInstance = Instantiate(pfb);
        prefabInstance.transform.SetParent(parent);
        return prefabInstance;
    }
    /// <summary>
    /// 清除子物体
    /// </summary>
    private void Clear(Transform transform)
    {
        if (transform.childCount > 0)
        {
            int b = transform.childCount;
            for (int i = 0; i < b; i++)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);//立即删除
            }
        }
    }
    /// <summary>
    /// 展示身份图
    /// </summary>
    /// <param name="item1"></param>
    public void ShowIdentityImage(Sprite item1)
    {
        NPCSpawnPoint_Image.gameObject.SetActive(true);
        NPCSpawnPoint_Image.sprite = item1;
    }
    /// <summary>
    /// 已进入当前轮游戏
    /// </summary>
    public void StartRound()
    {
        GhostKnock_Button.interactable = false;
        GiveSugar_Button.interactable = true;
        NoSugar_Button.interactable = true;
        SendText_Button.interactable = true;
    }
    /// <summary>
    /// 当前轮游戏结束
    /// </summary>
    public void RoundOver()
    {
        GhostKnock_Button.interactable = true;
        GiveSugar_Button.interactable = false;
        NoSugar_Button.interactable = false;
        SendText_Button.interactable = false;

    }
}
