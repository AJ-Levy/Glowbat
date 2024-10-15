using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogicScript : MonoBehaviour
{
    /// <summary>
    /// This class handles game logic, such as: scoring, the menus, and the tutorial.
    /// </summary>
    private int playerScore;
    private int score;
    private int highScore;

    private Outline textOutline;
    [SerializeField] Text scoreText;
    [SerializeField] Transform player;
    private float incFactor;

    [SerializeField] GameObject menuPanel;
    private Text highScoreText;
    private Text userHighScore;
    private Text menuText;
    private Button startBtn;
    private Text startBtnText;
    private Button pauseBtn;
    private Image pauseBtnImage;
    private bool isPaused = false;
    private bool isGameOver = false;
    private bool isTutorial = false;
    private bool decreaseGlow = false;

    private bool isEaten = false;
    AudioManager audioManager;

    private void Awake()
    {
        // <summary>
        /// Keeps the AudioManger accessible.
        /// </summary>
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Start()
    {
        // <summary>
        /// Sets starting score and binds some screen components to variables.
        /// </summary>
        textOutline = scoreText.GetComponent<Outline>();
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

        GameObject pauseBtnObject = GameObject.Find("Pause");
        if (pauseBtnObject != null)
        {
            pauseBtn = pauseBtnObject.GetComponent<Button>();
            pauseBtnImage = pauseBtn.GetComponent<Image>();
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
        // <summary>
        /// Allows the user to pause the game by pressing SPACE or ESC.
        /// </summary>
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
        // <summary>
        /// Updates the player's current distance travelled in set increments.
        /// </summary>
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
        // for testing changing of score outline based on distance travelled
        //score = (int)(player.position.x * 50);
        if (score > playerScore){
            playerScore = score;
        }
        UpdateScoreDisplay();
    }

     public void UpdateScoreDisplay()
    {
        /// <summary>
        /// Visually updates the player's distance travelled and changes the
        /// outline colour based on certain milestones (e.g. > 2500 m means a gold outline).
        /// </summary>
    
        scoreText.text = playerScore + " m";
        if (playerScore < 100)
        {
            // white
            textOutline.effectColor = new Color32(255, 255, 255, 0);
        } else if (playerScore < 500){
            // blue
            textOutline.effectColor = new Color32(4, 146, 194, 255);
        } else if (playerScore < 1000){
            // bronze
            textOutline.effectColor = new Color32(205, 127, 50, 255);
        } else if (playerScore < 2500){
            // silver
            textOutline.effectColor = new Color32(138, 141, 143, 255);
        } else{
            // gold
            textOutline.effectColor = new Color32(212, 175, 55, 255);
        } 
        
        
    }

    private void LoadHighScore()
    {
        /// <summary>
        /// Gets the player's all-time highscore from PlayerPrefs.
        /// </summary>
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    public void UpdateHighScore()
    {
        /// <summary>
        /// Updates and saves the player's highscore if a new one is obtained.
        /// </summary>
        if (playerScore > highScore)
        {
            highScore = playerScore;
            PlayerPrefs.SetInt("HighScore", playerScore);
            PlayerPrefs.Save(); 
        }
    }

    private void restartGame(){
        /// <summary>
        /// Restarts the game (after death).
        /// </summary>
        SceneLoader.instance.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowMenu(bool isGameOver)
    {
        /// <summary>
        /// Shows the Pause/Game Over menu and sets its components accordingly.
        /// </summary>
        /// <param name = "isGameOver">Whether the game is over (i.e. player has died) or not.</param>
        menuPanel.SetActive(true);
        scoreText.text = "";
        pauseBtnImage.enabled = false;

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
        /// <summary>
        /// Hides the Pause/Game Over menu.
        /// </summary>
        menuPanel.SetActive(false);
        pauseBtnImage.enabled = true;
        // Unfreeze the game
        Time.timeScale = 1f; 
        isPaused = false;
        scoreText.text = "";
    }

    public void ResumeGame()
    {
        /// <summary>
        /// Resumes the current game.
        /// </summary>
        audioManager.PlaySFX(audioManager.menu);
        audioManager.PlayMusic();
        HideMenu();
    }

    public void StartNewGame()
    {
        /// <summary>
        /// Starts a new game.
        /// </summary>
        HideMenu();
        restartGame();
    }

    public void GameOver(){
        /// <summary>
        /// Player has died/game has ended.
        /// </summary>
        isGameOver = true;
        UpdateHighScore();
        ShowMenu(true);
        audioManager.PauseMusic();
        audioManager.PlaySFX(audioManager.gameOver);
    }

    private void OnStartResumeButtonClicked()
    {
        /// <summary>
        /// Controls which version of the menu is shown based
        /// on whether the player has paused.
        /// </summary>
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
        /// <summary>
        /// Starts the interactive tutorial.
        /// </summary>
        HideMenu();
        SceneLoader.instance.LoadScene("Tutorial");
    }

    public void EndTutorial()
    {
        /// <summary>
        /// Ends the interactive tutorial.
        /// </summary>
        HideMenu();
        isTutorial = false;
        PlayerPrefs.SetInt("HasCompletedTutorial", 1);
        PlayerPrefs.Save();
        SceneLoader.instance.LoadScene("MainGame");
    }

    public void Quit()
    {
        /// <summary>
        /// Quits/closes the game.
        /// </summary>
        #if UNITY_EDITOR
            Debug.Log("Quit Game");
        #else
            // This will quit the application when running a build
            Application.Quit();
        #endif
    }

    public void PauseGame()
    {
        /// <summary>
        /// Pauses the current game.
        /// </summary>
        audioManager.PauseMusic();
        audioManager.PlaySFX(audioManager.menu);
        ShowMenu(false);
        isPaused = true;
    }

    // Get and set methods for the interactive tutorial to function.
    public bool getIsTutorial()
    {
        /// <summary>
        /// Get method for isTutorial variable.
        /// </summary>
        /// <returns>Whether the player is currently doing the tutorial.</returns>
        return isTutorial;
    }

    public void setIsTutorial(bool newIsTutorial)
    {
        /// <summary>
        /// Set method for isTutorial variable.
        /// </summary>
        /// <param name = "newIsTutorial">Whether the player has changed to or from the tutorial.</param>
        isTutorial = newIsTutorial;
    }

    public bool getDecreaseGlow(){
        /// <summary>
        /// Get method for decreaseGlow variable.
        /// </summary>
        /// <returns>Whether to decrease the bat's glow.</returns>
        return decreaseGlow;
    }

    public void setDecreaseGlow(bool b){
        /// <summary>
        /// Set method for decreaseGlow variable.
        /// </summary>
        /// <param name="b">Whether to change the bat's glow's ability to decay.</param>
        decreaseGlow = b;
    }

    public bool getIsEaten(){
        /// <summary>
        /// Get method for isEaten variable.
        /// </summary>
        /// <returns>Whether a firefly has been eaten.</returns>
        return isEaten;
    }

    public void setIsEaten(bool b){
        /// <summary>
        /// Set method for isEaten variable.
        /// </summary>
        /// <param name="b">Whether to change the fact that a firefly has been eaten.</param>
        isEaten = b;
    }
}
