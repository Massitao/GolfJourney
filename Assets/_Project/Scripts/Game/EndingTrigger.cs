using UnityEngine;

public class EndingTrigger : MonoBehaviour {

    public static EndingTrigger instance;

    private void Awake()
    {
        instance = this;        
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<Ball>() != null)
        {
            GameManager.instance.reachedEnding = true;
        }
    }
}
