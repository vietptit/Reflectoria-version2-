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
    #region Key
    public const string constellation2 = "constellation2";
    public const string constellation3 = "constellation3";
    public const string constellation4 = "constellation4";
    public const string constellation5 = "constellation5";
    public const string constellation6 = "constellation6";
    public const string constellation7 = "constellation7";
    public const string constellation8 = "constellation8";
    public const string constellation9 = "constellation9";
    public const string constellation10 = "constellation10";
    public const string constellation11 = "constellation11";
    public const string constellation12 = "constellation12";
    public const string constellation13 = "constellation13";
    #endregion
    StoreController storeController;
    
    async void Awake()
    {
        
        instance=this;

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
    

    List<ProductDefinition> BuildProductDefinition()
    {
        var intialProduct= new List<ProductDefinition>();
        intialProduct.Add(new ProductDefinition(constellation2,ProductType.NonConsumable));
        intialProduct.Add(new ProductDefinition(constellation3,ProductType.NonConsumable));
        intialProduct.Add(new ProductDefinition(constellation4,ProductType.NonConsumable));
        intialProduct.Add(new ProductDefinition(constellation5,ProductType.NonConsumable));
        intialProduct.Add(new ProductDefinition(constellation6,ProductType.NonConsumable));
        intialProduct.Add(new ProductDefinition(constellation7,ProductType.NonConsumable));
        intialProduct.Add(new ProductDefinition(constellation8,ProductType.NonConsumable));
        intialProduct.Add(new ProductDefinition(constellation9,ProductType.NonConsumable));
        intialProduct.Add(new ProductDefinition(constellation10,ProductType.NonConsumable));
        intialProduct.Add(new ProductDefinition(constellation11,ProductType.NonConsumable));
        intialProduct.Add(new ProductDefinition(constellation12,ProductType.NonConsumable));
        intialProduct.Add(new ProductDefinition(constellation13,ProductType.NonConsumable));

        return intialProduct;
    }

    public string GetPriceById(string id)
    {
        var product= storeController.GetProductById(id);
        if (product != null && product.metadata != null)
        {
            return product.metadata.localizedPriceString;
        }
        return "Loading ...";
    }

    public void BuyProductById(string id)
    {
        storeController.PurchaseProduct(id);
    }
    private void UnlockProduct(string productId)
    {
        switch (productId)
        {
            case constellation2: Controller_level_gr.instance.UnlockedLevel(IAPProductKey.constellation2); break;
            case constellation3: Controller_level_gr.instance.UnlockedLevel(IAPProductKey.constellation3); break;
            case constellation4: Controller_level_gr.instance.UnlockedLevel(IAPProductKey.constellation4); break;
            case constellation5: Controller_level_gr.instance.UnlockedLevel(IAPProductKey.constellation5); break;
            case constellation6: Controller_level_gr.instance.UnlockedLevel(IAPProductKey.constellation6); break;
            case constellation7: Controller_level_gr.instance.UnlockedLevel(IAPProductKey.constellation7); break;
            case constellation8: Controller_level_gr.instance.UnlockedLevel(IAPProductKey.constellation8); break;
            case constellation9: Controller_level_gr.instance.UnlockedLevel(IAPProductKey.constellation9); break;
            case constellation10: Controller_level_gr.instance.UnlockedLevel(IAPProductKey.constellation10); break;
            case constellation11: Controller_level_gr.instance.UnlockedLevel(IAPProductKey.constellation11); break;
            case constellation12: Controller_level_gr.instance.UnlockedLevel(IAPProductKey.constellation12); break;
            case constellation13: Controller_level_gr.instance.UnlockedLevel(IAPProductKey.constellation13); break;
        }
    }
}
