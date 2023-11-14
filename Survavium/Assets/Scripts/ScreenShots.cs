using UnityEngine;
public class ScreenShots : MonoBehaviour
{
    [SerializeField] private GameObject HUD;
    private int i;
    private bool HUDenabled = true;

    private void Start()
    {
        HUD = GameObject.Find("----GUI----");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            i += 1;
            ScreenCapture.CaptureScreenshot($"Screen{i}.png");
            Debug.Log("The screenshot was taken");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            HUDenabled = !HUDenabled;  

            if (HUDenabled)
            {
                HUD.SetActive(true);
            }
            else
            { 
                HUD.SetActive(false); 
            }
        }
    }
}
