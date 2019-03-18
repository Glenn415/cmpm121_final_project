using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Script : MonoBehaviour {
    // 0: Main Menu
    // 1: Game
    public void PlayGame() {
        // load the first level
        SceneManager.LoadScene(1);
    }

    public void QuitGame(){
        // quit game
        Application.Quit();
    }
}
