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
/// ��Ϸ�߼�����
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    /// <summary>
    /// NPC����
    /// </summary>
    private List<(Sprite, string)> NPCInf = new List<(Sprite, string)>();
    /// <summary>
    /// BGM�б�
    /// </summary>
    private List<AudioClip> audioClips = new List<AudioClip>();

    #region ����
    /// <summary>
    /// �ǹ�����
    /// </summary>
    [HideInInspector] public int CurrentCandyQuantity;
    /// <summary>
    /// ��ǰ�ִ�
    /// </summary>
    [HideInInspector] public int CurrentRound;
    /// <summary>
    /// ��ǰ����ʱ
    /// </summary>
    [HideInInspector] public int CurrentPhaseDuration;
    /// <summary>
    /// �Ƿ�����Ϸ��
    /// </summary>
    [HideInInspector] public bool WhetherToStart;
    /// <summary>
    /// ��ǰ�������ֵ�����
    /// </summary>
    private int currentTrackIndex = 0;
    /// <summary>
    /// ��һ���Ƿ񴥷�������
    /// </summary>
    private bool WhetherTheNextRoundTriggersThePrank = false;
    #endregion
    #region ��������
    /// <summary>
    /// ��ʼ�ǹ�����
    /// </summary>
    [Header("��ʼ�ǹ�����")] public int TotalCandyQuantity;
    /// <summary>
    /// ��Ϸ���ִ�
    /// </summary>
    [Header("��Ϸ���ִ�")] public int TotalRoundsOftheGame;
    /// <summary>
    /// �׶�ʱ������
    /// </summary>
    [Header("�׶ε���ʱ����ʱ������")] public int TotalPhaseDuration;
    /// <summary>
    /// �׶�ʱ������
    /// </summary>
    [Header("������ʱ�۳�ʱ������")] public int DeductTheDurationOfThePrank;
    /// <summary>
    /// BGM��Դ
    /// </summary>
    public AudioSource audioSource;
    #endregion

    /// <summary>
    /// ����ʱЭ��
    /// </summary>
    Coroutine RoundCountdown;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        DataInit();//���ݳ�ʼ��
        GetNPCInf();//��ȡ�ⲿNPC�籾
        LoadMusicFiles();//��ȡ�ⲿ����mp3�ļ�
        UIManager.Instance.ControlInit();//�ؼ���ʼ��
        StartCoroutine(PlayNextTrackNextFrame());
    }
    //��ȡ�ⲿNPC�籾��Ϣ
    private void GetNPCInf()
    {
        string streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, "�б��ļ�");

        if (Directory.Exists(streamingAssetsPath))
        {
            // ��ȡ�������ļ���·��
            string[] directories = Directory.GetDirectories(streamingAssetsPath);

            foreach (string dir in directories)
            {
                // ��ȡ�ļ����ڵĵ���.txt�ͻ���.png�ļ�·��
                string txtFile = Path.Combine(dir, "����.txt");
                string pngFile = Path.Combine(dir, "����.png");

                string textContent = "";
                if (File.Exists(txtFile))
                {
                    textContent = File.ReadAllText(txtFile);
                    //Debug.Log("��ȡ������.txt: " + txtFile);
                }
                else
                {
                    Debug.LogWarning("û���ҵ�����.txt: " + txtFile);
                }
                Sprite sprite = null;
                if (File.Exists(pngFile))
                {
                    byte[] imageBytes = File.ReadAllBytes(pngFile);
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(imageBytes);
                    sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    //Debug.Log("��ȡ������.png: " + pngFile);
                }
                else
                {
                    Debug.LogWarning("û���ҵ�����.png: " + pngFile);
                }
                NPCInf.Add((sprite, textContent));
            }
        }
        else
        {
            Debug.LogError("�б��ļ�Ŀ¼������");
        }
        foreach (var item in NPCInf)
        {
            Debug.Log("Sprite: " + (item.Item1 != null ? "����" : "��") + ", Text: " + item.Item2);
        }
    }
    // ��ȡ�����ֶ�����
    private static string ExtractContent(string input, string key)
    {
        int startIndex = input.IndexOf(key + "#");
        if (startIndex == -1) return string.Empty;

        int endIndex = input.IndexOf('\n', startIndex);
        if (endIndex == -1) endIndex = input.Length;

        return input.Substring(startIndex + key.Length + 1, endIndex - startIndex - key.Length - 1);
    }
    // ��ȡ�����ֶ����ݲ��ϲ�
    private static string ExtractDescription(string input)
    {
        int startIndex = input.IndexOf("����#");
        if (startIndex == -1) return string.Empty;

        startIndex += 5;  // ���� "����#"
        string description = input.Substring(startIndex);

        // �ϲ���������
        return description.Replace("\n", "");
    }
    // ����ʱЭ��
    private IEnumerator CountdownCoroutine()
    {
        Debug.Log("����ʱ��ʼ");
        // ѭ������ʱ��ֱ��ʱ��Ϊ 0
        do
        {
            UIManager.Instance.ShowStageCountdown(CurrentPhaseDuration);
            yield return new WaitForSeconds(1);  // �ȴ� 1 ��
            CurrentPhaseDuration--;  // ���� 1 ��
        }
        while (CurrentPhaseDuration > 0);
        UIManager.Instance.ShowStageCountdown(CurrentPhaseDuration);
        Debug.Log("����ʱ����");
        RoundOver();
    }
    /// <summary>
    /// ���ݳ�ʼ��
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
    /// ������һ��
    /// </summary>
    public void StartRound()
    {
        #region ȷ���߼���ȷ�Ĳ������
        if (WhetherToStart)
        {
            Debug.Log("��ǰ������Ϸ��,���Ƚ�������!");
        }
        #endregion
        UIManager.Instance.ClearSessionList();//ÿ�ο����µ�һ������������б�
        if (CurrentRound == 1)//���������չʾ�ǹ�����
        {
            CurrentCandyQuantity = TotalCandyQuantity;
            UIManager.Instance.ShowCandyQuantity(CurrentCandyQuantity);
        }
        //���õ���ʱ
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
        //��ȡ��ǰ�ֵ�NPC��Ϣ
        string input = NPCInf[CurrentRound - 1].Item2;
        string role = ExtractContent(input, "��ɫ");
        string identity = ExtractContent(input, "���");
        string description = ExtractDescription(input);
        //Debug.Log("��ɫ: " + role);
        //Debug.Log("���: " + identity);
        //Debug.Log("����: " + description);

        UIManager.Instance.ShowIdentityImage(NPCInf[CurrentRound - 1].Item1);//չʾ��ǰ��NPC�����Ϣͼ

        GlmConversation.Instance.InitRole(Convert.ToInt32(role), identity, description);//���ݵ�ǰ��NPC��Ϣ��ʼ��AI
        WhetherToStart = true;
        Debug.Log("��Ϸ��ʼ,��ǰ�ִ�:" + CurrentRound);
        UIManager.Instance.StartRound();
        RoundCountdown = StartCoroutine(CountdownCoroutine());
        StartCoroutine(PlayNextTrackNextFrame());
        GlmConversation.Instance.SendPlayerResponse("Hello!");
    }
    /// <summary>
    /// �ִν���
    /// </summary>
    public void RoundOver()
    {
        WhetherToStart = false;
        Debug.Log("���ֽ���");
        if (RoundCountdown != null)
        {
            StopCoroutine(RoundCountdown);
            RoundCountdown = null;
        }
        if (CurrentPhaseDuration <= 0)
        {
            Debug.Log("��ʱ-ִ�г�ʱ�ͷ�");
            #region ִ�г�ʱ�ͷ�
            int ro = Convert.ToInt32(ExtractContent(NPCInf[CurrentRound - 1].Item2, "��ɫ"));
            //0 ������1 а��2 ����
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
            UIManager.Instance.ShowCandyQuantity(CurrentCandyQuantity);//չʾ�ǹ�����
        }
        if (CurrentCandyQuantity <= 0)
        {
            Debug.Log("��Ϸʧ��:�ǹ���������");
            UIManager.Instance.DisplayTips("��Ϸʧ��!�ٽ�������");
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
                Debug.Log("��Ϸʤ��");
                UIManager.Instance.DisplayTips("��Ϸ�ɹ�!��л���棡");
            }
        }
    }
    /// <summary>
    /// NPC����
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
    /// ���Ƿ���
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
        string folderPath = Path.Combine(Application.streamingAssetsPath, "����");
        if (Directory.Exists(folderPath))
        {
            string[] mp3Files = Directory.GetFiles(folderPath, "*.mp3");
            foreach (string filePath in mp3Files)
            {
                string fileName = Path.GetFileName(filePath);  // ��ȡ�ļ���
                Debug.Log("�ҵ��ļ�: " + fileName);
                StartCoroutine(LoadAudio(filePath)); // ʹ��Э���첽������Ƶ
            }
        }
        else
        {
            Debug.LogError("�����ļ��в�����: " + folderPath);
        }
    }
    // Э�̼�����Ƶ�ļ�
    private IEnumerator LoadAudio(string filePath)
    {
        // ���ļ�·��ת��Ϊ URL
        string path = "file://" + filePath;  // ʹ�� file:// Э������ȡ�����ļ�

        // ʹ�� UnityWebRequestMultimedia.GetAudioClip ��������Ƶ
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG);  // ָ����Ƶ����Ϊ MP3

        // �������󲢵ȴ���Ӧ
        yield return www.SendWebRequest();

        // ����Ƿ��д���
        if (www.result == UnityWebRequest.Result.Success)
        {
            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);  // ��ȡ���ص���Ƶ����
            if (clip != null)
            {
                audioClips.Add(clip);  // ����Ƶ��ӵ��б���
                Debug.Log("������Ƶ�ɹ�: " + clip.name);
            }
            else
            {
                Debug.LogError("��Ƶ����ʧ��: " + filePath);
            }
        }
        else
        {
            Debug.LogError("��Ƶ���ش���: " + www.error);
        }
    }
    // �����б��е���Ƶ
    private IEnumerator PlayNextTrackNextFrame()
    {
        // �ȴ�һ֡
        yield return null;  // �ȴ�һ֡�����ִ��

        if (audioClips.Count == 0)
        {
            Debug.LogWarning("û����Ƶ�ɲ���");
        }
        else
        {
            // ���û�����ڲ��ŵ����ֻ������Ѿ�������ϣ����ŵ�һ��
            if (!audioSource.isPlaying)
            {
                if (audioClips.Count > 0)
                {
                    audioSource.clip = audioClips[0];  // ����Ϊ��һ����Ƶ
                    audioSource.Play();  // ����
                    currentTrackIndex = 0;  // ���õ�ǰ����Ϊ��һ��
                }
            }
            else
            {
                // �����ǰ�������ڲ��ţ�������һ��
                currentTrackIndex++;
                if (currentTrackIndex >= audioClips.Count)  // ��������б�ĩβ��ѭ������һ��
                {
                    currentTrackIndex = 0;
                }

                audioSource.clip = audioClips[currentTrackIndex];  // ����Ϊ��һ����Ƶ
                audioSource.Play();  // ����
            }
        }

    }
    /// <summary>
    /// ������ִ���߼�
    /// </summary>
    public void NoSugar()
    {

        if (!WhetherToStart)
        {
            return;
        }
        int ro = Convert.ToInt32(ExtractContent(NPCInf[CurrentRound - 1].Item2, "��ɫ"));
        //0 ������1 а��2 ����
        if (ro == 0)
        {
            int price = UnityEngine.Random.Range(1, 4);
            Debug.Log("�ƹ���������" + price + "����");
            if (CurrentCandyQuantity >= price)
            {
                CurrentCandyQuantity = CurrentCandyQuantity - price;
            }
            else
            {
                CurrentCandyQuantity = 0;
            }
            UIManager.Instance.DisplayTips("��û�и��ɰ����ǹ�������������" + price + "����!");
        }
        else if (ro == 1)
        {
            Debug.Log("�����������3����");
            if (CurrentCandyQuantity >= 3)
            {
                CurrentCandyQuantity = CurrentCandyQuantity - 3;
            }
            else
            {
                CurrentCandyQuantity = 0;
            }
            UIManager.Instance.DisplayTips("��û�и�����ǹ�������������3����!");
        }
        else if (ro == 2)
        {

        }
        UIManager.Instance.ShowCandyQuantity(CurrentCandyQuantity);//չʾ�ǹ�����
        RoundOver();
    }
    /// <summary>
    /// ����ִ���߼�
    /// </summary>
    public void GiveSugar()
    {
        if (!WhetherToStart)
        {
            return;
        }
        if (CurrentCandyQuantity > 0)//����
        {
            CurrentCandyQuantity--;
        }
        int ro = Convert.ToInt32(ExtractContent(NPCInf[CurrentRound - 1].Item2, "��ɫ"));
        //0 ������1 а��2 ����
        if (ro == 0)
        {
            WhetherTheNextRoundTriggersThePrank = true;
            Debug.Log("������������");
            UIManager.Instance.DisplayTips("�㱻׽Ū�ˣ�ʱ�����Ʊ����" + (TotalPhaseDuration - DeductTheDurationOfThePrank) + "s");
        }
        else if (ro == 1)
        {

        }
        else if (ro == 2)
        {

        }
        UIManager.Instance.ShowCandyQuantity(CurrentCandyQuantity);//չʾ�ǹ�����
        RoundOver();
    }
}
