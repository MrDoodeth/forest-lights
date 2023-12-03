using UnityEngine;
using UnityEngine.UI;

public class Respawn : MonoBehaviour
{
    [SerializeField] private Image addImage;
    [SerializeField] private GameObject quitPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject closeButton;
    private bool isDestroed = false;

    private void OnEnable()
    {
        addImage.fillAmount = 1; 
        closeButton.GetComponent<Animation>().Play();
    }
    public void RespawnReward()
    {
        //Show ad
        EventBus.OnPlayerRespawn?.Invoke();
        losePanel.SetActive(false);
    }

    private void Update()
    {
        addImage.fillAmount -= Time.deltaTime / 5;

        if (addImage.fillAmount == 0 && !isDestroed)
        {
            isDestroed = true;
            quitPanel.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
}
