using HW.Backend.DAL.Data;
using HW.Backend.DAL.Data.Entities;
using HW.Common.DataTransferObjects;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
        var user = await _dbContext.Students
            .FirstOrDefaultAsync(u => u.Id == userId) ?? throw new NotFoundException("User with this id not found");

        var userLearned = await _dbContext.Learned
            .Where(u => u.LearnDate.HasValue && u.LearnedBy == user)
            .ToListAsync();

        var userAnswers = await _dbContext.UserAnswerTests
            .Where(s => s.AnsweredAt.HasValue && s.Student == user)
            .ToListAsync();

        var yearActivity = new YearActivityDto
        {
            DayActivities = new List<DayActivityDto>()
        };

        for (var i = 365; i >= 0;  i--)
        {
            var tempDay = DateTime.UtcNow.AddDays(-i);

            var userLearnedDayCount = userLearned
                .Where(d => DateOnly.FromDateTime((DateTime)d.LearnDate) == DateOnly.FromDateTime(tempDay))
                .Count();

            var userAnswersDayCount = userAnswers
                .Where(d => DateOnly.FromDateTime((DateTime)d.AnsweredAt) == DateOnly.FromDateTime(tempDay))
                .Count();

            var dayActivity = new DayActivityDto
            {
                Date = tempDay,
                UserAnswers = userAnswersDayCount,
                UserLearned = userLearnedDayCount
            };

            yearActivity.DayActivities.Add(dayActivity);
        }

        return yearActivity;
    }
}