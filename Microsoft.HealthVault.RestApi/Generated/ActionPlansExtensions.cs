// Code generated by Microsoft (R) AutoRest Code Generator 1.0.1.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Microsoft.HealthVault.RestApi.Generated
{
    using Microsoft.HealthVault;
    using Microsoft.HealthVault.RestApi;
    using Models;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for ActionPlans.
    /// </summary>
    public static partial class ActionPlansExtensions
    {
            /// <summary>
            /// Get a collection of action plans
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='maxPageSize'>
            /// The maximum number of entries to return per page. Defaults to 1000.
            /// </param>
            public static ActionPlansResponseActionPlanInstance Get(this IActionPlans operations, int? maxPageSize = default(int?))
            {
                return operations.GetAsync(maxPageSize).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Get a collection of action plans
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='maxPageSize'>
            /// The maximum number of entries to return per page. Defaults to 1000.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<ActionPlansResponseActionPlanInstance> GetAsync(this IActionPlans operations, int? maxPageSize = default(int?), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetWithHttpMessagesAsync(maxPageSize, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Update/Replace a complete action plan instance with no merge.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='actionPlan'>
            /// The instance of the plan to update. The entire plan will be replaced with
            /// this version.
            /// </param>
            public static ActionPlansResponseActionPlanInstance Replace(this IActionPlans operations, ActionPlanInstance actionPlan)
            {
                return operations.ReplaceAsync(actionPlan).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Update/Replace a complete action plan instance with no merge.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='actionPlan'>
            /// The instance of the plan to update. The entire plan will be replaced with
            /// this version.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<ActionPlansResponseActionPlanInstance> ReplaceAsync(this IActionPlans operations, ActionPlanInstance actionPlan, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ReplaceWithHttpMessagesAsync(actionPlan, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Post an action plan instance
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='actionPlan'>
            /// The instance of the plan to create.
            /// </param>
            public static object Create(this IActionPlans operations, ActionPlan actionPlan)
            {
                return operations.CreateAsync(actionPlan).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Post an action plan instance
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='actionPlan'>
            /// The instance of the plan to create.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> CreateAsync(this IActionPlans operations, ActionPlan actionPlan, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.CreateWithHttpMessagesAsync(actionPlan, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Update an action plan instance with merge
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='actionPlan'>
            /// The instance of the plan to update. Only the fields present in the passed
            /// in model will be updated. All other fields and colelctions
            /// will be left, as is, unless invalid.
            /// </param>
            public static ActionPlansResponseActionPlanInstance Update(this IActionPlans operations, ActionPlanInstance actionPlan)
            {
                return operations.UpdateAsync(actionPlan).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Update an action plan instance with merge
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='actionPlan'>
            /// The instance of the plan to update. Only the fields present in the passed
            /// in model will be updated. All other fields and colelctions
            /// will be left, as is, unless invalid.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<ActionPlansResponseActionPlanInstance> UpdateAsync(this IActionPlans operations, ActionPlanInstance actionPlan, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.UpdateWithHttpMessagesAsync(actionPlan, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Get an instance of a specific action plan
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='actionPlanId'>
            /// The action plan to update.
            /// </param>
            public static ActionPlanInstance GetById(this IActionPlans operations, string actionPlanId)
            {
                return operations.GetByIdAsync(actionPlanId).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Get an instance of a specific action plan
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='actionPlanId'>
            /// The action plan to update.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<ActionPlanInstance> GetByIdAsync(this IActionPlans operations, string actionPlanId, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetByIdWithHttpMessagesAsync(actionPlanId, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Delete an action plan instance
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='actionPlanId'>
            /// The instance of the plan to delete.
            /// </param>
            public static object Delete(this IActionPlans operations, string actionPlanId)
            {
                return operations.DeleteAsync(actionPlanId).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Delete an action plan instance
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='actionPlanId'>
            /// The instance of the plan to delete.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> DeleteAsync(this IActionPlans operations, string actionPlanId, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.DeleteWithHttpMessagesAsync(actionPlanId, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Gets adherence information for an action plan.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='startTime'>
            /// The start time.
            /// </param>
            /// <param name='endTime'>
            /// The end time.
            /// </param>
            /// <param name='actionPlanId'>
            /// The action plan identifier.
            /// </param>
            /// <param name='objectiveId'>
            /// The objective to filter the report to.
            /// </param>
            /// <param name='taskId'>
            /// The task to filter the report to.
            /// </param>
            public static ActionPlanAdherenceSummary GetAdherence(this IActionPlans operations, System.DateTime startTime, System.DateTime endTime, string actionPlanId, string objectiveId = default(string), string taskId = default(string))
            {
                return operations.GetAdherenceAsync(startTime, endTime, actionPlanId, objectiveId, taskId).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Gets adherence information for an action plan.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='startTime'>
            /// The start time.
            /// </param>
            /// <param name='endTime'>
            /// The end time.
            /// </param>
            /// <param name='actionPlanId'>
            /// The action plan identifier.
            /// </param>
            /// <param name='objectiveId'>
            /// The objective to filter the report to.
            /// </param>
            /// <param name='taskId'>
            /// The task to filter the report to.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<ActionPlanAdherenceSummary> GetAdherenceAsync(this IActionPlans operations, System.DateTime startTime, System.DateTime endTime, string actionPlanId, string objectiveId = default(string), string taskId = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetAdherenceWithHttpMessagesAsync(startTime, endTime, actionPlanId, objectiveId, taskId, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

    }
}
