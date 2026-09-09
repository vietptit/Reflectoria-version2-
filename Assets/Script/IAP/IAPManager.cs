using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Purchasing;
using Unity.Services.Core.Environments;

public class IAPManager : MonoBehaviour
{
    public static IAPManager instance;
    public static bool isInitialized{get;private set;}=false;
    StoreController storeController;
    
    async void Awake()
    {
        if(instance==null)   
            instance=this;
        else
        {
            Destroy(gameObject);
            return;
        }

        await InitIAP();
    }

    private async Task InitIAP()
    {
        try
        {
            var option= new InitializationOptions().SetEnvironmentName("production");
            await UnityServices.InitializeAsync(option);

            storeController=UnityIAPServices.StoreController();
            
            storeController.OnStoreDisconnected+=OnStoreDisconnected;
            storeController.OnProductsFetched+=OnProductsFetched;
            storeController.OnProductsFetchFailed+=OnProductsFetchFailed;
            storeController.OnPurchasesFetched+=OnPurchasesFetched;
            storeController.OnPurchaseFailed+=OnPurchaseFailed;
            storeController.OnPurchaseConfirmed+=OnPurchaseConfirmed;
            storeController.OnPurchasePending+=OnPurchasePending;
            storeController.OnPurchaseDeferred+=OnPurchaseDeferred;

            await storeController.Connect();
            var tmp= BuildProductDefinition();
            storeController.FetchProducts(tmp);

        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
    }

    private void OnPurchaseDeferred(DeferredOrder order)
    {
        throw new NotImplementedException();
    }

    private void OnPurchasePending(PendingOrder order)
    {
        Debug.Log("Đang xử lý mua hàng");
        storeController.ConfirmPurchase(order);
    }

    private void OnPurchaseConfirmed(Order order)
    {
        Debug.Log("Mua hàng thành công");
        if (order?.Info?.PurchasedProductInfo != null && order.Info.PurchasedProductInfo.Count > 0)
        {
            string productId=order.Info.PurchasedProductInfo[0].productId;
            Debug.Log("Buy"+ " "+productId);
            UnlockProduct(productId);
        }
    }

    private void OnPurchaseFailed(FailedOrder order)
    {
        Debug.Log("Lấy lịch sử mua hàng thất bại");
    }
    

    private void OnPurchasesFetched(Orders orders)
    {
        Debug.Log("Lấy lịch sử mua hàng thành công. Đang khôi phục dữ liệu...");

        if (orders != null)
        {
         
            foreach (Order order in orders.ConfirmedOrders) 
            {
                if (order?.Info?.PurchasedProductInfo != null && order.Info.PurchasedProductInfo.Count > 0)
                {
                    string productId = order.Info.PurchasedProductInfo[0].productId;
                    UnlockProduct(productId);
                    
                    Debug.Log("Đã khôi phục thành công chòm sao: " + productId);
                }
            }
        }

        isInitialized = true;
    }

    private void OnProductsFetchFailed(ProductFetchFailed failed)
    {
        Debug.Log("Lấy mặt hàng thất bại");
    }
    

    private void OnProductsFetched(List<Product> list)
    {
       Debug.Log("Lấy dữ liệu giá sản phẩm thành công -> bắt đầu truy suất lịch sử");
        storeController.FetchPurchases();
    }
    

    private void OnStoreDisconnected(StoreConnectionFailureDescription description)
    {
        Debug.Log("mất kết nối tới cửa hàng");
    }
    

    private List<ProductDefinition> BuildProductDefinition()
    {
        var initialProducts = new List<ProductDefinition>();
        string[] productKeys = Enum.GetNames(typeof(IAPProductKey));
        
        foreach (string key in productKeys)
        {
            initialProducts.Add(new ProductDefinition(key, ProductType.NonConsumable));
        }

        return initialProducts;
    }

    public string GetPriceById(IAPProductKey id)
    {
        
        var product= storeController.GetProductById(id.ToString());
        if (product != null && product.metadata != null)
        {
            return product.metadata.localizedPriceString;
        }
        return "Loading ...";
    }

    public void BuyProductById(IAPProductKey id)
    {
        storeController.PurchaseProduct(id.ToString());
    }
    private void UnlockProduct(string productId)
    {
        if (Enum.TryParse(productId, out IAPProductKey parsedKey))
        {
            Controller_level_gr.instance.UnlockedLevel(parsedKey);
        }
        else
        {
            Debug.LogError($"[IAP Cảnh báo] Sản phẩm {productId} chưa được định nghĩa trong Enum!");
        }
    }
}
