using Blazored.SessionStorage;
using System.Text.Json;
using System.Text;
using SapirProductionFloorManagment.Shared.Authentication___Autherization;

namespace SapirProductionFloorManagment.Client.Logic
{
    public static class SessionStorageServiceExtension
    {

        //Saving the token
        public static async Task SaveEncryptedItemAsync<T>(this ISessionStorageService sessionServiceStorage, string key, T item)
        {
            var jsonItem = JsonSerializer.Serialize(item);
            var jsonItemInBytes = Encoding.UTF8.GetBytes(jsonItem);
            var base64Json = Convert.ToBase64String(jsonItemInBytes);       
            await sessionServiceStorage.SetItemAsync(key, base64Json);
        }
        
       
        //Reading token and encypt then 
        public static async Task<UserSession?> ReadEncryptedItemAsync(this ISessionStorageService sessionServiceStorage, string key)
        {
      

            var base64Json = await sessionServiceStorage.GetItemAsync<string>(key);

            if (!String.IsNullOrEmpty(base64Json))
            {              
                    var itemJsonBytes = Convert.FromBase64String(base64Json);
                    var itemJosn = Encoding.UTF8.GetString(itemJsonBytes);
                    var item = JsonSerializer.Deserialize<UserSession>(itemJosn);                   
                    return item;              
            }
            return null;

        }
    }
}
