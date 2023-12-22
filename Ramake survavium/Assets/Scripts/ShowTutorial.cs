using Platforms;
using UnityEngine;

public class ShowTutorial : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject Cross;

    private void Start()
    {
        if (PlayerPrefs.HasKey("ShowTutorial"))
        {
            tutorialPanel.SetActive(true);
            Cross.SetActive(true);
        }
        else
        {
            tutorialPanel.SetActive(false);
            Cross.SetActive(false);
        }

        if ((Platform)PlayerPrefs.GetInt("Platform") == Platform.PC)
        {
            tutorialPanel.SetActive(true);
            Cross.SetActive(true);
        }
        else
        {
            tutorialPanel.SetActive(false);
            Cross.SetActive(false);
        }
    }
}
