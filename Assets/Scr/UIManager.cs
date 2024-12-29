using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// UIȫ�ֹ���
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    #region �ؼ�
    #region �̳�ҳ���
    /// <summary>
    /// ���ֽ̳�Button
    /// </summary>
    Button Tutorial_Button;
    /// <summary>
    /// ���ֽ̳�ҳ��Image
    /// </summary>
    GameObject TutorialBackground_Image;
    /// <summary>
    /// ȷ��
    /// </summary>
    Button Affirm_Button;
    #endregion
    #region ��ʾҳ���
    /// <summary>
    /// ��ʾҳGOj
    /// </summary>
    GameObject HintBackground_Image;
    /// <summary>
    /// ��ʾҳText
    /// </summary>
    Text Hint_Text;
    /// <summary>
    /// ��ʾҳ����Button
    /// </summary>
    Button HintReturn_Button;
    /// <summary>
    /// ��ʾҳ����2Button
    /// </summary>
    Button HintReturn2_Button;
    #endregion
    /// <summary>
    /// NPC����Image
    /// </summary>
    Image NPCSpawnPoint_Image;
    /// <summary>
    /// ��ʷ�Ի��б�ScrollRect
    /// </summary>
    ScrollRect SessionList_ScrollRect;
    /// <summary>
    /// ���Ƿ��������InputField
    /// </summary>
    InputField Player_InputField;
    /// <summary>
    /// ��ǰ�ǹ�����չʾText
    /// </summary>
    Text CandyQuantity_Text;
    /// <summary>
    /// �����߼�ִ��Button
    /// </summary>
    Button GiveSugar_Button;
    /// <summary>
    /// �������߼�ִ��Button
    /// </summary>
    Button NoSugar_Button;
    /// <summary>
    /// ��ǰ�׶ε���ʱʣ��ʱ��չʾText
    /// </summary>
    Text StageCountdown_Text;
    /// <summary>
    /// �ύ���Ƿ�������Button
    /// </summary>
    Button SendText_Button;
    /// <summary>
    /// ִ�в鿴�ĵ�����Button
    /// </summary>
    Button ViewDocument_Button;
    /// <summary>
    /// ��ǰ�ֿ�ʼ(������)Button
    /// </summary>
    Button GhostKnock_Button;
    /// <summary>
    /// ȷ���˳���ǰ����Button
    /// </summary>
    Button ExitProgram_Button;
    /// <summary>
    /// NPC�ĵ�����GOj
    /// </summary>
    GameObject Document_GOj;
    /// <summary>
    /// NPC�ĵ�Image
    /// </summary>
    Image DocumentContent_Image;
    /// <summary>
    /// ������ҳButton
    /// </summary>
    Button ReurnBack_Button;
    #endregion
    #region Ԥ����
    /// <summary>
    /// NPC����Ԥ����
    /// </summary>
    GameObject NPC_SessionList_Child;
    /// <summary>
    /// ���Ƿ���Ԥ����
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
    /// �ؼ���ʼ��
    /// </summary>
    public void ControlInit()
    {
        Document_GOj.SetActive(false);
        Clear(SessionList_ScrollRect.content);
        CandyQuantity_Text.text = "�ǹ�������" + GameManager.Instance.CurrentCandyQuantity;
        StageCountdown_Text.text = "����ʣ�ࣺ0s";
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
    /// չʾ��ʾ
    /// </summary>
    public void DisplayTips(string tips)
    {
        HintBackground_Image.SetActive(true);
        Hint_Text.text = tips;
    }
    /// <summary>
    /// �����������
    /// </summary>
    public void ClearSessionList()
    {
        Clear(SessionList_ScrollRect.content);
    }
    /// <summary>
    /// չʾ��ǰ�ǹ�����
    /// </summary>
    /// <param name="num"></param>
    public void ShowCandyQuantity(int num)
    {
        CandyQuantity_Text.text = "�ǹ�����:" + num;
    }
    /// <summary>
    /// չʾ��ǰ����ʱ
    /// </summary>
    /// <param name="price"></param>
    public void ShowStageCountdown(int price)
    {
        StageCountdown_Text.text = "����ʣ��:" + price + "s";
    }
    /// <summary>
    /// �˻���ҳ�߼�
    /// </summary>
    private void ReurnBack_Listener()
    {
        Document_GOj.SetActive(false);
    }

    /// <summary>
    /// �˳������߼�
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
    /// ������ǰ���߼�
    /// </summary>
    private void GhostKnock_Listener()
    {
        GameManager.Instance.StartRound();
    }

    /// <summary>
    /// �鿴�ĵ�ִ���߼�
    /// </summary>
    private void ViewDocument_Listener()
    {
        Document_GOj.SetActive(true);
    }

    /// <summary>
    /// �ύ���Ƿ����߼�
    /// </summary>
    private void SendText_Listener()
    {
        GameManager.Instance.PlayerMakeStatement(Player_InputField.text);
    }

    /// <summary>
    /// ������ִ���߼�
    /// </summary>
    private void NoSugar_Listener()
    {


        GameManager.Instance.NoSugar();
    }
    /// <summary>
    /// �����Ƿ񱻶�����
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
    /// ����ִ���߼�
    /// </summary>
    private void GiveSugar_Listener()
    {


        GameManager.Instance.GiveSugar();
    }

    /// <summary>
    /// ���Ƿ���
    /// </summary>
    /// <param name="contentOfSpeech">��������</param>
    public void PlayerMakeaStatement(string contentOfSpeech)
    {
        Player_InputField.text = "";
        GameObject child = InstantiateControl(Player_SessionList_Child, SessionList_ScrollRect.content);
        Text text = child.transform.Find("Text").GetComponent<Text>();
        text.text = "Player:" + contentOfSpeech;
        StartCoroutine(WaitSetScrollbarValue());
    }
    /// <summary>
    /// NPC����
    /// </summary>
    /// <param name="contentOfSpeech">��������</param>
    public void NPCMakeaStatement(string contentOfSpeech)
    {
        GameObject child = InstantiateControl(NPC_SessionList_Child, SessionList_ScrollRect.content);
        Text text = child.transform.Find("Text").GetComponent<Text>();
        text.text = "NPC:" + contentOfSpeech;
        StartCoroutine(WaitSetScrollbarValue());
    }
    /// <summary>
    /// ��֤�б���ʾ�����·���Э��
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitSetScrollbarValue()
    {
        //�ȴ���֡
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        SessionList_ScrollRect.verticalScrollbar.value = 0;
        //StopCoroutine(WaitSetScrollbarValue_Coroutine);
    }
    /// <summary>
    /// ʵ��������
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
    /// ���������
    /// </summary>
    private void Clear(Transform transform)
    {
        if (transform.childCount > 0)
        {
            int b = transform.childCount;
            for (int i = 0; i < b; i++)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);//����ɾ��
            }
        }
    }
    /// <summary>
    /// չʾ���ͼ
    /// </summary>
    /// <param name="item1"></param>
    public void ShowIdentityImage(Sprite item1)
    {
        NPCSpawnPoint_Image.gameObject.SetActive(true);
        NPCSpawnPoint_Image.sprite = item1;
    }
    /// <summary>
    /// �ѽ��뵱ǰ����Ϸ
    /// </summary>
    public void StartRound()
    {
        GhostKnock_Button.interactable = false;
        GiveSugar_Button.interactable = true;
        NoSugar_Button.interactable = true;
        SendText_Button.interactable = true;
    }
    /// <summary>
    /// ��ǰ����Ϸ����
    /// </summary>
    public void RoundOver()
    {
        GhostKnock_Button.interactable = true;
        GiveSugar_Button.interactable = false;
        NoSugar_Button.interactable = false;
        SendText_Button.interactable = false;

    }
}
