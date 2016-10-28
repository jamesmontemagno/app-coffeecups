using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ilovecoffeeService.DataObjects
{
    public class CupOfCoffee : EntityData
    {
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
        public DateTime DateUtc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CoffeeCups.CupOfCoffee"/> made at home.
        /// </summary>
        /// <value><c>true</c> if made at home; otherwise, <c>false</c>.</value>
        public bool MadeAtHome { get; set; }

        /// <summary>
        /// Gets or sets the OS of the user
        /// </summary>
        /// <value>The OS</value>
        public string OS { get; set; }
    }
}