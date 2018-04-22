using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 2048処理部
public class GameManager : MonoBehaviour {

/* ------------------------------- */
	// 盤面
	int[,] board = new int[4,4]
	{
    	{ 0, 0, 0, 0},
    	{ 0, 0, 0, 0},
    	{ 0, 0, 0, 0},
    	{ 0, 0, 0, 0}
	};

/* ------------------------------- */
	// スコア関連
	[SerializeField]
	Text scoreText;
	[SerializeField]
	private int socre = 0;
/* ------------------------------- */
	// ゲームオーバー関連
	[SerializeField]
	private Text gameOverText;
/* ------------------------------- */
	// メインゲーム関連
	[SerializeField]
	Text[] textList = new Text[16];
	[SerializeField]
	private bool isPlay = false;// キーが押されたか判定
	[SerializeField]
	private bool isUpdate = false;// 数字が更新されたか判定

	private Enum.MoveAngle enum_NextMoveAngle;// どのキーが押されたかを保存
/* ------------------------------- */
	
	void Start () {
		Initilize ();
		StartCoroutine (UpdateCorutine ());
	}

	// 初期化
	void Initilize(){
		isPlay = false;
		isUpdate = false;
	}

/* ------------------------------- */
	// キーボード取得に使用
	void Update(){
		if (!isPlay) {
			if (Input.GetKeyDown (KeyCode.A)||Input.GetKeyDown (KeyCode.LeftArrow)) {
				isPlay = true;
				enum_NextMoveAngle = Enum.MoveAngle.Left;
			}

			if (Input.GetKeyDown (KeyCode.W)||Input.GetKeyDown (KeyCode.UpArrow)) {
				isPlay = true;
				enum_NextMoveAngle = Enum.MoveAngle.Up;
			}

			if (Input.GetKeyDown (KeyCode.D)||Input.GetKeyDown (KeyCode.RightArrow)) {
				isPlay = true;
				enum_NextMoveAngle = Enum.MoveAngle.Right;
			}

			if (Input.GetKeyDown (KeyCode.S)||Input.GetKeyDown (KeyCode.DownArrow)) {
				isPlay = true;
				enum_NextMoveAngle = Enum.MoveAngle.Down;
			}
		}
	}
/* ------------------------------- */
	// ゲームの進行を管理
	IEnumerator UpdateCorutine(){

		Reset();

		while (true) {

			yield return new WaitUntil (() => isPlay);
			Initilize ();
			MoveBoard ();
			if (isUpdate) {
				GenerateNumber ();
				UpdateDisplay ();
				CheckGameOver ();
			}

		}
			
	}
/* ------------------------------- */
	// 数字を生成
	void GenerateNumber(){
		int counter = 0;
		int randNumX = Random.Range (0, 4);
		int randNumY = Random.Range (0, 4);

		// 生成できない場合は合いているマスを探索する
		while (counter <= 15) {
			counter++;
			if (board [randNumX, randNumY] == 0) {
				board [randNumX, randNumY] = 2;
				break;
			}
			else {
				if (randNumX < 3) {
					randNumX++;
				}
				else {
					if (randNumY < 3) {
						randNumX = 0;
						randNumY++;
					} else {
						randNumX = 0;
						randNumY = 0;
					}
				}
			}
			if (counter >= 16)
				break;
		}
	}
/* ------------------------------- */
	// 画面を更新
	void UpdateDisplay(){
		int counterX = 0;
		int counterY = 0;
		foreach (var value in textList){
			value.text = board[counterX,counterY].ToString();
			counterX++;
			if(counterX > 3){
				counterX = 0;
				counterY++ ;
			}
		}
	}
/* ------------------------------- */
	// 数字を移動させる コードを短くするためにtry catchを使用　パフォーマンスに問題ありなのは把握しているのでリリースするなら要修正
	void MoveBoard(){

		// 移動
		for (int i = 0; i < 3; i++) {
			for (int x = 0; x <= 3; x++) {
				for (int y = 0; y <= 3; y++) {
					if (board [x, y] != 0) {
						switch (enum_NextMoveAngle) {
						case Enum.MoveAngle.Left:
							Move (x, y, x - 1, y);
							break;
						case Enum.MoveAngle.Right:
							Move (x, y, x + 1, y);
							break;
						case Enum.MoveAngle.Up:
							Move (x, y, x, y - 1);
							break;
						case Enum.MoveAngle.Down:
							Move (x, y, x, y + 1);
							break;
						}
					}
				}
			}
		}
		// 数値更新
		switch (enum_NextMoveAngle) {
			
			case Enum.MoveAngle.Left:
			for (int x = 0; x <= 3; x++) {
				for (int y = 0; y <= 3; y++) {
					if (board [x, y] != 0) {
						CalculationCheckAndUpdate (x, y, x - 1, y);			
					}
				}
			}
			break;
			
			case Enum.MoveAngle.Right:
			for (int x = 3; x >= 0; x--) {
				for (int y = 3; y >= 0; y--) {
					if (board [x, y] != 0) {
						CalculationCheckAndUpdate (x, y, x + 1, y);		
					}
				}
			}
			break;
			
			case Enum.MoveAngle.Up:
			for (int x = 0; x <= 3; x++) {
				for (int y = 0; y <= 3; y++) {
					if (board [x, y] != 0) {
						CalculationCheckAndUpdate (x, y, x, y - 1);
					}
				}
			}
			break;
			
			case Enum.MoveAngle.Down:
			for (int x = 3; x >= 0; x--) {
				for (int y = 3; y >= 0; y--) {
					if (board [x, y] != 0) {
						CalculationCheckAndUpdate (x, y, x, y + 1);
					}
				}
			}
			break;
		}// 移動
		for (int x = 0; x <= 3; x++) {
			for (int y = 0; y <= 3; y++) {
				if (board [x, y] != 0) {
					switch (enum_NextMoveAngle) {
					case Enum.MoveAngle.Left:
						Move (x, y, x - 1, y);

						break;

					case Enum.MoveAngle.Right:
						Move (x, y, x + 1, y);

						break;

					case Enum.MoveAngle.Up:
						Move (x, y, x, y - 1);

						break;

					case Enum.MoveAngle.Down:
						Move (x, y, x, y + 1);

						break;
					}
				}
			}
		}

	}
/* ------------------------------- */
	// 数字を計算して更新まで行う(画面の更新は行わない)
	void CalculationCheckAndUpdate(int currentCellX,int currentCellY,int nextCellX,int nextCellY){

		// 数値更新
		try {
			if(board[currentCellX,currentCellY] == board[nextCellX,nextCellY]){
				board[currentCellX,currentCellY] = 0;
				board[nextCellX,nextCellY]= board[nextCellX,nextCellY] * 2;
				UpdateScore(board[nextCellX,nextCellY]);
				isUpdate = true;
			}
		} catch {}
	}
/* ------------------------------- */
	void Move(int currentCellX,int currentCellY,int nextCellX,int nextCellY){

		// 移動
		try {
			if(board[nextCellX,nextCellY] == 0){
				board[nextCellX,nextCellY]= board[currentCellX,currentCellY];
				board[currentCellX,currentCellY] = 0;
				isUpdate = true;
			}
		} catch {}
	}
/* ------------------------------- */
	// ゲームオーバー確認 ゲームオーバーの遷移はここで行っている
	void CheckGameOver(){
		
		// 0がマスにあったら終了
		for (int x = 0; x <= 3; x++) 
			for (int y = 0; y <= 3; y++) 
				if (board [x, y] == 0)
					return;
			
		// 前後左右に同じ数字のマスがないか探索 あったら終了
		for (int x = 0; x <= 3; x++)
			for (int y = 0; y <= 3; y++) {
				try {
					if (board [x, y] == board [x + 1, y])
						return;
				} catch {}
				try {
					if (board [x, y] == board [x - 1, y])
						return;
				} catch {}
				try {
					if (board [x, y] == board [x, y + 1])
						return;
				} catch {}
				try {
					if (board [x, y] == board [x, y - 1])
						return;
				} catch {}
			}

		// ゲームオーバー処理へ移行
		GameOver();
	}
/* ------------------------------- */
	// ゲームオーバー処理
	void GameOver(){
		gameOverText.text = "GameOver";
	}
/* ------------------------------- */
	// スコア更新
	void UpdateScore(int num){
		socre += num;
		scoreText.text = socre.ToString();
	}
/* ------------------------------- */
	// リセットボタン用
	public void Reset(){
		gameOverText.text = "";
		scoreText.text = "0";
		socre = 0;

		for (int x = 0; x <= 3; x++) 
			for (int y = 0; y <= 3; y++) 
				board[x,y] = 0;
		
		GenerateNumber ();
		UpdateDisplay ();
	}

}
