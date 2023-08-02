using HW.Backend.DAL.Data;
using HW.Common.DataTransferObjects;
using HW.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HW.Common.Exceptions;
using HW.Backend.DAL.Data.Entities;

namespace HW.Backend.BL.Services;

public class TestService : ITestService
{
    private readonly ILogger<TestService> _logger;
    private readonly BackendDbContext _dbContext;

    public TestService(BackendDbContext dbContext, ILogger<TestService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task AddSimpleTestToChapter(Guid chapterId, TestSimpleCreateDto testModel)
    {
        var chapter = await _dbContext.Chapters
            .Include(m => m.ChapterTests)!
            .FirstOrDefaultAsync(n => n.Id == chapterId && !n.ArchivedAt.HasValue);
        if (chapter == null)
            throw new NotFoundException("Chapter not found");

        var test = new SimpleAnswerTest
        {
            Id = new Guid(),
            CreatedAt = DateTime.Now,
            Chapter = chapter,
            Question = testModel.Question ?? throw new BadRequestException("Test has no question"),
            Files = testModel.FileIds,
            TestType = testModel.TestType, //<------ разницы енамов?
            PossibleAnswers = testModel.PossibleAnswers != null
                ? testModel.PossibleAnswers.Select(n => new SimpleAnswer
                {
                    Id = new Guid(),
                    AnswerContent = n.AnswerContent,
                    IsRight = n.isRight,
                    SimpleAnswerTest = test ////<---- нужна ли тут ссылка?
                }).ToList()
                : throw new BadRequestException("Test has no answers"),
        };

        await _dbContext.AddAsync(test);
        await _dbContext.SaveChangesAsync();
    }

    public async Task EditSimpleTest(Guid testId, TestSimpleCreateDto model)
    {
        var test = await _dbContext.SimpleAnswerTests
            .FirstOrDefaultAsync(n => n.Id == testId && !n.ArchivedAt.HasValue);
        if (test == null)
            throw new NotFoundException("Test not found");

        test.Question = model.Question;
        ///// <--------может ли преподаватель изменять симп тест на другой тип?

        _dbContext.Update(test);
        await _dbContext.SaveChangesAsync();
    }

    public async Task SaveAnswerSimpleTest(Guid testId, List<UserAnswerSimpleDto> userAnswers)
    {
        throw new NotImplementedException();
    }

    public async Task AnswerSimpleTest(Guid testId, List<UserAnswerSimpleDto> userAnswers)
    {
        throw new NotImplementedException();
    }

    public async Task AddCorrectSequenceTestToChapter(Guid chapterId, TestCorrectSequenceCreateDto testModel)
    {
        var chapter = await _dbContext.Chapters
            .Include(m => m.ChapterTests)!
            .FirstOrDefaultAsync(n => n.Id == chapterId && !n.ArchivedAt.HasValue);
        if (chapter == null)
            throw new NotFoundException("Chapter not found");

        var test = new CorrectSequenceTest
        {
            Id = new Guid(),
            CreatedAt = DateTime.Now,
            Chapter = chapter,
            Question = testModel.Question,
            Files = testModel.FileIds, 
            TestType = testModel.TestType, //<------ разницы енамов?
            PossibleAnswers = testModel.PossibleAnswers != null
                ? testModel.PossibleAnswers.Select(n => new CorrectSequenceAnswer
                {
                    Id = new Guid(),
                    AnswerContent = n.AnswerContent,
                    RightOrder = n.RightOrder,
                    CorrectSequenceTest = test, //<------------нужна ли тут ссылка?
                }).ToList()
                : throw new BadRequestException("Test has no answers"),
        };

        await _dbContext.AddAsync(test);
        await _dbContext.SaveChangesAsync();
    }

    public async Task EditCorrectSequenceTest(Guid testId, TestCorrectSequenceCreateDto model)
    {
        var test = await _dbContext.CorrectSequenceTest
            .FirstOrDefaultAsync(n => n.Id == testId && !n.ArchivedAt.HasValue);
        if (test == null)
            throw new NotFoundException("Test not found");

        test.Question = model.Question;
        ///// <--------может ли преподаватель изменять симп тест на другой тип?

        _dbContext.Update(test);
        await _dbContext.SaveChangesAsync();
    }


    public async Task AnswerCorrectSequenceTest(Guid testId, List<UserAnswerCorrectSequenceDto> userAnswers)
    {
        throw new NotImplementedException();
    }

    public async Task SaveAnswerCorrectSequenceTest(Guid testId, List<UserAnswerCorrectSequenceDto> userAnswers)
    {
        throw new NotImplementedException();
    }

    public async Task ArchiveTest(Guid testId) //<---- архивация равна удалению? или учитель может разархивировать тесты?
    {
        var test = await _dbContext.Tests
            .FirstOrDefaultAsync(n => n.Id == testId);
        if (test == null)
            throw new NotFoundException("Test not found");
        if (test.ArchivedAt.HasValue)
            throw new ConflictException("Test already archived");
        test.ArchivedAt = DateTime.UtcNow;
        _dbContext.Update(test);
        await _dbContext.SaveChangesAsync();
    }
}
