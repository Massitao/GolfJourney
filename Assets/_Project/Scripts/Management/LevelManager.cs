using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager> {

    public float waitForLoadTimer = 1.5f;
    public bool once;

    public Scene scene;

    Animator fader;

    // Use this for initialization
    void Start()
    {
        fader = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (scene != SceneManager.GetActiveScene())
        scene = SceneManager.GetActiveScene();
    }

    public void LoadLevel(string nameLevel)
    {
        if (!once)
        {
            once = true;
            fader.ResetTrigger("EndFade");
            fader.SetTrigger("StartFade");
            StartCoroutine(LevelTransition(waitForLoadTimer, nameLevel));
        }
    }

    public void ApplicationQuit()
    {
        fader.ResetTrigger("EndFade");
        fader.SetTrigger("StartFade");
        StartCoroutine(Quit(waitForLoadTimer));   
    }

    IEnumerator LevelTransition(float time, string sceneName)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(sceneName);
        fader.ResetTrigger("StartFade");
        fader.SetTrigger("EndFade");
        once = false;
        Time.timeScale = 1f;                                               // Directly set Time Scale to Unpause value
    }

    IEnumerator Quit(float time)
    {
        yield return new WaitForSeconds(time);
        Application.Quit();
    }
}