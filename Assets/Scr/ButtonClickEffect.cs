using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonClickEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    /// <summary>
    /// �������ǵĽ�����ť
    /// </summary>
    Image Interaction_Image;
    /// <summary>
    /// Ĭ�ϰ�ť
    /// </summary>
    Sprite Default_Spr;
    /// <summary>
    /// �����ǰ���
    /// </summary>
    Sprite NoSugarPress_Spr;
    /// <summary>
    /// ���ǰ���
    /// </summary>
    Sprite SugarPress_Spr;
    private void Awake()
    {
        Interaction_Image = GameObject.Find("Interaction_Image").GetComponent<Image>();
        Default_Spr= Resources.Load<Sprite>("Spr/Ĭ��");
        NoSugarPress_Spr = Resources.Load<Sprite>("Spr/�����ǰ���");
        SugarPress_Spr = Resources.Load<Sprite>("Spr/���ǰ���");
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if(this.gameObject.name== "GiveSugar_Button")
        {
            Interaction_Image.sprite = SugarPress_Spr;
        }
        else if (this.gameObject.name == "NoSugar_Button")
        {
            Interaction_Image.sprite = NoSugarPress_Spr;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Interaction_Image.sprite = Default_Spr;
    }
}
