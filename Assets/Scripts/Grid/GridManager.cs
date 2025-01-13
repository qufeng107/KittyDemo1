using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [Header("P1战斗场地(BF1)")]
    [SerializeField] private int _width = 6; // 棋盘的列数
    [SerializeField] private int _height = 5; // 棋盘的行数
    [SerializeField] private Tile _tilePrefabBF1; // 格子预制体
    [SerializeField] private float _tileSize = 0.7f; // 每个格子的默认大小（1个单位）
    [SerializeField] private float _spacing = 0f; // 每个格子之间的间隙
    private Dictionary<Vector2, Tile> _tilesBF1; // 格子字典

    [Header("相机位置")]
    [SerializeField] private Transform _cam; // 相机

    [Header("交互设置")]
    [SerializeField] private InputActionAsset inputActions; // 输入配置文件
    private InputAction positionPrimaryTouchAction; // 捕获鼠标或触屏位置
    private InputAction primaryTouchPressAction; // 捕获按压状态
    private Tile _currentHighlightedTile; // 当前高光的 Tile


    void Awake(){
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("GridManager Start called.");
        // 检查是否绑定了 InputActionAsset
        if (inputActions == null)
        {
            Debug.LogError("InputActionAsset is not assigned in the Inspector!");
            return;
        }
        Debug.Log("InputActionAsset is assigned.");

        // 获取位置 Action
        positionPrimaryTouchAction = inputActions.FindAction("PositionPrimaryTouch");
        if (positionPrimaryTouchAction == null)
        {
            Debug.LogError("Action 'PositionPrimaryTouch' not found in InputActionAsset!");
            return;
        }
        Debug.Log("Action 'PositionPrimaryTouch' found.");

        // 获取按压 Action
        primaryTouchPressAction = inputActions.FindAction("PressPrimaryTouch"); 
        if (primaryTouchPressAction == null)
        {
            Debug.LogError("Action 'PressPrimaryTouch' not found in InputActionAsset!");
            return;
        }
        Debug.Log("Action 'PressPrimaryTouch' found.");


        // 启用输入 Action
        inputActions.Enable();
        positionPrimaryTouchAction.Enable();
        primaryTouchPressAction.Enable();
        Debug.Log("Action enabled.");

    }

    // Update is called once per frame
    void Update()
    {

        // 格子高光事件
        HandleTileHighlightBF1();
    }


    // 生成格子
    public void GenerateGridBF1(){

        // 初始化格子字典
        _tilesBF1 = new Dictionary<Vector2, Tile>();

        for (int x = 0; x < _width; x++){
            for (int y = 0; y < _height; y++){
                // 计算格子的位置
                float posX = x * (_tileSize + _spacing);
                float posY = y * (_tileSize + _spacing);

                // 创建格子对象
                var spawnedTile = Instantiate(_tilePrefabBF1, new Vector3(posX, posY, 0), Quaternion.identity);
                // 设置格子名称
                spawnedTile.name = $"Tile_{x}_{y}";
                // 设置格子缩放大小
                spawnedTile.transform.localScale = new Vector3(_tileSize, _tileSize, 1.0f);


                // 初始化格子
                spawnedTile.Init(x, y);

                // 存储格子
                _tilesBF1[new Vector2(x,y)] = spawnedTile;
            }
        }

        // 使用世界坐标系，直接移动摄像头到棋盘中心位置，而不是移动棋盘到原摄像头位置
        float gridWidth = _width * (_tileSize + _spacing) - _spacing; // 棋盘的总宽度
        float gridHeight = _height * (_tileSize + _spacing) - _spacing; // 棋盘的总高度
        _cam.transform.position = new Vector3(gridWidth / 2 - _tileSize / 2, gridHeight / 2 - _tileSize / 2, -10);
    

        // 修改阶段
        GameManager.Instance.Changestate(GameState.SpawnHeroes);

    }

    // 根据下标获取格子对象
    public Tile GetTileAtPositionBF1(Vector2 pos){
        if (_tilesBF1.TryGetValue(pos, out var tile)){
            return tile;
        }

        return null;
    }


    // 格子高光检测
    void HandleTileHighlightBF1(){

        // 检测输入是否启用
        if (positionPrimaryTouchAction == null){
            Debug.LogError("PositionPrimaryTouch Action is null.");
            return;
        }

        // TODO 检查是否有选中拖动棋子



        // 检查主输入是否释放按压
        if (primaryTouchPressAction.ReadValue<float>() == 0) // 0 表示输入松开
        {
            // 已释放，重置高光
            if (_currentHighlightedTile != null)
            {
                _currentHighlightedTile.SetHighlight(false);
                Debug.Log($"Removed highlight from: {_currentHighlightedTile.name}");
                _currentHighlightedTile = null;
            }
            return;
        }

        // 如果主输入未释放，判断是否发生碰撞
        // 获取输入位置
        Vector2 inputPosition = positionPrimaryTouchAction.ReadValue<Vector2>();
        Debug.Log($"Input position: {inputPosition}");

        // 转换为世界坐标
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, 0));
        Vector2 worldPosition2D = new Vector2(worldPosition.x, worldPosition.y);
        Debug.Log($"World position: {worldPosition2D}");
        
        // 检测当前输入位置 是否触碰某个格子的碰撞体
        Collider2D hitCollider = Physics2D.OverlapPoint(worldPosition2D);

        // 如果某个碰撞体激活
        if (hitCollider != null){
            Debug.Log($"Collider hit: {hitCollider.name}");

            // 获取碰撞体对应的格子对象
            if (hitCollider.TryGetComponent<Tile>(out Tile tile)){

                // 如果之前的格子和当前格子不一致，更新高光
                if (_currentHighlightedTile != tile){

                    // 取消之前的格子高光
                    if (_currentHighlightedTile != null){
                        _currentHighlightedTile.SetHighlight(false);
                        Debug.Log($"Highlight removed from Tile: {_currentHighlightedTile.name}");
                    }

                    // 激活新的格子高光
                    _currentHighlightedTile = tile;
                    _currentHighlightedTile.SetHighlight(true);
                    Debug.Log($"Highlight set on Tile: {_currentHighlightedTile.name}");
                }
            }
        }

        // 如果没有碰撞任何格子
        else {
            // 重置高光
            if (_currentHighlightedTile != null){
                _currentHighlightedTile.SetHighlight(false);
                Debug.Log($"Highlight removed from Tile: {_currentHighlightedTile.name}");

                _currentHighlightedTile = null;
            }
        }

    }





}
