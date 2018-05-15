using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 2048処理部
public class GameManager : MonoBehaviour
{

    /* ------------------------------- */
    // 盤面
    int[,] board = new int[4, 4]
    {
        { 0, 0, 0, 0},
        { 0, 0, 0, 0},
        { 0, 0, 0, 0},
        { 0, 0, 0, 0}
    };

    /* ------------------------------- */
    // スコア関連
    public Text scoreText;
    public Text scoreUpText;
    public Animator scoreUpAnim;
    public int socre = 0;

    public int upScore = 0;
    /* ------------------------------- */
    // ゲームオーバー関連
    public Text gameOverText;
    /* ------------------------------- */
    // メインゲーム関連
    // メインゲーム関連
    public Text[] textList = new Text[16];
    public bool isPlay = false;// キーが押されたか判定
    public bool isUpdate = false;// 数字が更新されたか判定
    public bool isCalculation = false;// 数字が合体されたか判定

    private MoveAngle enum_NextMoveAngle;// どのキーが押されたかを保存
    /* ------------------------------- */
    // 起動時に1回だけ実行
    void Start()
    {
        Initilize();// 初期化
        StartCoroutine(UpdateCorutine());// 毎フレーム実行するコルーチンを開始
    }

    // 初期化
    void Initilize()
    {
        isPlay = false;
        isUpdate = false;
        isCalculation = false;
        upScore = 0;
    }

