using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    private bool isGameOver = false;
    private bool isTutorial = false;
    private bool decreaseGlow = false;

    private bool isEaten = false;
    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        UpdateScoreDisplay();

        Transform textTransform = menuPanel.transform.Find("GameOverText");
        if (textTransform != null)
        {
            menuText = textTransform.GetComponent<Text>();
        }

        textTransform = menuPanel.transform.Find("UserHighScore");
        if (textTransform != null)
        {
            userHighScore = textTransform.GetComponent<Text>();
        }

        textTransform = menuPanel.transform.Find("HighScoreText");
        if (textTransform != null)
        {
            highScoreText = textTransform.GetComponent<Text>();
        }

        Transform btnTransform = menuPanel.transform.Find("StartBtn");
        if (btnTransform != null)
        {
            startBtn = btnTransform.GetComponent<Button>();
            startBtnText = startBtn.GetComponentInChildren<Text>();
            startBtn.onClick.AddListener(OnStartResumeButtonClicked);
        }
        
        menuPanel.SetActive(false);
        LoadHighScore();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))) 
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
        SceneLoader.instance.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowMenu(bool isGameOver)
    {
        menuPanel.SetActive(true);
        scoreText.text = "";

        if (isGameOver)
        {
            userHighScore.text = highScore + " m";
            menuText.text = "Game Over";
            Sprite buttonImg = Resources.Load<Sprite>("Images/PlayImage");
            startBtn.GetComponent<Image>().sprite = buttonImg;
            highScoreText.text = "High Score";
        }
        else
        {   
            userHighScore.text = playerScore + " m";
            menuText.text = "Paused";
            Sprite buttonImg = Resources.Load<Sprite>("Images/ResumeImage");
            startBtn.GetComponent<Image>().sprite = buttonImg;
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
        audioManager.PlaySFX(audioManager.menu);
        audioManager.PlayMusic();
        HideMenu();
    }

    public void StartNewGame()
    {
        HideMenu();
        restartGame();
    }

    public void GameOver(){
        isGameOver = true;
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
        HideMenu();
        SceneLoader.instance.LoadScene("Tutorial");
    }

    public void EndTutorial()
    {
        HideMenu();
        PlayerPrefs.SetInt("HasCompletedTutorial", 1);
        PlayerPrefs.Save();
        SceneLoader.instance.LoadScene("MainGame");
    }

    public void Quit()
    {
        #if UNITY_EDITOR
            Debug.Log("Quit Game");
        #else
            // This will quit the application when running a build
            Application.Quit();
        #endif
    }

    public void PauseGame()
    {
        audioManager.PauseMusic();
        audioManager.PlaySFX(audioManager.menu);
        ShowMenu(false);
        isPaused = true;
    }

    public bool getIsTutorial()
    {
        return isTutorial;
    }

    public void setIsTutorial(bool newIsTutorial)
    {
        isTutorial = newIsTutorial;
    }

    public bool getDecreaseGlow(){
        return decreaseGlow;
    }

    public void setDecreaseGlow(bool b){
        decreaseGlow = b;
    }

    public bool getIsEaten(){
        return isEaten;
    }

    public void setIsEaten(bool b){
        isEaten = b;
    }
}
