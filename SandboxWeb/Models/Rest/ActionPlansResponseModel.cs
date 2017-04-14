// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;

namespace SandboxWeb.Models.Rest
{
    public class ActionPlansResponseModel
    {
        public Plan[] Plans { get; set; }
        public string NextLink { get; set; }
    }

    public class Plan
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public Associatedtask[] AssociatedTasks { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string ThumbnailImageUrl { get; set; }
        public string OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationLogoUrl { get; set; }
        public string OrganizationLongFormImageUrl { get; set; }
        public string Category { get; set; }
        public Objective[] Objectives { get; set; }
    }

    public class Associatedtask
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string ImageUrl { get; set; }
        public string ThumbnailImageUrl { get; set; }
        public string OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string TaskType { get; set; }
        public Trackingpolicy TrackingPolicy { get; set; }
        public string SignupName { get; set; }
        public string AssociatedPlanId { get; set; }
        public string[] AssociatedObjectiveIds { get; set; }
        public string CompletionType { get; set; }
        public Frequencytaskcompletionmetrics FrequencyTaskCompletionMetrics { get; set; }
        public Scheduledtaskcompletionmetrics ScheduledTaskCompletionMetrics { get; set; }
    }

    public class Trackingpolicy
    {
        public bool IsAutoTrackable { get; set; }
        public Occurrencemetrics OccurrenceMetrics { get; set; }
        public Targetevent[] TargetEvents { get; set; }
    }

    public class Occurrencemetrics
    {
        public bool EvaluateTargets { get; set; }
        public Target[] Targets { get; set; }
    }

    public class Target
    {
        public string PropertyName { get; set; }
        public string ValueType { get; set; }
        public int MaxTarget { get; set; }
        public int MinTarget { get; set; }
        public string PropertyXPath { get; set; }
    }

    public class Targetevent
    {
        public string ElementXPath { get; set; }
        public string[] ElementValues { get; set; }
        public bool IsNegated { get; set; }
    }

    public class Frequencytaskcompletionmetrics
    {
        public string WindowType { get; set; }
        public int OccurrenceCount { get; set; }
        public string ReminderState { get; set; }
        public string[] ScheduledDays { get; set; }
    }

    public class Scheduledtaskcompletionmetrics
    {
        public Scheduledtime ScheduledTime { get; set; }
        public string ReminderState { get; set; }
        public string[] ScheduledDays { get; set; }
    }

    public class Scheduledtime
    {
        public int Hours { get; set; }
        public int Minutes { get; set; }
    }

    public class Objective
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string State { get; set; }
        public string OutcomeName { get; set; }
        public string OutcomeType { get; set; }
    }
}