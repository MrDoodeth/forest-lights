using System.Collections;
using UnityEngine;

public class Berry_bush : MonoBehaviour
{
    public void Selection()
    {
        StartCoroutine(Death());
        IEnumerator Death()
        {
            yield return new WaitForSeconds(1f);
            gameObject.SetActive(false);
        }
    }
}
