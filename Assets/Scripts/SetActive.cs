using UnityEngine;

public class SetActive : MonoBehaviour
{
    
    void Start()
    {
        Invoke(nameof(SetActiceFor),.01f);
    }

    void SetActiceFor()
    {
        gameObject.SetActive(false);
    }


}
