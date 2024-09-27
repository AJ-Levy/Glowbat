using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LogicScript : MonoBehaviour
{
    private int playerScore;
    private int score;
    private int highScore;
    [SerializeField] Text scoreText;
    [SerializeField] Transform player;
    private float incFactor;

    [SerializeField] GameObject menuPanel;
    private Text highScoreText;
    private Text userHighScore;
    private Text menuText;
    private Button startBtn;
    private Text startBtnText;
    private bool isPaused = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateScoreDisplay();

        Transform textTransform = menuPanel.transform.Find("GameOverText");
        menuText = textTransform.GetComponent<Text>();

        textTransform = menuPanel.transform.Find("UserHighScore");
        userHighScore = textTransform.GetComponent<Text>();

        textTransform = menuPanel.transform.Find("HighScoreText");
        highScoreText = textTransform.GetComponent<Text>();

        Transform btnTransform = menuPanel.transform.Find("StartBtn");
        startBtn = btnTransform.GetComponent<Button>();
        startBtnText = startBtn.GetComponentInChildren<Text>();
        startBtn.onClick.AddListener(OnStartResumeButtonClicked);
        
        menuPanel.SetActive(false);
        LoadHighScore();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space)) 
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void UpdateScore()
    {
        if (score < 100) {
            incFactor = 5f;
        }
        else if (score < 1000) {
            incFactor = 10f;
        } else {
            incFactor = 100f;
        } 
        
        // Calculate score in meters, round to the nearest incFactor
        score = (int)(Mathf.Floor(player.position.x / incFactor) * incFactor);
        if (score > playerScore){
            playerScore = score;
        }
        UpdateScoreDisplay();
    }

     public void UpdateScoreDisplay()
    {
        // Update the score text in the UI
        if (scoreText != null)
        {
            scoreText.text = playerScore + " m";
        }
    }

    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    public void UpdateHighScore()
    {
        if (playerScore > highScore)
        {
            highScore = playerScore;
            PlayerPrefs.SetInt("HighScore", playerScore);
            PlayerPrefs.Save(); 
        }
    }

    private void restartGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowMenu(bool isGameOver)
    {
        menuPanel.SetActive(true);
        scoreText.text = "";

        if (isGameOver)
        {
            userHighScore.text = highScore + " m";
            menuText.text = "Game Over";
            startBtnText.text = "Play Again";
            highScoreText.text = "High Score";
        }
        else
        {   
            userHighScore.text = playerScore + " m";
            menuText.text = "Paused";
            startBtnText.text = "Resume";
            highScoreText.text = "Current Score";
        }

         // Freeze the game when the menu is shown
        Time.timeScale = 0f;
    }

    public void HideMenu()
    {
        menuPanel.SetActive(false);
        // Unfreeze the game
        Time.timeScale = 1f; 
        isPaused = false;
        scoreText.text = "";
    }

    public void ResumeGame()
    {
        HideMenu();
    }

    public void StartNewGame()
    {
        HideMenu();
        restartGame();
    }

    public void GameOver(){
        UpdateHighScore();
        ShowMenu(true);
    }

    private void OnStartResumeButtonClicked()
    {
        if (isPaused)
        {
            ResumeGame(); 
        }
        else
        {
            StartNewGame();
        }
    }

    public void OpenTutorial()
    {
        // Logic to show the tutorial screen
    }

    public void PauseGame()
    {
        ShowMenu(false);
        isPaused = true;
    }
}
