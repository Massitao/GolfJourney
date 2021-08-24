using UnityEngine;

public class UIMenuReferencer : MonoBehaviour {
	
    // Calls UI to manage Opening and Closing the Stats.
	public void OpenClose()
    {
        UIManager.instance.StatsAnimHandling();
    }

    public void EndOpenClose()
    {
        UIManager.instance.EndStatMenuOpenClose();
    }
}
