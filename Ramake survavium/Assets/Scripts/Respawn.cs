using UnityEngine;
using UnityEngine.UI;
using YG;

public class Respawn : MonoBehaviour
{
    [SerializeField] private Image addImage;
    [SerializeField] private GameObject quitPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject closeButton;
    private bool isDestroed = false;

    private void OnEnable()
    {
        YandexGame.RewardVideoEvent += Reward;
        addImage.fillAmount = 1; 
        closeButton.GetComponent<Animation>().Play();
    }

    private void OnDisable()
    {
        YandexGame.RewardVideoEvent -= Reward;
    }
    public void RespawnReward()
    {
        YandexGame.RewVideoShow(1);
    }

    public void Reward(int id)
    {
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
