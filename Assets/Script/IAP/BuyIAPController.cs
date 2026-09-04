using System.Security;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


 
public class BuyIAPController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI price_text;
    public static BuyIAPController instance;
    string id="";
    void Awake()
    {
        if(instance==null)
            instance=this;
        else
        {
            Destroy(gameObject);
            return;
        }

    }

    public void UpdatePriceProduct(string price,string id)
    {
        price_text.text=price;
        this.id=id;
    }

    public void BuyProduct()
    {
        bool check=IAPManager.isInitialized;
        if (!check)
        {
            Debug.Log("xin lỗi chưa cập giá thành công");
        }
        else
        {
            IAPManager.instance.BuyProductById(id);
        }
    }
}
