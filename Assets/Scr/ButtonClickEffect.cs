using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonClickEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    /// <summary>
    /// 给不给糖的交互按钮
    /// </summary>
    Image Interaction_Image;
    /// <summary>
    /// 默认按钮
    /// </summary>
    Sprite Default_Spr;
    /// <summary>
    /// 不给糖按下
    /// </summary>
    Sprite NoSugarPress_Spr;
    /// <summary>
    /// 给糖按下
    /// </summary>
    Sprite SugarPress_Spr;
    private void Awake()
    {
        Interaction_Image = GameObject.Find("Interaction_Image").GetComponent<Image>();
        Default_Spr= Resources.Load<Sprite>("Spr/默认");
        NoSugarPress_Spr = Resources.Load<Sprite>("Spr/不给糖按下");
        SugarPress_Spr = Resources.Load<Sprite>("Spr/给糖按下");
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
