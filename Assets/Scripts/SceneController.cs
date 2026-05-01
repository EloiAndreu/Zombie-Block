using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private static SceneController instance;

    public static SceneController Instance{
        get{
            if (instance == null)
            {
                GameObject gameManagerObject = GameObject.FindGameObjectWithTag("GameManager");
                if(gameManagerObject != null){ 
                    instance = gameManagerObject.AddComponent<SceneController>();
                    //DontDestroyOnLoad(gameManagerObject);
                }
            }
            return instance;
        }
    }

    void Awake(){
        if(instance != null && instance != this){
            Destroy(gameObject);
        }
        else{
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }
    
    public void NextScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }

    public void BeforeScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -1);
    }

    public void LoadSameScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadScene(int num){
        SceneManager.LoadScene(num);
    }

    public void LastScene(){
        int lastSceneIndex = SceneManager.sceneCountInBuildSettings - 1;
        SceneManager.LoadScene(lastSceneIndex);
    }

    public int GetBuildIndexByName(string sceneName){
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (scene.IsValid()){
            return scene.buildIndex;
        }
        else{
            return -1;
        }
    }

    public void QuitGame(){
        Application.Quit();
    }
}
