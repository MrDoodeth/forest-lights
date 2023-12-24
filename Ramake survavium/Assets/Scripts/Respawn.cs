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
        closeButton.GetComponent<Animation>().Play();
        YandexGame.CloseVideoEvent += Reward;
        YandexGame.RewardVideoEvent += Reward;
    }

    private void OnDisable()
    {
        YandexGame.CloseVideoEvent -= Reward;
    }
    public void RespawnReward()
    {
        YandexGame.RewVideoShow(1);
    }

    public void Reward()
    {
        EventBus.OnPlayerRespawn?.Invoke();
        losePanel.SetActive(false);
    }

    public void Reward(int i)
    {
        EventBus.OnPlayerRespawn?.Invoke();
        losePanel.SetActive(false);
    }
}
