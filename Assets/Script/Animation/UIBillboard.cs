using UnityEngine;
using UnityEngine.UI;


public class UIBillboard : MonoBehaviour
{
    private Camera mainCamera;
    IAPProductKey iAPProductKey;
    

    BuyIAPController buyIAPController;

    void Start()
    {
        buyIAPController = BuyIAPController.instance;


        mainCamera = Camera.main;
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
    }
    public void IAPButtonPrice()
    {
        if (IAPManager.instance == null)
        {
            Debug.Log("IAP null");
            return;
        }
        buyIAPController.UpdatePriceProduct(IAPManager.instance.GetPriceById(iAPProductKey),iAPProductKey);
    }



}