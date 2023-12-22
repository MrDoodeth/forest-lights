using UnityEngine;
using UnityEngine.UI;
using YG;

public class Respawn : MonoBehaviour
{
    [SerializeField] private Image adImage;
    [SerializeField] private GameObject quitPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject closeButton;
    [SerializeField] private GameObject respawnPanel;

    private void OnEnable()
    {
        YandexGame.RewardVideoEvent += Reward;
        closeButton.GetComponent<Animation>().Play();
    }

    private void OnDisable()
    {
        YandexGame.RewardVideoEvent -= Reward;
    }
    public void RespawnReward()
    {
        Time.timeScale = 0;
        YandexGame.RewVideoShow(1);
    }

    public void Reward(int id)
    {
        losePanel.SetActive(false);
        Time.timeScale = 1;
        EventBus.OnPlayerRespawn?.Invoke();
    }
}
