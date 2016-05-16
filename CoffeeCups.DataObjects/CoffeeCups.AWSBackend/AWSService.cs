using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using CoffeeCups.DataObjects;
using CoffeeCups.Utils;
using System.Linq;
using Plugin.Connectivity;
using Amazon.DynamoDBv2.DocumentModel;

namespace CoffeeCups.AWSBackend
{
    public class AWSService : IDataService
    {
        protected AmazonDynamoDBClient dbClient;
        public DynamoDBContext DynamoContext { get; protected set; }

        bool initialized;

        public Task InitializeAsync()
        {
            if (initialized)
                return Task.FromResult(true);
            
            //Step 1: Ensure Clock Skew is set
            AWSConfigs.CorrectForClockSkew = true;


            //Step 2: Initialize the Amazon Cognito credentials provider
            var credentialsProvider = new CognitoAWSCredentials(
                AWSConstants.IdentityPoolId, // Identity Pool ID
                AWSConstants.Region // Region
            );

            //Step 3: Create DynamoDB Client with Credentials
            dbClient = new AmazonDynamoDBClient(credentialsProvider, RegionEndpoint.USEast1);

            //Step 4: Create DynmaoDB Contect with the client
            DynamoContext = new DynamoDBContext(dbClient);

            initialized = true;

            return Task.FromResult(true);
        }


        public async Task<IEnumerable<CupOfCoffee>> GetAllCoffeeAsync()
        {
            await InitializeAsync();

            //Step 1: Create List of ScanCondition
            var conditions = new List<ScanCondition>();

            //Step 2: ScanAsync for Item from teh dbContext
            var scan = DynamoContext.ScanAsync<AWSCupofCoffee>(conditions);

            //Step 3: GetRemainingAsync viea the scan
            var items = await scan.GetRemainingAsync();

            return items.Select(c => new CupOfCoffee
            {
                DateUtc = c.DateUtc,
                Id = c.Id,
                MadeAtHome = c.MadeAtHome,
                UserId = c.UserId,
                OS = c.OS
            });
        }

        public async Task<CupOfCoffee> AddCoffeeAsync(bool atHome, string os)
        {
            await InitializeAsync();

            //Step 1: Create new OperationConfig
            var config = new DynamoDBOperationConfig
            {
                IgnoreNullValues = false
            };

            //create and insert coffee
            var coffee = new AWSCupofCoffee
            {
                DateUtc = DateTime.UtcNow,
                MadeAtHome = atHome,
                OS = os,
                Id = Guid.NewGuid().ToString(),
                UserId = string.Empty
            };

            //Step 2: Save Feedback to dbContext
            await DynamoContext.SaveAsync<AWSCupofCoffee>(coffee, config);

            return new CupOfCoffee
            {
                DateUtc = coffee.DateUtc,
                MadeAtHome = coffee.MadeAtHome,
                OS = coffee.OS,
                Id = coffee.Id
            };
        }


        public Task SyncCoffeeAsync()
        {
            return Task.FromResult(true);
        }

        public bool NeedsAuthentication => false;


        public AuthProvider AuthProvider => AuthProvider.Facebook;



    }
}