    /* ------------------------------- */
    // ずっと実行
    // キーボード取得に使用
    void Update()
    {
        if (!isPlay)// キーを連続で叩けるとバグの原因になるのでフラグで管理
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))// 左
            {
                isPlay = true;
                enum_NextMoveAngle = MoveAngle.Left;
            }

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))// 上
            {
                isPlay = true;
                enum_NextMoveAngle = MoveAngle.Up;
            }

            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))// 右
            {
                isPlay = true;
                enum_NextMoveAngle = MoveAngle.Right;
            }

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))// 下
            {
                isPlay = true;
                enum_NextMoveAngle = MoveAngle.Down;
            }
        }
    }
    /* ------------------------------- */
    // ずっと実行
    // ゲームの進行を管理
    // コルーチンを使用するとゲームの処理を一時停止できるため使用
    IEnumerator UpdateCorutine()
    {

        Reset();// 初期化

        while (true)// While(true)にすることで永久ループするようにする
        {

            yield return new WaitUntil(() => isPlay); //isPlayフラグがTRUEになるまで処理を一時停止する キーボードを押すとTRUEになる
            Initilize();// 初期化
            MoveBoard();// 盤面の数字を移動させる
            if (isUpdate)// 盤面の数字が動いたら実行
            {
                if (isCalculation)
                    PlayScoreUpAnimation();// スコアアップのアニメーションを再生
                GenerateNumber();// 盤面上に数字をランダムに生成
                UpdateDisplay();// 画面を更新
                ChangeScaleText();// 3桁以上の数字になると数字が見切れるので文字サイズを調整する
                CheckGameOver();// ゲームオーバー判定
            }

        }

    }
    /* ------------------------------- */
    // 数字を生成
    void GenerateNumber()
    {
        int counter = 0;
        int randNumX = Random.Range(0, 4);// ランダムに0~4の数字を生成
        int randNumY = Random.Range(0, 4);// ランダムに0~4の数字を生成

        // 生成できない場合は合いているマスを探索する
        while (counter <= 15)
        {
            counter++;
            if (board[randNumX, randNumY] == 0)// 調べたマスが0だったら2を生成
            {
                board[randNumX, randNumY] = 2;
                break;
            }
            else// 調べたマスが0ではなかったら次のマスを探索
            {
                if (randNumX < 3)
                {
                    randNumX++;
                }
                else
                {
                    if (randNumY < 3)
                    {
                        randNumX = 0;
                        randNumY++;
                    }
                    else
                    {
                        randNumX = 0;
                        randNumY = 0;
                    }
                }
            }
            if (counter >= 16)// 全てのマスを探索し終えたら終了
                break;
        }
    }
    /* ------------------------------- */
    // 画面を更新
    void UpdateDisplay()
    {
        int counterX = 0;
        int counterY = 0;
        foreach (var value in textList)//変数Boardを画面の数字テキストに全て反映させる
        {
            value.text = board[counterX, counterY].ToString();
            counterX++;
            if (counterX > 3)
            {
                counterX = 0;
                counterY++;
            }
        }
    }
    /* ------------------------------- */
    // 数字を移動させる
    void MoveBoard()
    {

        // 盤面を移動
        for (int i = 0; i < 3; i++)
        {
            for (int x = 0; x <= 3; x++)
            {
                for (int y = 0; y <= 3; y++)
                {
                    if (board[x, y] != 0)
                    {
                        switch (enum_NextMoveAngle)// enum_NextMoveAngleの中身はUpdate関数で変更
                        {
                            case MoveAngle.Left:
                                Move(x, y, x - 1, y);
                                break;
                            case MoveAngle.Right:
                                Move(x, y, x + 1, y);
                                break;
                            case MoveAngle.Up:
                                Move(x, y, x, y - 1);
                                break;
                            case MoveAngle.Down:
                                Move(x, y, x, y + 1);
                                break;
                        }
                    }
                }
            }
        }

        // 数値を更新
        switch (enum_NextMoveAngle)
        {

            case MoveAngle.Left:
                for (int x = 0; x <= 3; x++)
                {
                    for (int y = 0; y <= 3; y++)
                    {
                        if (board[x, y] != 0)
                        {
                            CalculationCheckAndUpdate(x, y, x - 1, y);
                        }
                    }
                }
                break;

            case MoveAngle.Right:
                for (int x = 3; x >= 0; x--)
                {
                    for (int y = 3; y >= 0; y--)
                    {
                        if (board[x, y] != 0)
                        {
                            CalculationCheckAndUpdate(x, y, x + 1, y);
                        }
                    }
                }
                break;

            case MoveAngle.Up:
                for (int x = 0; x <= 3; x++)
                {
                    for (int y = 0; y <= 3; y++)
                    {
                        if (board[x, y] != 0)
                        {
                            CalculationCheckAndUpdate(x, y, x, y - 1);
                        }
                    }
                }
                break;

            case MoveAngle.Down:
                for (int x = 3; x >= 0; x--)
                {
                    for (int y = 3; y >= 0; y--)
                    {
                        if (board[x, y] != 0)
                        {
                            CalculationCheckAndUpdate(x, y, x, y + 1);
                        }
                    }
                }
                break;
        }

        // 盤面を移動
        for (int x = 0; x <= 3; x++)
        {
            for (int y = 0; y <= 3; y++)
            {
                if (board[x, y] != 0)
                {
                    switch (enum_NextMoveAngle)
                    {
                        case MoveAngle.Left:
                            Move(x, y, x - 1, y);

                            break;

                        case MoveAngle.Right:
                            Move(x, y, x + 1, y);

                            break;

                        case MoveAngle.Up:
                            Move(x, y, x, y - 1);

                            break;

                        case MoveAngle.Down:
                            Move(x, y, x, y + 1);

                            break;
                    }
                }
            }
        }

    }
    /* ------------------------------- */
    // 数字を計算して更新まで行う(画面の更新は行わない)
    void CalculationCheckAndUpdate(int currentCellX, int currentCellY, int nextCellX, int nextCellY)
    {

        // 数値更新
        // 存在しないマスを読んだ場合はtry catchを利用して実行しない
        try
        {
            if (board[currentCellX, currentCellY] == board[nextCellX, nextCellY])
            {
                board[currentCellX, currentCellY] = 0;
                board[nextCellX, nextCellY] = board[nextCellX, nextCellY] * 2;
                UpdateScore(board[nextCellX, nextCellY]);
                isUpdate = true;
                isCalculation = true;
            }
        }
        catch { }
    }
    /* ------------------------------- */
    void Move(int currentCellX, int currentCellY, int nextCellX, int nextCellY)
    {

        // 移動
        // 存在しないマスを読んだ場合はtry catchを利用して実行しない
        try
        {
            if (board[nextCellX, nextCellY] == 0)
            {
                board[nextCellX, nextCellY] = board[currentCellX, currentCellY];
                board[currentCellX, currentCellY] = 0;
                isUpdate = true;
            }
        }
        catch { }
    }
    /* ------------------------------- */
    // ゲームオーバー確認 ゲームオーバーの遷移はここで行っている
    void CheckGameOver()
    {

        // 0がマスにあったら終了
        for (int x = 0; x <= 3; x++)
            for (int y = 0; y <= 3; y++)
                if (board[x, y] == 0)
                    return;

        // 前後左右に同じ数字のマスがないか探索 あったら終了
        // 存在しないマスを読んだ場合はtry catchを利用して終了
        for (int x = 0; x <= 3; x++)
            for (int y = 0; y <= 3; y++)
            {
                try
                {
                    if (board[x, y] == board[x + 1, y])
                        return;
                }
                catch { }
                try
                {
                    if (board[x, y] == board[x - 1, y])
                        return;
                }
                catch { }
                try
                {
                    if (board[x, y] == board[x, y + 1])
                        return;
                }
                catch { }
                try
                {
                    if (board[x, y] == board[x, y - 1])
                        return;
                }
                catch { }
            }

        // ゲームオーバー処理へ移行
        GameOver();
    }
    /* ------------------------------- */
    // ゲームオーバー処理
    void GameOver()
    {
        if (gameOverText)// gameOverTextがアタッチされていてたら実行
            gameOverText.text = "GameOver";
    }
    /* ------------------------------- */
    // スコア更新
    void UpdateScore(int num)
    {
        socre += num;
        scoreText.text = socre.ToString();
        upScore += num;
        scoreUpText.text = upScore.ToString();
    }
    // スコア更新のアニメーションを再生
    void PlayScoreUpAnimation()
    {
        if (scoreUpAnim)// scoreUpAnimがアタッチされていたら実行
            scoreUpAnim.Play("ScoreUp", 0, 0.0f);
    }
    /* ------------------------------- */
    // リセットボタン用
    public void Reset()
    {
        if (gameOverText)// gameOverTextがアタッチされていてたら実行
            gameOverText.text = "";
        scoreText.text = "0";
        socre = 0;

        for (int x = 0; x <= 3; x++)
            for (int y = 0; y <= 3; y++)
                board[x, y] = 0;

        UpdateDisplay();// スコアアップのアニメーションを再生
        ChangeScaleText();// 3桁以上の数字になると数字が見切れるので文字サイズを調整する
        GenerateNumber();// 盤面上に数字をランダムに生成   
        UpdateDisplay();// GenerateNumberを反映させるために再度ディスプレイ描画を行う

    }
    /* ------------------------------- */
    // 数字の大きさを桁数に応じて変更
    public void ChangeScaleText()
    {

        foreach (var cell in textList)
        {
            var value = int.Parse(cell.text);// 桁数を取得
            switch (Digit(value))
            {
                case 0:
                    break;
                case 1:
                    cell.fontSize = 80;
                    break;
                case 2:
                    cell.fontSize = 80;
                    break;
                case 3:
                    cell.fontSize = 50;
                    break;
                case 4:
                    cell.fontSize = 30;
                    break;
                default:
                    cell.fontSize = 10;
                    break;
            }
        }
    }

    // 数値の桁数を調べる
    private int Digit(int num)
    {
        // 対数(log10)を取って調べる
        return (num == 0) ? 1 : ((int)Mathf.Log10(num) + 1);
    }

}

// キー入力を管理するEnum
enum MoveAngle
{
    Right,
    Left,
    Up,
    Down
}