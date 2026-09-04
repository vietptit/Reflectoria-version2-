using DG.Tweening;
using UnityEngine;

public class DestroyItem : MonoBehaviour
{
    private bool isCollected = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !isCollected)
        {
            isCollected = true;
            Sequence s = DOTween.Sequence();
            s.Append(transform.DOScale(1.5f, 0.2f).SetEase(Ease.OutQuad));
            s.Append(transform.DOScale(0f, 0.2f).SetEase(Ease.InBack));
            s.OnComplete(() => 
            {
                Destroy(gameObject);
            });
        }
    }
}