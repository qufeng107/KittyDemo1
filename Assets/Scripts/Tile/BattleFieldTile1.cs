using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFieldTile1 : Tile
{

    [Header("格子颜色")]
    [SerializeField] private Color _baseColor; // 初始颜色
    [SerializeField] private Color _offsetColor; // 间隔颜色

    // 初始化当前格子
    public override void Init(int x, int y){

        // 初始化颜色
        _baseColor = ConvertHexColor("#009FCF");
        _offsetColor = ConvertHexColor("#6DE1FF");

        // 初始化颜色透明度为不透明
        _baseColor.a = 1.0f;
        _offsetColor.a = 1.0f;

        // 获取偏移量以初始化格子属性
        var isOffset = (x + y) % 2 == 1;

        // 根据偏移量获取当前格子颜色
        _renderer.color = isOffset ? _baseColor : _offsetColor;
        Debug.Log($"Tile initialized. isOffset: {isOffset}, color: {_renderer.color}");

    }



}
