using System;
using Amazon.DynamoDBv2.DataModel;

namespace CoffeeCups.AWSBackend
{
    [DynamoDBTable("CupOfCoffee")]
    public class AWSCupofCoffee
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
        [DynamoDBProperty]
        public string UserId { get; set; }

        [DynamoDBProperty]
        public bool MadeAtHome { get; set; }

        [DynamoDBProperty]
        public string OS { get; set; }

        [DynamoDBProperty]
        public DateTime DateUtc { get; set; }
    }
}

