using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoSync;
using Amazon.CognitoSync.SyncManager;
using CoffeeCups.Utils;

namespace CoffeeCups.AWSBackend
{
    public class AWSFavoriteService
    {

        CognitoSyncManager syncManager;
        Dataset dataset;
       
        public AWSFavoriteService()
        {

            var clientConfig = new AmazonCognitoSyncConfig { RegionEndpoint = AWSConstants.Region };

           
            var credentialsProvider = new CognitoAWSCredentials(
                AWSConstants.IdentityPoolId, // Identity Pool ID
                AWSConstants.Region // Region
            );

            syncManager = new CognitoSyncManager(credentialsProvider, clientConfig);

            credentialsProvider.AddLogin("api.twitter.com", Settings.AuthToken);

            dataset = syncManager.OpenOrCreateDataset("favs");
            dataset.OnSyncSuccess += (sender, e) =>
            {
                
            };

            dataset.OnSyncFailure += (sender, e) =>
            {

            };
        }


        public async Task<string> GetAsync(string key)
        {
            
            await dataset.SynchronizeAsync();
            return dataset.Get(key);
        }

        public async Task SaveFavAsync(string key, string favorite)
        {
            dataset.Put(key, favorite);
            await dataset.SynchronizeAsync();
        }
    }
}


