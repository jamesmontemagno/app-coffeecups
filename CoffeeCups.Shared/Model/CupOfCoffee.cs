using System;

namespace CoffeeCups
{
    public class CupOfCoffee
    {
        

        [Newtonsoft.Json.JsonProperty("Id")]
        public string Id { get; set; }

        [Microsoft.WindowsAzure.MobileServices.Version]
        public string AzureVersion { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>The user identifier.</value>
        [Newtonsoft.Json.JsonProperty("userId")]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the date UTC.
        /// </summary>
        /// <value>The date UTC.</value>
        public DateTime DateUtc { get; set;}

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CoffeeCups.CupOfCoffee"/> made at home.
        /// </summary>
        /// <value><c>true</c> if made at home; otherwise, <c>false</c>.</value>
        public bool MadeAtHome{ get; set; }

        /// <summary>
        /// Gets or sets the OS of the user
        /// </summary>
        /// <value>The OS</value>
        public string OS { get; set; }


        [Newtonsoft.Json.JsonIgnore]
        public string DateDisplay { get { return DateUtc.ToLocalTime().ToString("d"); } }

        [Newtonsoft.Json.JsonIgnore]
        public string TimeDisplay { get { return DateUtc.ToLocalTime().ToString("t") + " " + OS.ToString(); } }

        [Newtonsoft.Json.JsonIgnore]
        public string AtHomeDisplay { get { return MadeAtHome ? "Made At Home" : string.Empty; } }
    }
}

