using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// This class handles game logic, such as: scoring, the menus, and the tutorial.
/// </summary>
public class LogicScript : MonoBehaviour
{
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

    // <summary>
    /// Keeps the AudioManger accessible.
    /// </summary>
    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // <summary>
    /// Sets starting score and binds some screen components to variables.
    /// </summary>
    void Start()
    {
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

    // <summary>
    /// Allows the user to pause the game by pressing SPACE or ESC.
    /// </summary>
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

    // <summary>
    /// Updates the player's current distance travelled in set increments.
    /// </summary>
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
        // for testing changing of score outline based on distance travelled
        //score = (int)(player.position.x * 50);
        if (score > playerScore){
            playerScore = score;
        }
        UpdateScoreDisplay();
    }

    /// <summary>
    /// Visually updates the player's distance travelled and changes the
    /// outline colour based on certain milestones (e.g. > 2500 m means a gold outline).
    /// </summary>
     public void UpdateScoreDisplay()
    {
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

    /// <summary>
    /// Gets the player's all-time highscore from PlayerPrefs.
    /// </summary>
    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    /// <summary>
    /// Updates and saves the player's highscore if a new one is obtained.
    /// </summary>
    public void UpdateHighScore()
    {
        if (playerScore > highScore)
        {
            highScore = playerScore;
            PlayerPrefs.SetInt("HighScore", playerScore);
            PlayerPrefs.Save(); 
        }
    }

    /// <summary>
    /// Restarts the game (after death).
    /// </summary>
    private void restartGame(){
        SceneLoader.instance.LoadScene(SceneManager.GetActiveScene().name);
    }

     /// <summary>
    /// Shows the Pause/Game Over menu and sets its components accordingly.
    /// </summary>
    /// <param name = "isGameOver">Whether the game is over (i.e. player has died) or not.</param>
    public void ShowMenu(bool isGameOver)
    {
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

    /// <summary>
    /// Hides the Pause/Game Over menu.
    /// </summary>
    public void HideMenu()
    {
        menuPanel.SetActive(false);
        pauseBtnImage.enabled = true;
        // Unfreeze the game
        Time.timeScale = 1f; 
        isPaused = false;
        scoreText.text = "";
    }

    /// <summary>
    /// Resumes the current game.
    /// </summary>
    public void ResumeGame()
    {
        audioManager.PlaySFX(audioManager.menu);
        audioManager.PlayMusic();
        HideMenu();
    }

    /// <summary>
    /// Starts a new game.
    /// </summary>
    public void StartNewGame()
    {
        HideMenu();
        restartGame();
    }

    /// <summary>
    /// Player has died/game has ended.
    /// </summary>
    public void GameOver(){
        
        isGameOver = true;
        UpdateHighScore();
        ShowMenu(true);
        audioManager.PauseMusic();
        audioManager.PlaySFX(audioManager.gameOver);
    }

    /// <summary>
    /// Controls which version of the menu is shown based
    /// on whether the player has paused.
    /// </summary>
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

    /// <summary>
    /// Starts the interactive tutorial.
    /// </summary>
    public void OpenTutorial()
    {
        
        HideMenu();
        SceneLoader.instance.LoadScene("Tutorial");
    }

    /// <summary>
    /// Ends the interactive tutorial.
    /// </summary>
    public void EndTutorial()
    {
        
        HideMenu();
        isTutorial = false;
        PlayerPrefs.SetInt("HasCompletedTutorial", 1);
        PlayerPrefs.Save();
        SceneLoader.instance.LoadScene("MainGame");
    }

    /// <summary>
    /// Quits/closes the game.
    /// </summary>
    public void Quit()
    {
        #if UNITY_EDITOR
            Debug.Log("Quit Game");
        #else
            // This will quit the application when running a build
            Application.Quit();
        #endif
    }

    /// <summary>
    /// Pauses the current game.
    /// </summary>
    public void PauseGame()
    {
        audioManager.PauseMusic();
        audioManager.PlaySFX(audioManager.menu);
        ShowMenu(false);
        isPaused = true;
    }

    // Get and set methods for the interactive tutorial to function.

    /// <summary>
    /// Get method for isTutorial variable.
    /// </summary>
    /// <returns>Whether the player is currently doing the tutorial.</returns>
    public bool getIsTutorial()
    {
        
        return isTutorial;
    }

    /// <summary>
    /// Set method for isTutorial variable.
    /// </summary>
    /// <param name = "newIsTutorial">Whether the player has changed to or from the tutorial scene.</param>
    public void setIsTutorial(bool newIsTutorial)
    {
        isTutorial = newIsTutorial;
    }

    /// <summary>
    /// Get method for decreaseGlow variable.
    /// </summary>
    /// <returns>Whether to decrease the bat's glow.</returns>
    public bool getDecreaseGlow(){
        
        return decreaseGlow;
    }

    /// <summary>
    /// Set method for decreaseGlow variable.
    /// </summary>
    /// <param name="b">Whether to change the bat's glow's ability to decay.</param>
    public void setDecreaseGlow(bool b){
        
        decreaseGlow = b;
    }

    /// <summary>
    /// Get method for isEaten variable.
    /// </summary>
    /// <returns>Whether a firefly has been eaten.</returns>
    public bool getIsEaten(){
        
        return isEaten;
    }

    /// <summary>
    /// Set method for isEaten variable.
    /// </summary>
    /// <param name="b">Whether to change the fact that a firefly has been eaten.</param>
    public void setIsEaten(bool b){

        isEaten = b;
    }
}
