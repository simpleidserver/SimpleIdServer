// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Configuration.Tests;

public class UserOptions
{
    [ConfigurationRecord("FirstName", "Description", order: 0)]
    public string FirstName { get; set; }
    [ConfigurationRecord("Gender", "Gender", order: 1)]
    public Genders Gender { get; set; }
    [ConfigurationRecord("Status", "Status", order: 2)]
    public List<Status> Status { get; set; }
    [ConfigurationRecord("IsActive", "IsActive", order: 3)]
    public bool IsActive { get; set; }
    [ConfigurationRecord("Age", "Age", order: 4)]
    public int Age { get; set; }
    [ConfigurationRecord("Password", "Password", 5, null, CustomConfigurationRecordType.PASSWORD)]
    public string Password { get; set; }
    [ConfigurationRecord("BirthDate", "BirthDate", order: 6)]
    public DateTime BirthDateTime { get; set; }
    [ConfigurationRecord("IsFemale", "IsFemale", order: 7, displayCondition: "Gender=Female")]
    public string IsFemale { get; set; }
}


public enum Genders
{
    [ConfigurationRecordEnum("Male")]
    MAL = 0,
    [ConfigurationRecordEnum("Female")]
    SEX = 1
}

public enum Status
{
    [ConfigurationRecordEnum("Freelance")]
    FREELANCE = 0,
    [ConfigurationRecordEnum("Employee")]
    EMPLOYEE =1
}