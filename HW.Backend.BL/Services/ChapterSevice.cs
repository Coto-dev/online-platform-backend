using HW.Backend.DAL.Data;
using HW.Backend.DAL.Data.Entities;
using HW.Common.DataTransferObjects;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HW.Backend.BL.Services;

public class ChapterService : IChapterService
{
    private readonly ILogger<ChapterService> _logger;
    private readonly BackendDbContext _dbContext;

    public ChapterService(ILogger<ChapterService> logger, BackendDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task LearnChapter(Guid chapterId, Guid userId)
    {
        var user = await _dbContext.Students
            .FirstOrDefaultAsync(c => c.Id == userId);

        if (user == null) 
            throw new NotFoundException("User with this id not found");

        var chapter = await _dbContext.Chapters
            .FirstOrDefaultAsync(c => c.Id == chapterId);

        if (chapter == null)
            throw new NotFoundException("Chapter with this id not found");

        var learnedChapter = await _dbContext.Learned
            .FirstOrDefaultAsync(c => c.LearnedBy == user && c.Chapter == chapter);

        if (learnedChapter != null && learnedChapter.LearnDate.HasValue)
            throw new BadRequestException("User already learned this chapter");

        await _dbContext.AddAsync(new Learned
        {
            LearnedBy = user,
            Chapter = chapter,
            LearnDate = DateTime.UtcNow
        });
        await _dbContext.SaveChangesAsync();
    }

    public async Task SendComment(ChapterCommentDto message, Guid chapterId)
    {
        var user = await _dbContext.UserBackends
            .FirstOrDefaultAsync(c => c.Id == message.UserId);

        if (user == null)
            throw new NotFoundException("User with this id not found");

        var chapter = await _dbContext.Chapters
            .FirstOrDefaultAsync(c => c.Id == chapterId);

        if (chapter == null)
            throw new NotFoundException("Chapter with this id not found");

        await _dbContext.AddAsync(new ChapterComment
        {
            Id = Guid.NewGuid(),
            Chapter = chapter,
            User = user,
            IsTeacherComment = message.IsTeacherComment,
            Comment = message.Message
        });
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteComment(Guid commentId, Guid userId)
    {
        var comment = await _dbContext.ChapterComments
            .FirstOrDefaultAsync(c => c.Id == commentId);
        if (comment == null)
            throw new NotFoundException("Comment with this id not found");

        var user = await _dbContext.UserBackends
            .FirstOrDefaultAsync (c => c.Id == userId);
        if (user == null)
            throw new NotFoundException("User with this id not found");

        if (comment.User != user)
            throw new ConflictException("User have not rules to delete this comment");

        var chapter = comment.Chapter;

        _dbContext.ChapterComments.Remove(comment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task EditComment(ChapterCommentSendDto message, Guid commentId, Guid userId)
    {
        var comment = await _dbContext.ChapterComments
            .FirstOrDefaultAsync(c => c.Id == commentId);
        if (comment == null)
            throw new NotFoundException("Comment with this id not found");

        var user = await _dbContext.UserBackends
            .FirstOrDefaultAsync(c => c.Id == userId);
        if (user == null)
            throw new NotFoundException("User with this id not found");

        if (comment.User != user)
            throw new ConflictException("User have not rules to delete this comment");

        comment.Comment = message.Message;
        _dbContext.Update(comment);
        await _dbContext.SaveChangesAsync();
    }
}