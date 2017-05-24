// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Defines a method for registering all the thing types with the
    /// <see cref="ItemTypeManager"/> that are in this assembly.
    /// </summary>
    ///
    public static class ItemTypeRegistrar
    {
        /// <summary>
        /// Registers all the thing types in this assembly with the
        /// <see cref="ItemTypeManager"/>.
        /// </summary>
        ///
        public static void RegisterAssemblyHealthRecordItemTypes()
        {
            foreach (ItemTypeManager.DefaultTypeHandler typeHandler in s_defaultTypeHandlers)
            {
                ItemTypeManager.RegisterTypeHandler(typeHandler.TypeId, typeHandler.Type, true);
            }
        }

        private static readonly ItemTypeManager.DefaultTypeHandler[] s_defaultTypeHandlers =
            {
                new ItemTypeManager.DefaultTypeHandler(ApplicationDataReference.TypeId, typeof(ApplicationDataReference)),
                new ItemTypeManager.DefaultTypeHandler(BasicV2.TypeId, typeof(BasicV2)),
                new ItemTypeManager.DefaultTypeHandler(BloodOxygenSaturation.TypeId, typeof(BloodOxygenSaturation)),
                new ItemTypeManager.DefaultTypeHandler(BodyComposition.TypeId, typeof(BodyComposition)),
                new ItemTypeManager.DefaultTypeHandler(BodyDimension.TypeId, typeof(BodyDimension)),
////                new ItemTypeManager.DefaultTypeHandler(BlueButtonTextFile.TypeId, typeof(BlueButtonTextFile)),
                new ItemTypeManager.DefaultTypeHandler(CalorieGuideline.TypeId, typeof(CalorieGuideline)),
                new ItemTypeManager.DefaultTypeHandler(CarePlan.TypeId, typeof(CarePlan)),
                new ItemTypeManager.DefaultTypeHandler(CCD.TypeId, typeof(CCD)),
                new ItemTypeManager.DefaultTypeHandler(CCR.TypeId, typeof(CCR)),
                new ItemTypeManager.DefaultTypeHandler(CDA.TypeId, typeof(CDA)),
                new ItemTypeManager.DefaultTypeHandler(Concern.TypeId, typeof(Concern)),
                new ItemTypeManager.DefaultTypeHandler(Contraindication.TypeId, typeof(Contraindication)),
                new ItemTypeManager.DefaultTypeHandler(DefibrillatorEpisode.TypeId, typeof(DefibrillatorEpisode)),
                new ItemTypeManager.DefaultTypeHandler(DietaryIntake.TypeId, typeof(DietaryIntake)),
                new ItemTypeManager.DefaultTypeHandler(Encounter.TypeId, typeof(Encounter)),
                new ItemTypeManager.DefaultTypeHandler(Exercise.TypeId, typeof(Exercise)),
                new ItemTypeManager.DefaultTypeHandler(ExerciseSamples.TypeId, typeof(ExerciseSamples)),
                new ItemTypeManager.DefaultTypeHandler(ExplanationOfBenefits.TypeId, typeof(ExplanationOfBenefits)),
                new ItemTypeManager.DefaultTypeHandler(FamilyHistory.TypeId, typeof(FamilyHistory)),
                new ItemTypeManager.DefaultTypeHandler(FamilyHistoryV3.TypeId, typeof(FamilyHistoryV3)),
                new ItemTypeManager.DefaultTypeHandler(FamilyHistoryCondition.TypeId, typeof(FamilyHistoryCondition)),
                new ItemTypeManager.DefaultTypeHandler(FamilyHistoryPerson.TypeId, typeof(FamilyHistoryPerson)),
                new ItemTypeManager.DefaultTypeHandler(GeneticSnpResults.TypeId, typeof(GeneticSnpResults)),
                new ItemTypeManager.DefaultTypeHandler(GroupMembership.TypeId, typeof(GroupMembership)),
                new ItemTypeManager.DefaultTypeHandler(GroupMembershipActivity.TypeId, typeof(GroupMembershipActivity)),
                new ItemTypeManager.DefaultTypeHandler(HealthAssessment.TypeId, typeof(HealthAssessment)),
                new ItemTypeManager.DefaultTypeHandler(HealthEvent.TypeId, typeof(HealthEvent)),
                new ItemTypeManager.DefaultTypeHandler(HealthGoal.TypeId, typeof(HealthGoal)),
                new ItemTypeManager.DefaultTypeHandler(HealthJournalEntry.TypeId, typeof(HealthJournalEntry)),
                new ItemTypeManager.DefaultTypeHandler(HeartRate.TypeId, typeof(HeartRate)),
                new ItemTypeManager.DefaultTypeHandler(Immunization.TypeId, typeof(Immunization)),
                new ItemTypeManager.DefaultTypeHandler(LabTestResults.TypeId, typeof(LabTestResults)),
                new ItemTypeManager.DefaultTypeHandler(MealDefinition.TypeId, typeof(MealDefinition)),
                new ItemTypeManager.DefaultTypeHandler(MedicalImageStudy.TypeId, typeof(MedicalImageStudy)),
                new ItemTypeManager.DefaultTypeHandler(MedicalImageStudyV2.TypeId, typeof(MedicalImageStudyV2)),
                new ItemTypeManager.DefaultTypeHandler(Medication.TypeId, typeof(Medication)),
                new ItemTypeManager.DefaultTypeHandler(MedicationFill.TypeId, typeof(MedicationFill)),
                new ItemTypeManager.DefaultTypeHandler(Message.TypeId, typeof(Message)),
                new ItemTypeManager.DefaultTypeHandler(Comment.TypeId, typeof(Comment)),
                new ItemTypeManager.DefaultTypeHandler(PapSession.TypeId, typeof(PapSession)),
                new ItemTypeManager.DefaultTypeHandler(PeakFlow.TypeId, typeof(PeakFlow)),
                new ItemTypeManager.DefaultTypeHandler(Pregnancy.TypeId, typeof(Pregnancy)),
                new ItemTypeManager.DefaultTypeHandler(Procedure.TypeId, typeof(Procedure)),
                new ItemTypeManager.DefaultTypeHandler(QuestionAnswer.TypeId, typeof(QuestionAnswer)),
                new ItemTypeManager.DefaultTypeHandler(Status.TypeId, typeof(Status)),
                new ItemTypeManager.DefaultTypeHandler(AerobicProfile.TypeId, typeof(AerobicProfile)),
                new ItemTypeManager.DefaultTypeHandler(AerobicWeeklyGoal.TypeId, typeof(AerobicWeeklyGoal)),
                new ItemTypeManager.DefaultTypeHandler(AllergicEpisode.TypeId, typeof(AllergicEpisode)),
                new ItemTypeManager.DefaultTypeHandler(Allergy.TypeId, typeof(Allergy)),
                new ItemTypeManager.DefaultTypeHandler(Annotation.TypeId, typeof(Annotation)),
                new ItemTypeManager.DefaultTypeHandler(ApplicationSpecific.TypeId, typeof(ApplicationSpecific)),
                new ItemTypeManager.DefaultTypeHandler(Appointment.TypeId, typeof(Appointment)),
                new ItemTypeManager.DefaultTypeHandler(AsthmaInhaler.TypeId, typeof(AsthmaInhaler)),
                new ItemTypeManager.DefaultTypeHandler(AsthmaInhalerUse.TypeId, typeof(AsthmaInhalerUse)),
                new ItemTypeManager.DefaultTypeHandler(Basic.TypeId, typeof(Basic)),
                new ItemTypeManager.DefaultTypeHandler(BloodGlucose.TypeId, typeof(BloodGlucose)),
                new ItemTypeManager.DefaultTypeHandler(BloodPressure.TypeId, typeof(BloodPressure)),
                new ItemTypeManager.DefaultTypeHandler(CardiacProfile.TypeId, typeof(CardiacProfile)),
                new ItemTypeManager.DefaultTypeHandler(CholesterolProfile.TypeId, typeof(CholesterolProfile)),
                new ItemTypeManager.DefaultTypeHandler(CholesterolProfileV2.TypeId, typeof(CholesterolProfileV2)),
                new ItemTypeManager.DefaultTypeHandler(Condition.TypeId, typeof(Condition)),
                new ItemTypeManager.DefaultTypeHandler(Contact.TypeId, typeof(Contact)),
                new ItemTypeManager.DefaultTypeHandler(DailyMedicationUsage.TypeId, typeof(DailyMedicationUsage)),
                new ItemTypeManager.DefaultTypeHandler(Device.TypeId, typeof(Device)),
                new ItemTypeManager.DefaultTypeHandler(DiabeticProfile.TypeId, typeof(DiabeticProfile)),
                new ItemTypeManager.DefaultTypeHandler(DietaryDailyIntake.TypeId, typeof(DietaryDailyIntake)),
                new ItemTypeManager.DefaultTypeHandler(Directive.TypeId, typeof(Directive)),
                new ItemTypeManager.DefaultTypeHandler(DischargeSummary.TypeId, typeof(DischargeSummary)),
                new ItemTypeManager.DefaultTypeHandler(Emotion.TypeId, typeof(Emotion)),
                new ItemTypeManager.DefaultTypeHandler(File.TypeId, typeof(File)),
                new ItemTypeManager.DefaultTypeHandler(HbA1C.TypeId, typeof(HbA1C)),
                new ItemTypeManager.DefaultTypeHandler(HbA1CV2.TypeId, typeof(HbA1CV2)),
                new ItemTypeManager.DefaultTypeHandler(HealthcareProxy.TypeId, typeof(HealthcareProxy)),
                new ItemTypeManager.DefaultTypeHandler(Height.TypeId, typeof(Height)),
                new ItemTypeManager.DefaultTypeHandler(InsulinInjection.TypeId, typeof(InsulinInjection)),
                new ItemTypeManager.DefaultTypeHandler(InsulinInjectionUse.TypeId, typeof(InsulinInjectionUse)),
                new ItemTypeManager.DefaultTypeHandler(LifeGoal.TypeId, typeof(LifeGoal)),
                new ItemTypeManager.DefaultTypeHandler(Link.TypeId, typeof(Link)),
                new ItemTypeManager.DefaultTypeHandler(MicrobiologyLabResults.TypeId, typeof(MicrobiologyLabResults)),
                new ItemTypeManager.DefaultTypeHandler(PasswordProtectedPackage.TypeId, typeof(PasswordProtectedPackage)),
                new ItemTypeManager.DefaultTypeHandler(Payer.TypeId, typeof(Payer)),
                new ItemTypeManager.DefaultTypeHandler(Person.TypeId, typeof(Person)),
                new ItemTypeManager.DefaultTypeHandler(Personal.TypeId, typeof(Personal)),
                new ItemTypeManager.DefaultTypeHandler(PersonalImage.TypeId, typeof(PersonalImage)),
                new ItemTypeManager.DefaultTypeHandler(Problem.TypeId, typeof(Problem)),
                new ItemTypeManager.DefaultTypeHandler(RadiologyLabResults.TypeId, typeof(RadiologyLabResults)),
                new ItemTypeManager.DefaultTypeHandler(RespiratoryProfile.TypeId, typeof(RespiratoryProfile)),
                new ItemTypeManager.DefaultTypeHandler(SleepJournalAM.TypeId, typeof(SleepJournalAM)),
                new ItemTypeManager.DefaultTypeHandler(SleepJournalPM.TypeId, typeof(SleepJournalPM)),
                new ItemTypeManager.DefaultTypeHandler(VitalSigns.TypeId, typeof(VitalSigns)),
                new ItemTypeManager.DefaultTypeHandler(Weight.TypeId, typeof(Weight)),
                new ItemTypeManager.DefaultTypeHandler(WeightGoal.TypeId, typeof(WeightGoal)),
                new ItemTypeManager.DefaultTypeHandler(Menstruation.TypeId, typeof(Menstruation)),
                new ItemTypeManager.DefaultTypeHandler(Insight.TypeId, typeof(Insight)),
                new ItemTypeManager.DefaultTypeHandler(ActionPlanWrapper.TypeId, typeof(ActionPlanWrapper))
            };
    }
}
