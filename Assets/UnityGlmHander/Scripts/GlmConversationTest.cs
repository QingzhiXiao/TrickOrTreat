using GlmUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 与GLM对话脚本管理 - 主对话脚本 需挂载到场景
/// </summary>
public class GlmConversationTest : MonoBehaviour
{
    public static GlmConversationTest Instance;

    public Text AIText1;
    public Text AIText2;

    public InputField Input1;
    public InputField Input2;


    [SerializeField, Tooltip("玩家和GLM的历史对话记录以List<SendDat>格式保存")]
    [HideInInspector]
    private List<SendData> chatHistory1 = new List<SendData>();
    [SerializeField, Tooltip("玩家和GLM的历史对话记录以List<SendDat>格式保存")]
    [HideInInspector]
    private List<SendData> chatHistory2 = new List<SendData>();
    private void Awake()
    {
        Instance = this;
    }
    public void Start()
    {
        InitRole();
    }
    //[ContextMenu("初始化")]
    public void InitRole()
    {
        SendData systemPrompt = new SendData();
        systemPrompt.role = "system";
        systemPrompt.content = $"我提供一个角色你来扮演他：戴南瓜头的小女孩扮演的吸血鬼" +
            $"小女孩的名字叫艾尔德拉Eldra.年龄:16，" +
            $"喜欢甜食，喜欢看哈利波特，尤其喜欢里面的赫敏，自认为是狮院的巫师。" +
            $"她的妈妈喜欢烘焙，她的爸爸喜欢钓鱼。" +
            $"害怕大型犬，数学不好，喜欢大笑，最喜欢的事是全家一起看电视节目。" +
            $"平时周一到周五上学，周一周五晚上要去上数学补习班。" +
            $"周末去做志愿或者帮邻居溜猫赚零花钱。" +
            $"<她假扮成吸血鬼的妆扮和身份在万圣节去敲门要糖果>这个吸血鬼爱做美甲，喜欢红色饰品，" +
            $"住在废弃古堡里，名字：奥布斯库丽亚Obscuria。年龄约1300岁，喜欢丝绸做的衣服。" +
            $"讨厌银器，讨厌大蒜，讨厌阳光。保养头发的秘诀是多喝女人的血。喜欢对着月亮品尝“红酒”。" +
            $"脖子上的装饰是有一次被吸血鬼猎人追杀，留了疤，干脆在上面纹了身。喜欢去酒吧寻找猎物。" ;

        chatHistory1.Add(systemPrompt);

        SendData systemPrompt2 = new SendData();
        systemPrompt2.role = "system";
        systemPrompt2.content = $"扮演一个万圣节善良的" +
            $"你会敲玩家的门，向玩家索要糖果，" +
            $"不会给玩家糖果，将回答中关于要糖果吗？和要点糖果吗？替换为快给我糖果" +
            $"玩家会问你问题来判断你的身份，" +
            $"你需要简单掩饰自己是鬼的身份，假扮是玩家的邻居，在一些符合角色特征的问题上可以提示性的回答真实情况，" +
            $"" +
            $"回复的字数尽可能简洁,最好控制在15个字左右。";

        chatHistory2.Add(systemPrompt2);

    }

    /// <summary>
    /// 发送问题，绑定于场景的发送按钮
    /// </summary>
   [ContextMenu("发送给AI1")] 
    public async void SendPlayerResponseTo1()
    {
        string playerInput = Input1.text;

        SendData playerMessage = new SendData()
        {
            role = "user",
            content = playerInput
        };
        chatHistory1.Add(playerMessage);

        SendData respone = await GlmHandler.GenerateGlmResponse(chatHistory1, 0.8f);
        // 将GLM的回复加进chatHistory
        chatHistory1.Add(respone);

        AIText1.text = respone.content;
        // 使用response.content获取GLM的回复
        //glmText.text = respone.content;
        //GameManager.Instance.NPCMakeStatement(respone.content);
        //print(respone.content);
    }
    /// <summary>
    /// 发送问题，绑定于场景的发送按钮
    /// </summary>
    [ContextMenu("发送给AI2")]
    public async void SendPlayerResponseTo2()
    {
        string playerInput = Input2.text;

        SendData playerMessage = new SendData()
        {
            role = "user",
            content = playerInput
        };
        chatHistory2.Add(playerMessage);

        SendData respone = await GlmHandler.GenerateGlmResponse(chatHistory2, 0.8f);
        // 将GLM的回复加进chatHistory
        chatHistory2.Add(respone);

        AIText2.text = respone.content;
        // 使用response.content获取GLM的回复
        //glmText.text = respone.content;
        //GameManager.Instance.NPCMakeStatement(respone.content);
        //print(respone.content);
    }
}
