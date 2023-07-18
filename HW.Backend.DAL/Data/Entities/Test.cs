﻿namespace HW.Backend.DAL.Data.Entities;
/// <summary>
/// Entity for Test
/// </summary>
public class Test
{
    /// <summary>
    /// Test's id
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Chapter identifier
    /// </summary>
    public Chapter Chapter { get; set; }
    /// <summary>
    /// Test's question
    /// </summary>
    public string Question { get; set; }
    /// <summary>
    /// Test's files
    /// </summary>
    public List<Guid>? Files { get; set; }

}
