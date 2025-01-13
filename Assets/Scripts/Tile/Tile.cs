using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{

    [Header("格子颜色")]

    [SerializeField] protected SpriteRenderer _renderer; // 

    [Header("高光交互")]
    [SerializeField] private GameObject _highlight; // 高光






    // 初始化当前格子
    public virtual void Init(int x, int y){

    }


    public void SetHighlight(bool status)
    {
        if (_highlight != null)
        {
            _highlight.SetActive(status);
        }
    }


    // 将十六进制颜色解析为颜色对象
    public Color ConvertHexColor(string hexColor){
        if (ColorUtility.TryParseHtmlString(hexColor, out Color ColorObj)) {
            return ColorObj;
        } 
        else {
            Debug.LogError("Failed to parse color!");
            ColorUtility.TryParseHtmlString("#009FCF", out Color defaultColorObj);
            return defaultColorObj;
        }

    }
    







    // Start is called before the first frame update
    // void Start()
    // {

    // }

    // Update is called once per frame
    // void Update()
    // {
        
    // }



}
