// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Configuration.Tests;

public class UserOptions
{
    [ConfigurationRecord("FirstName", "Description")]
    public string FirstName { get; set; }
    [ConfigurationRecord("Gender", "Gender")]
    public Genders Gender { get; set; }
    [ConfigurationRecord("Status", "Status")]
    public List<Status> Status { get; set; }
    [ConfigurationRecord("IsActive", "IsActive")]
    public bool IsActive { get; set; }
    [ConfigurationRecord("Age", "Age")]
    public int Age { get; set; }
    [ConfigurationRecord("Password", "Password", true)]
    public string Password { get; set; }
    [ConfigurationRecord("BirthDate", "BirthDate")]
    public DateTime BirthDateTime { get; set; }
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