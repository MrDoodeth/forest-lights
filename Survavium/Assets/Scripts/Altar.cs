using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : MonoBehaviour
{
    [SerializeField] private AudioSource audioLightUp;
    [SerializeField] private AudioSource audioFire;
    [SerializeField] private GameObject Kinding;
    [SerializeField] private GameObject fxFire;
    [SerializeField] private Transform[] KindingCells;
    public void Fire()
    {
        StartCoroutine(LightUp());

        IEnumerator LightUp()
        {
            GameObject[] KindingArray = new GameObject[3];
            for (int i = 0; i < KindingCells.Length; i++)
            {
                KindingArray[i] = Instantiate(Kinding, KindingCells[i].position, Quaternion.Euler(new Vector3(0f, Random.Range(0, 180), 0f)));
            }
            gameObject.layer = 0;
            yield return new WaitForSeconds(2);
            audioLightUp.Play();
            fxFire.SetActive(true);           
            fxFire.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
            EventBus.OnExitActivated?.Invoke();
            yield return new WaitForSeconds(10);
            for (int i = 0; i < KindingArray.Length; i++)
            {
                Destroy(KindingArray[i].gameObject);
                Destroy(KindingCells[i].gameObject);
            }
        }
    }
}
