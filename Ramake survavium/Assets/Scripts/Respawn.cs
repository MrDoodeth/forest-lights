using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using YG;

public class Respawn : MonoBehaviour
{
    [SerializeField] private Image adImage;
    [SerializeField] private GameObject quitPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject closeButton;
    private bool isDestroed = false;
    private bool isPressed = false;

    private void OnEnable()
    {
        YandexGame.RewardVideoEvent += Reward;
        adImage.fillAmount = 1;
        closeButton.GetComponent<Animation>().Play();
        adImage.GetComponent<Button>().enabled = true;
        isPressed = false;
    }

    private void OnDisable()
    {
        YandexGame.RewardVideoEvent -= Reward;
    }
    public void RespawnReward()
    {
        isPressed = true;
        adImage.fillAmount = 1;
        YandexGame.RewVideoShow(1);
    }

    public void Reward(int id)
    {
        EventBus.OnPlayerRespawn?.Invoke();
        losePanel.SetActive(false);
    }

    private void Update()
    {
        if (!isPressed)
            adImage.fillAmount -= Time.deltaTime / 5;

        if (adImage.fillAmount < 0.12f)
            adImage.GetComponent<Button>().enabled = false;

        if (adImage.fillAmount == 0 && !isDestroed)
        {
            isDestroed = true;
            quitPanel.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
}
