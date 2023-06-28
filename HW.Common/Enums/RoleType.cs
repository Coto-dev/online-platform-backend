using System.ComponentModel.DataAnnotations;

namespace HW.Common.Enums; 

/// <summary>
/// Role types
/// </summary>
public enum RoleType {
    /// <summary>
    /// Administrator role
    /// </summary>
    [Display(Name = ApplicationRoleNames.Administrator)]
    Administrator,

    /// <summary>
    /// Manager role
    /// </summary>
    [Display(Name = ApplicationRoleNames.Manager)]
    Manager,

    /// <summary>
    /// Student role
    /// </summary>
    [Display(Name = ApplicationRoleNames.Student)]
    Student,

    /// <summary>
    /// Teacher role
    /// </summary>
    [Display(Name = ApplicationRoleNames.Teacher)]
    Teacher
}

/// <summary>
/// Role names
/// </summary>
public class ApplicationRoleNames {
    /// <summary>
    /// Administrator role name
    /// </summary>
    public const string Administrator = "Administrator";

    /// <summary>
    /// Manager role name
    /// </summary>
    public const string Manager = "Manager";

    /// <summary>
    /// Student role name
    /// </summary>
    public const string Student = "Student";

    /// <summary>
    /// Teacher role name
    /// </summary>
    public const string Teacher = "Teacher";
}