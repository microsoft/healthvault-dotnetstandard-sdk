// Code generated by Microsoft (R) AutoRest Code Generator 1.2.2.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Microsoft.HealthVault.RestApi.Generated.Models
{
    using Microsoft.HealthVault;
    using Microsoft.HealthVault.RestApi;
    using Microsoft.HealthVault.RestApi.Generated;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// A tracking object for an Action Plan Task
    /// </summary>
    public partial class ActionPlanTaskTracking
    {
        /// <summary>
        /// Initializes a new instance of the ActionPlanTaskTracking class.
        /// </summary>
        public ActionPlanTaskTracking()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ActionPlanTaskTracking class.
        /// </summary>
        /// <param name="id">Gets or sets the Id of the task tracking</param>
        /// <param name="trackingType">Gets or sets the task tracking type.
        /// Possible values include: 'Unknown', 'Manual', 'Auto'</param>
        /// <param name="timeZoneOffset">Gets or sets the timezone offset of
        /// the task tracking,
        /// if a task is local time based, it should be set as null</param>
        /// <param name="trackingDateTime">Gets or sets the task tracking
        /// time</param>
        /// <param name="creationDateTime">Gets or sets the creation time of
        /// the task tracking</param>
        /// <param name="trackingStatus">Gets or sets the task tracking status.
        /// Possible values include: 'Unknown', 'Occurrence', 'Completion',
        /// 'OutOfWindow'</param>
        /// <param name="occurrenceStart">Gets or sets the start time of the
        /// occurrence window,
        /// it is null for Completion and OutOfWindow tracking</param>
        /// <param name="occurrenceEnd">Gets or sets the end time of the
        /// occurrence window,
        /// it is null for Completion and OutOfWindow tracking</param>
        /// <param name="completionStart">Gets or sets the start time of the
        /// completion window</param>
        /// <param name="completionEnd">Gets or sets the end time of the
        /// completion window</param>
        /// <param name="taskId">Gets or sets task Id</param>
        /// <param name="feedback">Gets or sets the tracking feedback</param>
        /// <param name="evidence">Gets or sets the evidence of the task
        /// tracking</param>
        public ActionPlanTaskTracking(string id = default(string), string trackingType = default(string), int? timeZoneOffset = default(int?), System.DateTime? trackingDateTime = default(System.DateTime?), System.DateTime? creationDateTime = default(System.DateTime?), string trackingStatus = default(string), System.DateTime? occurrenceStart = default(System.DateTime?), System.DateTime? occurrenceEnd = default(System.DateTime?), System.DateTime? completionStart = default(System.DateTime?), System.DateTime? completionEnd = default(System.DateTime?), System.Guid? taskId = default(System.Guid?), string feedback = default(string), ActionPlanTaskTrackingEvidence evidence = default(ActionPlanTaskTrackingEvidence))
        {
            Id = id;
            TrackingType = trackingType;
            TimeZoneOffset = timeZoneOffset;
            TrackingDateTime = trackingDateTime;
            CreationDateTime = creationDateTime;
            TrackingStatus = trackingStatus;
            OccurrenceStart = occurrenceStart;
            OccurrenceEnd = occurrenceEnd;
            CompletionStart = completionStart;
            CompletionEnd = completionEnd;
            TaskId = taskId;
            Feedback = feedback;
            Evidence = evidence;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the Id of the task tracking
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the task tracking type. Possible values include:
        /// 'Unknown', 'Manual', 'Auto'
        /// </summary>
        [JsonProperty(PropertyName = "trackingType")]
        public string TrackingType { get; set; }

        /// <summary>
        /// Gets or sets the timezone offset of the task tracking,
        /// if a task is local time based, it should be set as null
        /// </summary>
        [JsonProperty(PropertyName = "timeZoneOffset")]
        public int? TimeZoneOffset { get; set; }

        /// <summary>
        /// Gets or sets the task tracking time
        /// </summary>
        [JsonProperty(PropertyName = "trackingDateTime")]
        public System.DateTime? TrackingDateTime { get; set; }

        /// <summary>
        /// Gets or sets the creation time of the task tracking
        /// </summary>
        [JsonProperty(PropertyName = "creationDateTime")]
        public System.DateTime? CreationDateTime { get; set; }

        /// <summary>
        /// Gets or sets the task tracking status. Possible values include:
        /// 'Unknown', 'Occurrence', 'Completion', 'OutOfWindow'
        /// </summary>
        [JsonProperty(PropertyName = "trackingStatus")]
        public string TrackingStatus { get; set; }

        /// <summary>
        /// Gets or sets the start time of the occurrence window,
        /// it is null for Completion and OutOfWindow tracking
        /// </summary>
        [JsonProperty(PropertyName = "occurrenceStart")]
        public System.DateTime? OccurrenceStart { get; set; }

        /// <summary>
        /// Gets or sets the end time of the occurrence window,
        /// it is null for Completion and OutOfWindow tracking
        /// </summary>
        [JsonProperty(PropertyName = "occurrenceEnd")]
        public System.DateTime? OccurrenceEnd { get; set; }

        /// <summary>
        /// Gets or sets the start time of the completion window
        /// </summary>
        [JsonProperty(PropertyName = "completionStart")]
        public System.DateTime? CompletionStart { get; set; }

        /// <summary>
        /// Gets or sets the end time of the completion window
        /// </summary>
        [JsonProperty(PropertyName = "completionEnd")]
        public System.DateTime? CompletionEnd { get; set; }

        /// <summary>
        /// Gets or sets task Id
        /// </summary>
        [JsonProperty(PropertyName = "taskId")]
        public System.Guid? TaskId { get; set; }

        /// <summary>
        /// Gets or sets the tracking feedback
        /// </summary>
        [JsonProperty(PropertyName = "feedback")]
        public string Feedback { get; set; }

        /// <summary>
        /// Gets or sets the evidence of the task tracking
        /// </summary>
        [JsonProperty(PropertyName = "evidence")]
        public ActionPlanTaskTrackingEvidence Evidence { get; set; }

    }
}
