using UnityEngine;
using UnityEngine.UI;

public class CreditsScroller : MonoBehaviour
{
    public Transform creditsText;
    private int counter;

	void Update ()
    {
        counter++;
        if(counter % 5 == 0)
        {
            creditsText.position = new Vector3(creditsText.position.x, creditsText.position.y + 1);
        }
	}
}
