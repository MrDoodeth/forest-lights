using UnityEngine;
using UnityEngine.UI;
using YandexMobileAds.Base;

public class Respawn : MonoBehaviour
{
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject closeButton;

    private void OnEnable()
    {
        closeButton.GetComponent<Animation>().Play();
        RewardedAdController.OnGotRewarded += Reward;
    }

    private void OnDisable()
    {
        RewardedAdController.OnGotRewarded -= Reward;
    }
    private void Reward(Reward reward)
    {
        EventBus.OnPlayerRespawn?.Invoke();
        Debug.Log(reward.type);
        losePanel.SetActive(false);
    }
}
