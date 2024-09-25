using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogicScript : MonoBehaviour
{
    private int playerScore;
    private int score;
    [SerializeField] Text scoreText;
    [SerializeField] Transform player;
    private float incFactor;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateScoreDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void restartGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
