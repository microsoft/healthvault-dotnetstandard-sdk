// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.HealthVault.Client;
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
        private const string ObjectiveName = "Manage your weight";
        private const string PlanName = "Track your weight";

        [TestMethod]
        public async Task SimpleActionPlans()
        {
            IHealthVaultSodaConnection connection = HealthVaultConnectionFactory.Current.GetOrCreateSodaConnection(Constants.Configuration);
            PersonInfo personInfo = await connection.GetPersonInfoAsync();
            HealthRecordInfo record = personInfo.SelectedRecord;

            var restClient = connection.CreateMicrosoftHealthVaultRestApi(record.Id);

            await RemoveAllActionPlansAsync(restClient);

            Guid objectiveId = Guid.NewGuid();
            await restClient.ActionPlans.CreateAsync(CreateWeightActionPlan(objectiveId));

            ActionPlansResponseActionPlanInstance plans = await restClient.ActionPlans.GetAsync();
            Assert.AreEqual(1, plans.Plans.Count);

            ActionPlanInstance planInstance = plans.Plans[0];
            Assert.AreEqual(objectiveId.ToString(), planInstance.Objectives[0].Id);
            Assert.AreEqual(ObjectiveName, planInstance.Objectives[0].Name);
            Assert.AreEqual(PlanName, planInstance.Name);
        }

        private static async Task RemoveAllActionPlansAsync(IMicrosoftHealthVaultRestApi api)
        {
            var plans = await api.ActionPlans.GetAsync();
            foreach (var plan in plans.Plans)
            {
                await api.ActionPlans.DeleteAsync(plan.Id);
            }
        }

        private static ActionPlan CreateWeightActionPlan(Guid objectiveId)
        {
            var plan = new ActionPlan();
            var objective = new Objective
            {
                Id = objectiveId.ToString(),
                Name = ObjectiveName,
                Description = "Manage your weight better by measuring daily. ",
                State = "Active",
                OutcomeName = "Better control over your weight",
                OutcomeType = "Other"
            };

            // Use this if you want to create the task with the plan in one call.
            // You can also create tasks in a separate call after the action plan is created.
            var task = CreateDailyWeightMeasurementActionPlanTask(objective.Id);

            plan.Name = PlanName;
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
