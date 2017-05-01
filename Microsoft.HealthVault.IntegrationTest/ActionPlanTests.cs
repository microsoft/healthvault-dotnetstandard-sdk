using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Record;
using Microsoft.HealthVault.RestApi;
using Microsoft.HealthVault.RestApi.Generated;
using Microsoft.HealthVault.RestApi.Generated.Models;
using Microsoft.Rest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.IntegrationTest
{
    [TestClass]
    public class ActionPlanTests
    {
        private const string PlanName = "Manage your weight";

        [TestMethod]
        public async Task BasicActionPlans()
        {
            IHealthVaultSodaConnection connection = HealthVaultConnectionFactory.Current.GetOrCreateSodaConnection(Constants.Configuration);
            PersonInfo personInfo = await connection.GetPersonInfoAsync();
            HealthRecordInfo record = personInfo.SelectedRecord;

            var restClient = connection.CreateMicrosoftHealthVaultRestApi(record.Id);

            await RemoveAllActionPlansAsync(restClient);

            Guid planId = Guid.NewGuid();
            await restClient.CreateActionPlanWithHttpMessagesAsync(CreateWeightActionPlan(planId));

            HttpOperationResponse<ActionPlansResponseActionPlanInstance> plans = await restClient.GetActionPlansWithHttpMessagesAsync();
            Assert.AreEqual(1, plans.Body.Plans.Count);

            ActionPlanInstance planInstance = plans.Body.Plans[0];
            Assert.AreEqual(planId.ToString(), planInstance.Id);
            Assert.AreEqual(PlanName, planInstance.Name);
        }

        private static async Task RemoveAllActionPlansAsync(IMicrosoftHealthVaultRestApi api)
        {
            HttpOperationResponse<ActionPlansResponseActionPlanInstance> plans = await api.GetActionPlansWithHttpMessagesAsync();
            foreach (var plan in plans.Body.Plans)
            {
                await api.DeleteActionPlanWithHttpMessagesAsync(plan.Id);
            }
        }

        private static ActionPlan CreateWeightActionPlan(Guid planId)
        {
            var plan = new ActionPlan();
            var objective = new Objective
            {
                Id = planId.ToString(),
                Name = PlanName,
                Description = "Manage your weight better by measuring daily. ",
                State = "Active",
                OutcomeName = "Better control over your weight",
                OutcomeType = "Other"
            };

            // Use this if you want to create the task with the plan in one call.
            // You can also create tasks in a separate call after the action plan is created.
            var task = CreateDailyWeightMeasurementActionPlanTask(objective.Id);

            plan.Name = "Track your weight";
            plan.Description = "Daily weight tracking can help you be more conscious of what you eat. ";

            plan.ImageUrl = "https://img-prod-cms-rt-microsoft-com.akamaized.net/cms/api/am/imageFileData/RW680a?ver=b227";
            plan.ThumbnailImageUrl = "https://img-prod-cms-rt-microsoft-com.akamaized.net/cms/api/am/imageFileData/RW6fN6?ver=6479";
            plan.Category = "Health";
            plan.Objectives = new Collection<Objective> { objective };
            plan.AssociatedTasks = new Collection<ActionPlanTask> { task };

            return plan;
        }

        /// <summary>
        /// Creates a sample frequency based task associated with the specified objective.
        /// </summary>
        private static ActionPlanTask CreateDailyWeightMeasurementActionPlanTask(string objectiveId, Guid planId = default(Guid))
        {
            var task = new ActionPlanTask
            {
                Name = "Measure your weight",
                ShortDescription = "Measure your weight daily",
                LongDescription = "Measure your weight daily",
                ImageUrl = "https://img-prod-cms-rt-microsoft-com.akamaized.net/cms/api/am/imageFileData/RW680a?ver=b227",
                ThumbnailImageUrl = "https://img-prod-cms-rt-microsoft-com.akamaized.net/cms/api/am/imageFileData/RW6fN6?ver=6479",
                TaskType = "Other",
                SignupName = "Measure your weight",
                AssociatedObjectiveIds = new Collection<string> { objectiveId },
                AssociatedPlanId = planId.ToString(), // Only needs to be set if adding as task after the plan
                TrackingPolicy = new ActionPlanTrackingPolicy
                {
                    IsAutoTrackable = true,
                    OccurrenceMetrics = new ActionPlanTaskOccurrenceMetrics
                    {
                        EvaluateTargets = false
                    },
                    TargetEvents = new Collection<ActionPlanTaskTargetEvent>
                    {

                        new ActionPlanTaskTargetEvent
                        {
                            ElementXPath = "thing/data-xml/weight",
                        }
                    }
                },
                CompletionType = "Frequency",
                FrequencyTaskCompletionMetrics = new ActionPlanFrequencyTaskCompletionMetrics()
                {
                    ReminderState = "Off",
                    ScheduledDays = new Collection<string> { "Everyday" },
                    OccurrenceCount = 1,
                    WindowType = "Daily"
                }
            };

            return task;
        }
    }
}
