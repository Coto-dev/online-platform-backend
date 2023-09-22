using HW.Backend.DAL.Data;
using HW.Backend.DAL.Data.Entities;
using HW.Common.DataTransferObjects;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;

namespace HW.Backend.BL.Services;

public class ActivityService : IActivityService
{
    private readonly ILogger<ChapterService> _logger;
    private readonly BackendDbContext _dbContext;

    public ActivityService(ILogger<ChapterService> logger, BackendDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<YearActivityDto> GetUserActivity(Guid userId)
    {
        DateTime NowDate = DateTime.UtcNow;
        DateTime YearAgoDate = DateTime.UtcNow.AddDays(-365);

        var user = await _dbContext.Students
            .FirstOrDefaultAsync(u => u.Id == userId) ?? throw new NotFoundException("User with this id not found");

        var userLearneds = await _dbContext.Learned
            .Where(d => d.LearnDate.HasValue && d.LearnedBy == user && d.LearnDate >= YearAgoDate && d.LearnDate <= NowDate)
            .GroupBy(u => DateOnly.FromDateTime((DateTime)u.LearnDate))
            .Select(g => new
            {
                LearnDate = g.Key,
                LearnedCount = g.Count()
            })
            .ToListAsync();

        var userAnswers = await _dbContext.UserAnswerTests
            .Where(u => u.AnsweredAt.HasValue && u.Student == user && u.AnsweredAt >= YearAgoDate && u.AnsweredAt <= NowDate)
            .GroupBy(n => DateOnly.FromDateTime((DateTime)n.AnsweredAt))
            .Select(y => new
            {
                AnswerDate = y.Key,
                AnswersCount = y.Count()
            })
            .ToListAsync();

        var yearActivity = new YearActivityDto
        {
            DayActivities = new List<DayActivityDto>(),
            MaxActivity = 0
        };

        for(int i = 365; i >= 0; i--)
        {
            var tempDay = DateTime.UtcNow.AddDays(-i);

            var dayActivity = new DayActivityDto
            {
                Date = tempDay,
                UserAnswers = 0,
                UserLearned = 0
            };

            yearActivity.DayActivities.Add(dayActivity);
        };

        foreach (var userLearned in userLearneds)
        {
            yearActivity.DayActivities.Find(g => DateOnly.FromDateTime(g.Date) == userLearned.LearnDate).UserLearned = userLearned.LearnedCount;
        };

        foreach (var userAnswer in userAnswers)
        {
            yearActivity.DayActivities.Find(g => DateOnly.FromDateTime(g.Date) == userAnswer.AnswerDate).UserAnswers = userAnswer.AnswersCount;
        };

        yearActivity.MaxActivity = yearActivity.DayActivities.Max(d => d.UserAnswers + d.UserLearned);

        return yearActivity;
    }
}