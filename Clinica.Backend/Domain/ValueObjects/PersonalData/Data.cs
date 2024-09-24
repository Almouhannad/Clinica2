﻿using Domain.Errors;
using Domain.ValueObjects.PersonalData.Genders;
using SharedKernel.ResultPattern;
using SharedKernel.ValueObjects;

namespace Domain.ValueObjects.PersonalData;

public class Data : ValueObject
{
    private Data(string firstName, string middleName, string lastName,
    Gender gender, DateOnly dateOfBirth)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Gender = gender;
        DateOfBirth = dateOfBirth;
    }

    #region Properties
    public string FirstName { get; private set; } = null!;
    public string MiddleName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string FullName
    {
        get
        {
            return $"{FirstName} {MiddleName} {LastName}";
        }
    }
    public Gender Gender { get; private set; }
    public DateOnly DateOfBirth { get; private set; }
    public int Age
    {
        get
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - DateOfBirth.Year;
            if (today.Month < DateOfBirth.Month || (today.Month == DateOfBirth.Month && today.Day < DateOfBirth.Day))
            {
                age--;
            }
            return age;
        }
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        return [FirstName, MiddleName, LastName, Gender, DateOfBirth];
    }
    #endregion

    #region Factory
    public const int MaxNameLength = 50;
    public static Result<Data> Create(string firstName, string middleName, string lastName,
        Gender gender, DateOnly dateOfBirth)
    {
        #region Validate name
        if (
            // Check nulls or empty
            string.IsNullOrWhiteSpace(firstName)    ||
            string.IsNullOrWhiteSpace(middleName)   ||
            string.IsNullOrWhiteSpace(lastName)     ||

            // Check max length
            firstName.Length > MaxNameLength    ||
            middleName.Length > MaxNameLength   ||
            lastName.Length > MaxNameLength)
        {
            return Result.Failure<Data>(DomainErrors.InvalidInputValues);
        }
        #endregion

        #region Validate DOB
        if (dateOfBirth.CompareTo(DateOnly.FromDateTime(DateTime.Now)) > 0)
        {
            return Result.Failure<Data>(DomainErrors.InvalidInputValues);
        }
        #endregion

        return new Data(firstName, middleName, lastName, gender, dateOfBirth);
    }
    #endregion


}
