using UnityEngine;
using System.Collections;
using System.IO;
using System;


public class ScoreKeeper : MonoBehaviour {

	public float score;
	public MeshRenderer scoreRenderer;
	public TextMesh scoreText;
	public GameObject gloveMesh;

	private int[] scoreHistory;
	private GameManager gameManager;
	private string scoreFileName = "MyScores.txt";
	private PlayerManager player;
	private bool areGlovesDisplayed =false;
	private GameObject[] gloves;
	private MeshRenderer chalkBoard;


	// Use this for initialization
	void Start () {

		chalkBoard = GameObject.Find ("Chalkboard").GetComponent<MeshRenderer>();
		gameManager = FindObjectOfType<GameManager> ();
		player = FindObjectOfType<PlayerManager> ();
		scoreText = GetComponent<TextMesh> ();
		TestScoreRecorder();
		scoreRenderer = GetComponent<MeshRenderer>();
	
	}
	
	// Update is called once per frame
	void Update () {
		if (gameManager.isStarted && !gameManager.isGameOver)
		{
			//use if you want bombs to spawn infront of the player.
			//SelfSpawnBombs();
			if (scoreRenderer.enabled == false)
			{
				scoreRenderer.enabled = true;

			}
			if (!areGlovesDisplayed) 
			{
				ShowGloves (player.maxHealth);
				areGlovesDisplayed = true;
			}


			IncreaseScore(Time.deltaTime);
			scoreText.text = Math.Round(score).ToString();
		}

		if (gameManager.isGameOver && !gameManager.isGameOverDisplayed)
		{
			WriteScoreToFile (scoreFileName);
			scoreHistory = ReadScores (scoreFileName);

			gameManager.isGameOverDisplayed = true;
		}
		if (gameManager.isGameOverDisplayed)
		{
			DisplayTop3();
		}
	
	}
	private void DisplayTop3()
	{
		string textForScoreText = "\n Top 3 Scores:\n";
		foreach (int tempscore in scoreHistory)
		{
			textForScoreText = textForScoreText + tempscore.ToString() + "\n";
		}
		scoreText.fontSize = 50;
		scoreText.text = textForScoreText;
	}

	//Displays your life as gloves above the chalkboard.
	private void ShowGloves (int maxHealth)
	{
		gloves = new GameObject[maxHealth];
		Vector3 offset = Vector3.zero + new Vector3(.85f,.8f,0);
		for (int i = 0; i < maxHealth; i++)
		{
			gloves[i] = (GameObject)Instantiate (gloveMesh, this.transform.position + offset, Quaternion.identity);
			gloves [i].transform.parent = this.transform;

			offset.x -= chalkBoard.bounds.size.x/maxHealth;
		}
		
	}

	//Returns a sorted array of the top 3 scores that were written by the WriteScoreToFile function
	private int[] ReadScores(string fileName)
	{
		string[] lines;
		lines = File.ReadAllLines(fileName);

		int[] scoresAsInt = new int[lines.Length];

		for (int i = 0; i < lines.Length; i++)
		{
			scoresAsInt[i] = Convert.ToInt32(lines[i]);
		}

		int[] sortedScores = SortScores(scoresAsInt);
		int[] top3 = new int[3];
		for(int i = 0; i < sortedScores.Length && i < 3; i++)
		{
			top3[i] = sortedScores[i];
		}

		return top3;

	}

	//Writes a new line the scores file.
	private void WriteScoreToFile( string FileName)
	{
		var scoreHistory = File.AppendText(FileName);
		scoreHistory.WriteLine(Math.Round(score,0).ToString());
		scoreHistory.Close();
	}

	private void TestScoreRecorder()
	{
		var scoreHistory = File.AppendText("Test.txt");
		scoreHistory.WriteLine(1);
		scoreHistory.WriteLine(5);
		scoreHistory.WriteLine(7);
		scoreHistory.WriteLine(2);
		scoreHistory.WriteLine(22);
		scoreHistory.Close();

		int[] readScores = ReadScores("Test.txt");

		foreach (int scoreLine in readScores)
		{
			Debug.Log(scoreLine);
		}

	}

	//Conduct a bubble sort
	private int[] SortScores(int[] inputArray)
	{
		int i, j;


		for (j = inputArray.Length - 1; j > 0; j--)
		{
			for(i = 0; i<j; i++)
			{
				if(inputArray[i]< inputArray[i + 1])
				{

					int temporary;

					temporary = inputArray[i+1];
					inputArray[i+1] = inputArray[i];
					inputArray[i] = temporary;
				}
			}
		}
		return inputArray;
	}
	//A public method is used to allow the bosscontroller to increase the score while leaving the score a private variable
	public void IncreaseScore(float increaseBy)
	{
		score += increaseBy;
	}
	public void LoseLife(int currentHealth)
	{
		Destroy (gloves [currentHealth]);
	}
}
