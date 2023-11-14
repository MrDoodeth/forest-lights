using UnityEngine;

public class ShowTutorial : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject Cross;

    private void Start()
    {
        if (PlayerPrefs.HasKey("ShowTutorial"))
        {
            tutorialPanel.SetActive(System.Convert.ToBoolean(PlayerPrefs.GetInt("ShowTutorial")));
            Cross.SetActive(System.Convert.ToBoolean(PlayerPrefs.GetInt("ShowTutorial")));
        }
        else
        {
            tutorialPanel.SetActive(true);
            Cross.SetActive(true);
        }
    }
}
