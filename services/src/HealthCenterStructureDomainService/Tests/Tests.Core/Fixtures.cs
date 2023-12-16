using System;
using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.Dsl;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Constants;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Enums;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.Core
{
    public class Fixtures
    {
        public const int DefaultAutofixtureMaxStringLength = 36;

        public static IPostprocessComposer<Patient> GetPatientComposer(Guid? id = null, string? firstName = null, string? lastName = null, string? phoneNumber = null, string? innerId = null,
            Guid? interdisciplinaryTeamId = null, Guid? healthCenterId = null, Guid? primaryCareProviderId = null, bool isDeleted = false, DateTime? birthDate = null, DateTime? createdOn = null, DateTime? updatedOn = null, uint? timestamp = null)
        {
            var composer = CreateComposer<Patient>();
            composer = SetValue(composer, p => p.Id, id);
            composer = SetStringValue(composer, pc => pc.FirstName, firstName, FieldLengthConstants.FirstOrLastNameLength);
            composer = SetStringValue(composer, pc => pc.LastName, lastName, FieldLengthConstants.FirstOrLastNameLength);
            composer = SetValue(composer, p => p.PhoneNumber, phoneNumber);
            composer = SetValue(composer, p => p.BirthDate, birthDate);
            composer = SetStringValue(composer, pc => pc.InnerId, innerId, FieldLengthConstants.InnerIdLength);
            composer = composer.With(p => p.InterdisciplinaryTeamId, interdisciplinaryTeamId);
            composer = SetValue(composer, p => p.HealthCenterId, healthCenterId);
            composer = composer.With(p => p.PrimaryCareProviderId, primaryCareProviderId);
            composer = SetValue(composer, p => p.CreatedOn, createdOn);
            composer = SetValue(composer, p => p.UpdatedOn, updatedOn);
            composer = SetValue(composer, p => p.IsDeleted, isDeleted);
            composer = SetValue(composer, p => p.Timestamp, timestamp);
            composer = SetValue(composer, p => p.Type, UserType.Patient);
            composer = composer.Without(p => p.InterdisciplinaryTeam);
            composer = composer.Without(p => p.HealthCenter);
            composer = composer.Without(p => p.PrimaryCareProvider);

            return composer;
        }

        public static IPostprocessComposer<Doctor> GetDoctorComposer(Guid? id = null, string? firstName = null, string? lastName = null, string? phoneNumber = null, string? innerId = null,
            bool isAdmin = false, bool isDeleted = false, DateTime? createdOn = null, DateTime? updatedOn = null, uint? timestamp = null)
        {
            var composer = CreateComposer<Doctor>();
            composer = SetValue(composer, me => me.Id, id);
            composer = SetStringValue(composer, pc => pc.FirstName, firstName, FieldLengthConstants.FirstOrLastNameLength);
            composer = SetStringValue(composer, pc => pc.LastName, lastName, FieldLengthConstants.FirstOrLastNameLength);
            composer = SetValue(composer, p => p.PhoneNumber, phoneNumber);
            composer = SetStringValue(composer, pc => pc.InnerId, innerId, FieldLengthConstants.InnerIdLength);
            composer = SetValue(composer, me => me.CreatedOn, createdOn);
            composer = SetValue(composer, me => me.UpdatedOn, updatedOn);
            composer = SetValue(composer, me => me.Type, UserType.Doctor);
            composer = SetValue(composer, me => me.IsAdmin, isAdmin);
            composer = SetValue(composer, me => me.IsDeleted, isDeleted);
            composer = SetValue(composer, me => me.Timestamp, timestamp);
            composer = composer.Without(me => me.InterdisciplinaryTeams);
            return composer;
        }

        public static IPostprocessComposer<InterdisciplinaryTeam> GetInterdisciplinaryTeamComposer(Guid? id = null, string? name = null, Guid? healthCenterId = null, string? innerId = null,
            bool isDeleted = false, DateTime? createdOn = null, DateTime? updatedOn = null, uint? timestamp = null)
        {
            var composer = CreateComposer<InterdisciplinaryTeam>();
            composer = SetValue(composer, idt => idt.Id, id);
            composer = SetValue(composer, idt => idt.Name, name);
            composer = SetStringValue(composer, pc => pc.InnerId, innerId, FieldLengthConstants.InnerIdLength);
            composer = SetValue(composer, idt => idt.HealthCenterId, healthCenterId);
            composer = SetValue(composer, idt => idt.CreatedOn, createdOn);
            composer = SetValue(composer, idt => idt.UpdatedOn, updatedOn);
            composer = SetValue(composer, idt => idt.IsDeleted, isDeleted);
            composer = SetValue(composer, idt => idt.Timestamp, timestamp);
            composer = composer.Without(idt => idt.Doctors);
            return composer;
        }

        public static IPostprocessComposer<HealthCenter> GetHealthCenterComposer(Guid? id = null, string? name = null, string? innerId = null, string? state = null,
            bool isDeleted = false, DateTime? createdOn = null, DateTime? updatedOn = null, uint? timestamp = null)
        {
            var composer = CreateComposer<HealthCenter>();

            composer = SetValue(composer, pc => pc.Id, id);
            composer = SetValue(composer, pc => pc.Name, name);
            composer = SetStringValue(composer, pc => pc.InnerId, innerId, FieldLengthConstants.InnerIdLength);
            composer = SetStringValue(composer, pc => pc.UsaState, state, FieldLengthConstants.HealthCenterLengths.UsaStateLength);
            composer = SetValue(composer, pc => pc.CreatedOn, createdOn);
            composer = SetValue(composer, pc => pc.UpdatedOn, updatedOn);
            composer = SetValue(composer, pc => pc.IsDeleted, isDeleted);
            composer = SetValue(composer, pc => pc.Timestamp, timestamp);
            composer = composer.Without(pc => pc.Doctors);
            return composer;
        }

        private static IPostprocessComposer<T> CreateComposer<T>()
        {
            return new Fixture().Build<T>();
        }

        private static IPostprocessComposer<T> SetValue<T, TProperty>(IPostprocessComposer<T> composer, Expression<Func<T, TProperty>> propertyPicker, TProperty value)
        {
            return value != null
                ? composer.With(propertyPicker, value)
                : composer.With(propertyPicker);
        }

        private static IPostprocessComposer<T> SetStringValue<T>(IPostprocessComposer<T> composer, Expression<Func<T, string?>> propertyPicker, string? value, int maxFieldLength = -1)
        {
            var fieldValue = value;
            if (fieldValue == null)
            {
                fieldValue = composer.Create<string>();
                var fieldLength = maxFieldLength > 0 ? Math.Min(maxFieldLength, DefaultAutofixtureMaxStringLength) : DefaultAutofixtureMaxStringLength;
                fieldValue = fieldValue.Substring(0, fieldLength);
            }

            return SetValue(composer, propertyPicker, fieldValue);
        }
    }
}
