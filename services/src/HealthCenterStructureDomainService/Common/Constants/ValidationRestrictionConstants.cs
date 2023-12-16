namespace Telemedicine.Services.HealthCenterStructureDomainService.Common.Constants
{
    public static class ValidationRestrictionConstants
    {
        /// <summary>
        /// Possible length of text for searching by name of person.
        /// FirstName + space + LastName
        /// </summary>
        public const int SearchByNameLength = FieldLengthConstants.FirstOrLastNameLength + 1 + FieldLengthConstants.FirstOrLastNameLength;

        /// <summary>
        /// Maximum allowed quantity of health centers in request filters.
        /// Agreed with business analysts
        /// </summary>
        public const int MaxHealthCenterQuantity = 100;
    }
}
