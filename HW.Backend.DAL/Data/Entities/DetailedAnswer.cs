﻿namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for DetailedAnswer
/// </summary>
public class DetailedAnswer : UserAnswer
{
    /// <summary>
    /// DetailedAnswer content 
    /// </summary>
    public required string AnswerContent { get; set; }
    /// <summary>
    /// Accuracy of answer
    /// </summary>
    public int Accuracy { get; set; }

}