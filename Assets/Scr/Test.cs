using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public Image image;
    public Text A_TEXT;
    public Text B_TEXT;
    public Text C_TEXT;

    private string streamingAssetsPath;
    // �洢ÿ���ļ��е� Sprite �� string
    private List<(Sprite, string)> spriteTextList = new List<(Sprite, string)>();
    // Start is called before the first frame update
    void Start()
    {

    }
    public int index = 0;

    [ContextMenu("����")]
    public void test()
    {


        //image.sprite = spriteTextList[index].Item1;
        //A_TEXT.text = role;
        //B_TEXT.text = identity;
        //C_TEXT.text = description;

        //index++;
    }

}

