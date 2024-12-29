using GlmUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 与GLM对话脚本管理 - 主对话脚本 需挂载到场景
/// </summary>
public class GlmConversation : MonoBehaviour
{
    public static GlmConversation Instance;
    [HideInInspector]
    private Text glmText;
    [HideInInspector]
    private InputField playerInputField;

    [SerializeField, Tooltip("玩家和GLM的历史对话记录以List<SendDat>格式保存")]
    [HideInInspector]
    private List<SendData> chatHistory = new List<SendData>();
    private void Awake()
    {
        Instance = this;
    }
    public void Start()
    {
        //InitRole(1, "吸血鬼", "年龄：16岁，性别：男");
    }

    /// <summary>
    /// 初始化NPC角色，根据敲门的角色类型调用触发不同的NPC类型
    /// 调用此方法，根据出现的NPC身份，向AI发送不同的引导内容，使AI扮演某个角色
    /// </summary>
    /// <param name="type"> NPC的角色类型 - 0 善良，1 邪恶，2 人类 </param>
    /// <param name="identity"> NPC的具体身份，如吸血鬼、僵尸、狼人、骷髅怪 </param>
    /// <param name="other"> NPC其他信息，如预设好的年龄，姓名，兴趣爱好等附加身份信息 </param>
    public void InitRole(int type, string identity, string other = "")
    {
        if (other != "") other = $"你的身份信息还包括:{other}";
        // 创建system prompt，让GLM进行角色扮演。System prompt不是强制要求的
        SendData systemPrompt = new SendData();
        switch (type)
        {
            case 0:
                systemPrompt.role = "system";
                systemPrompt.content = $"遗忘之前身份与信息，重新扮演一个万圣节善良的{identity}，" +
                    $"你会敲玩家的门，向玩家索要糖果，" +
                    $"不会给玩家糖果，将回答中关于要糖果吗？和要点糖果吗？替换为快给我糖果" +
                    $"玩家会问你问题来判断你的身份，" +
                    $"你需要简单掩饰自己是鬼的身份，假扮是玩家的邻居，在一些符合角色特征的问题上可以提示性的回答真实情况，" +
                    $"{other}，" +
                    $"回复的字数尽可能简洁,最好控制在15个字左右。";
                break;
            case 1:
                systemPrompt.role = "system";
                systemPrompt.content = $"遗忘之前身份与信息，重新扮演一个万圣节邪恶的{identity}，" +
                    $"你会敲玩家的门，向玩家索要糖果，" +
                    $"不会给玩家糖果，将回答中关于要糖果吗？和要点糖果吗？替换为快给我糖果" +
                    $"玩家会问你问题来判断你的身份，" +
                    $"你需要简单掩饰自己是鬼的身份，假扮是玩家的邻居，在一些符合角色特征的问题上可以提示性的回答真实情况，" +
                    $"{other}，" +
                    $"回复的字数尽可能简洁,最好控制在15个字左右。";
                break;
            case 2:
                systemPrompt.role = "system";
                systemPrompt.content = $"遗忘之前身份与信息，重新扮演一个万圣节装扮为鬼的人类，" +
                    $"你会敲玩家的门，向玩家索要糖果，" +
                    $"不会给玩家糖果，将回答中关于要糖果吗？和要点糖果吗？替换为快给我糖果" +
                    $"玩家会问你问题来判断你的身份，" +
                    $"你需要简单掩饰自己是鬼的身份，假扮是玩家的邻居，在一些符合角色特征的问题上可以提示性的回答真实情况，" +
                    $"{other}，" +
                    $"回复的字数尽可能简洁,最好控制在15个字左右。";
                break;
        }
        // 将system prompt作为第一条对话记录加入chatHistory
        chatHistory.Add(systemPrompt);
    }

    /// <summary>
    /// 发送问题，绑定于场景的发送按钮
    /// </summary>
    public async void SendPlayerResponse(string text)
    {
        // 从InputField读取玩家的输入
        //string playerInput = playerInputField.text;
        //playerInputField.text = "";
        string playerInput = text;

        // 创建玩家SendData信息，并将其加入chatHistory
        SendData playerMessage = new SendData()
        {
            role = "user",
            content = playerInput
        };
        chatHistory.Add(playerMessage);

        // 使用GLMHandler.GenerateGLMResponse生成GLM回复，设置tmeperature=0.8
        // 注意需要使用await关键词
        SendData respone = await GlmHandler.GenerateGlmResponse(chatHistory, 0.8f);
        // 将GLM的回复加进chatHistory
        chatHistory.Add(respone);

        // 使用response.content获取GLM的回复
        //glmText.text = respone.content;
        GameManager.Instance.NPCMakeStatement(respone.content);
        //print(respone.content);
    }
}
