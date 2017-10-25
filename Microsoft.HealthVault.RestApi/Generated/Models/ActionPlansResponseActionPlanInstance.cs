// Code generated by Microsoft (R) AutoRest Code Generator 1.0.1.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Microsoft.HealthVault.RestApi.Generated.Models
{
    using Microsoft.HealthVault;
    using Microsoft.HealthVault.RestApi;
    using Microsoft.HealthVault.RestApi.Generated;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The action plans collection response.
    /// </summary>
    public partial class ActionPlansResponseActionPlanInstance
    {
        /// <summary>
        /// Initializes a new instance of the
        /// ActionPlansResponseActionPlanInstance class.
        /// </summary>
        public ActionPlansResponseActionPlanInstance()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// ActionPlansResponseActionPlanInstance class.
        /// </summary>
        /// <param name="plans">The collection of action plans</param>
        public ActionPlansResponseActionPlanInstance(IList<ActionPlanInstance> plans = default(IList<ActionPlanInstance>))
        {
            Plans = plans;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the collection of action plans
        /// </summary>
        [JsonProperty(PropertyName = "plans")]
        public IList<ActionPlanInstance> Plans { get; set; }

    }
}
