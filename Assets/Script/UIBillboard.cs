using UnityEngine;
using UnityEngine.UI;

public enum IAPProductKey
{
    constellation2,
    constellation3,
    constellation4,
    constellation5,
    constellation6,
    constellation7,
    constellation8,
    constellation9,
    constellation10,
    constellation11,
    constellation12,
    constellation13
}
public class UIBillboard : MonoBehaviour
{
    private Camera mainCamera;
    IAPProductKey iAPProductKey;
    string id="";

    BuyIAPController buyIAPController;

    void Start()
    {
        buyIAPController = BuyIAPController.instance;


        mainCamera = Camera.main;
    }

    private void CheckID()
    {
        switch (iAPProductKey)
        {
            case IAPProductKey.constellation2: id = IAPManager.constellation2; break;
            case IAPProductKey.constellation3: id = IAPManager.constellation3; break;
            case IAPProductKey.constellation4: id = IAPManager.constellation4; break;
            case IAPProductKey.constellation5: id = IAPManager.constellation5; break;
            case IAPProductKey.constellation6: id = IAPManager.constellation6; break;
            case IAPProductKey.constellation7: id = IAPManager.constellation7; break;
            case IAPProductKey.constellation8: id = IAPManager.constellation8; break;
            case IAPProductKey.constellation9: id = IAPManager.constellation9; break;
            case IAPProductKey.constellation10: id = IAPManager.constellation10; break;
            case IAPProductKey.constellation11: id = IAPManager.constellation11; break;
            case IAPProductKey.constellation12: id = IAPManager.constellation12; break;
            case IAPProductKey.constellation13: id = IAPManager.constellation13; break;
        }
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
        
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, 
                             mainCamera.transform.rotation * Vector3.up);
        }
    }

    public void YouAreLocked()
    {
        Debug.Log("you need to watch promoted to unlock");
    }
    
    public void SetUp(IAPProductKey iAPProductKey)
    {
        this.iAPProductKey=iAPProductKey;
        CheckID();
    }
    public void IAPButtonPrice()
    {
        if (IAPManager.instance == null)
        {
            Debug.Log("IAP null");
            return;
        }
        buyIAPController.UpdatePriceProduct(IAPManager.instance.GetPriceById(id),id);
    }



}