using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState{
    GenerateGrid = 0,
    SpawnHeroes = 1,
    SpawnEnemies = 2,
    Battle = 3,
}


public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public GameState GameState;


    void Awake(){
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Changestate(GameState.GenerateGrid);
    }

    public void Changestate(GameState newState){

        GameState = newState;

        switch (newState){

            case GameState.GenerateGrid:

                // 生成格子
                GridManager.Instance.GenerateGridBF1();
                break;

            case GameState.SpawnHeroes:
                break;

            case GameState.SpawnEnemies:
                break;



            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);

        }


    }


}
