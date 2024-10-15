using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

/// <summary>
/// A class that controls the flow of the interactive tutorial.
/// </summary>
public class TutorialManager : MonoBehaviour
{
    [SerializeField] GameObject[] popUps;
    private int popUpIndex;
    [SerializeField] GameObject spawner;
    [SerializeField] GameObject globalLight;
    private Light2D lightComp;
    private float waitTime = 5f;

    private LogicScript logic;

    /// <summary>
    /// Some intialisation to set the tutorial as active.
    /// </summary>
    void Start(){
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();

        popUpIndex = 0;
        spawner.SetActive(false);
        logic.setIsTutorial(true);
        lightComp = globalLight.GetComponent<Light2D>();
        globalLight.SetActive(false);
        lightComp.intensity = 0f;
    }

    /// <summary>
    /// The tutorial shows hints going through five key stages after which it ends.
    /// 1. Horizontal movement.
    /// 2. Vertical movement.
    /// 3. Glow decay.
    /// 4. Firefly consumption.
    /// 5. Obstacle avoidance.
    /// </summary>
    void Update()
    {
        for (int i = 0; i < popUps.Length; i++)
        {
            if (i == popUpIndex)
            {
                popUps[i].SetActive(true);
            } else {
                popUps[i].SetActive(false);
            }
        }

        if (popUpIndex == 0){
            
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                popUpIndex++;
            }

        } else if (popUpIndex == 1){

            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                popUpIndex++;
            }

        } else if (popUpIndex == 2){

            logic.setDecreaseGlow(true);

            if (waitTime <= 0){
                popUpIndex++;
                waitTime = 3f;
                logic.setDecreaseGlow(false);
            } else {
                waitTime -= Time.deltaTime;
            }
            

        } else if (popUpIndex == 3){

            if (waitTime <= 0){
                spawner.SetActive(true);
            }else{
                waitTime -= Time.deltaTime;
            }

            if (logic.getIsEaten()) {
                popUpIndex++;
                waitTime = 1f;
            }
        
        } else if (popUpIndex == 4){
            
            if (waitTime <= 0){
                globalLight.SetActive(true);
                if (lightComp.intensity < 0.2){
                    lightComp.intensity += 0.05f * Time.deltaTime;
                } else {
                    popUpIndex++;
                    waitTime = 4f;
                }
            }else{
                waitTime -= Time.deltaTime;
            }
    

        } else if (popUpIndex == 5) {

            if (waitTime <= 0){       
                logic.EndTutorial();
            }else{
                waitTime -= Time.deltaTime;
            }
    
        }
    }

}
