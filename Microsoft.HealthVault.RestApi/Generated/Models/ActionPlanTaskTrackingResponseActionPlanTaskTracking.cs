// Code generated by Microsoft (R) AutoRest Code Generator 1.2.2.0
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
    /// The action plans task tracking collection response
    /// </summary>
    public partial class ActionPlanTaskTrackingResponseActionPlanTaskTracking
    {
        /// <summary>
        /// Initializes a new instance of the
        /// ActionPlanTaskTrackingResponseActionPlanTaskTracking class.
        /// </summary>
        public ActionPlanTaskTrackingResponseActionPlanTaskTracking()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// ActionPlanTaskTrackingResponseActionPlanTaskTracking class.
        /// </summary>
        /// <param name="taskTrackingInstances">The collection of tasks
        /// tracking occurrences</param>
        public ActionPlanTaskTrackingResponseActionPlanTaskTracking(IList<ActionPlanTaskTracking> taskTrackingInstances = default(IList<ActionPlanTaskTracking>))
        {
            TaskTrackingInstances = taskTrackingInstances;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the collection of tasks tracking occurrences
        /// </summary>
        [JsonProperty(PropertyName = "taskTrackingInstances")]
        public IList<ActionPlanTaskTracking> TaskTrackingInstances { get; set; }

    }
}
