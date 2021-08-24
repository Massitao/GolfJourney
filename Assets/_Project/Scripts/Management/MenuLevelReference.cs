using UnityEngine;

public class MenuLevelReference : MonoBehaviour {

    LevelManager lvlManager;

    private void Awake()
    {
        lvlManager = LevelManager.instance;
    }

    public void LevelLoad(string levelName)
    {
        lvlManager.LoadLevel(levelName);
    }

    public void Quit()
    {
        lvlManager.ApplicationQuit();
    }

}
