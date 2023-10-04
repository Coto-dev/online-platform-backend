﻿using HW.Backend.DAL.Data;
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
using static System.Net.Mime.MediaTypeNames;
using HW.Common.Enums;
using Microsoft.IdentityModel.Tokens;

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

        var newTest = new SimpleAnswerTest
        {
            Chapter = chapter,
            Question = testModel.Question ?? throw new BadRequestException("Test has no question"),
            Files = testModel.FileIds,
            PossibleAnswers = new List<SimpleAnswer>(),
            TestType = testModel.testType == TestSimpleType.SingleAnswer 
                ? TestType.SingleAnswer 
                : TestType.MultipleAnswer
        };
        chapter.OrderedTests!.Add(newTest.Id);
        await _dbContext.AddAsync(newTest);
        await _dbContext.SaveChangesAsync();
    }

    public async Task SaveAnswerSimpleTest(Guid testId, List<UserAnswerSimpleDto> userAnswers, Guid userId)
    { 
        var student = await _dbContext.Students
            .FirstOrDefaultAsync(n => n.Id == userId);
        if (student == null)
            throw new NotFoundException("Student with this id not found");

        var test = await _dbContext.SimpleAnswerTests
            .Include(n => n.PossibleAnswers)
            .FirstOrDefaultAsync(n => n.Id == testId);
        if (test == null)
            throw new NotFoundException("Test not found");

        if (test.ArchivedAt.HasValue)
            throw new NotFoundException("Test was deleted");

        var existingUserAnswerTest = _dbContext.UserAnswerTests
            .Include(n => n.UserAnswers)!
            .Where(n => n.Test == test && n.Student == student)
            .MaxBy(n => n.NumberOfAttempt);

        if (test.TestType == TestType.SingleAnswer && (userAnswers.Count != 1))
            throw new ConflictException("The number of answers is more than 1");
        

        if (existingUserAnswerTest == null) {
            var newUserAnswerTest = new UserAnswerTest
            {
                Test = test,
                Student = student,
                AnsweredAt = null,
                NumberOfAttempt = 0,
                UserAnswers = new List<UserAnswer>()
            };

            foreach (var userAnswer in userAnswers)
            {
                var newSimpleUserAnswer = new SimpleUserAnswer
                {
                    SimpleAnswer = test.PossibleAnswers.FirstOrDefault(n => n.Id == userAnswer.Id)
                          ?? throw new NotFoundException("Answer not found"),
                    UserAnswerTest = newUserAnswerTest
                };

                newUserAnswerTest.UserAnswers.Add(newSimpleUserAnswer);
            }

            await _dbContext.UserAnswerTests.AddAsync(newUserAnswerTest);
            await _dbContext.SaveChangesAsync();
        }
        else {

            //foreach (var existAnswer in existingUserAnswerTest.UserAnswers) //clear
            //{
            //    _dbContext.Remove(existAnswer);
            //}

            existingUserAnswerTest.UserAnswers = new List<UserAnswer>();

            foreach (var userAnswer in userAnswers)
            {
                var newSimpleUserAnswer = new SimpleUserAnswer
                {
                    SimpleAnswer = test.PossibleAnswers.FirstOrDefault(n => n.Id == userAnswer.Id)
                        ?? throw new NotFoundException("Answer not found"),
                    UserAnswerTest = existingUserAnswerTest
                };

                existingUserAnswerTest.UserAnswers.Add(newSimpleUserAnswer);
            }

            _dbContext.Update(existingUserAnswerTest);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task AnswerSimpleTest(Guid testId, Guid userId)
    {
        var student = await _dbContext.Students
            .FirstOrDefaultAsync(n => n.Id == userId);
        if (student == null)
            throw new NotFoundException("Student with this id not found");

        var test = await _dbContext.SimpleAnswerTests
            .Include(n => n.PossibleAnswers)
            .FirstOrDefaultAsync(n => n.Id == testId);
        if (test == null)
            throw new NotFoundException("Test not found");

        if (test.ArchivedAt.HasValue)
            throw new NotFoundException("Test was deleted");

        var existingUserAnswerTest = await _dbContext.UserAnswerTests
            .Include(n => n.UserAnswers)!
            .FirstOrDefaultAsync(n => n.Test == test && n.Student == student);

        if (existingUserAnswerTest == null)
            throw new NotFoundException("User Answer not found");
        existingUserAnswerTest.AnsweredAt = DateTime.UtcNow;

        _dbContext.Update(existingUserAnswerTest);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddCorrectSequenceTestToChapter(Guid chapterId, TestCorrectSequenceCreateDto testModel)
    {
        var chapter = await _dbContext.Chapters
            .Include(m => m.ChapterTests)!
            .FirstOrDefaultAsync(n => n.Id == chapterId && !n.ArchivedAt.HasValue);
        if (chapter == null)
            throw new NotFoundException("Chapter not found");

        var newTest = new CorrectSequenceTest
        {
            Chapter = chapter,
            Question = testModel.Question,
            Files = testModel.FileIds, 
            PossibleAnswers = new List<CorrectSequenceAnswer>(),
            TestType = TestType.CorrectSequenceAnswer
        };
        chapter.OrderedTests!.Add(newTest.Id);
        await _dbContext.AddAsync(newTest);
        await _dbContext.SaveChangesAsync();
    }

    public async Task SaveAnswerCorrectSequenceTest(Guid testId, List<UserAnswerCorrectSequenceDto> userAnswers, Guid userId)
    {
        var student = await _dbContext.Students
            .FirstOrDefaultAsync(n => n.Id == userId);
        if (student == null)
            throw new NotFoundException("Student with this id not found");

        var test = await _dbContext.CorrectSequenceTest
            .Include(n => n.PossibleAnswers)
            .FirstOrDefaultAsync(n => n.Id == testId);
        if (test == null)
            throw new NotFoundException("Test not found");

        if (test.ArchivedAt.HasValue)
            throw new NotFoundException("Test was deleted");

        if (test.PossibleAnswers.Count < userAnswers.Count)
            throw new ConflictException("The number of answers is more then maximum");

        var existingUserAnswerTest = _dbContext.UserAnswerTests
            .Include(n => n.UserAnswers)!
            .Where(n => n.Test == test && n.Student == student)
            .MaxBy(n => n.NumberOfAttempt);
        
        if (existingUserAnswerTest == null) {
            var newUserAnswerTest = new UserAnswerTest
            {
                Test = test,
                Student = student,
                AnsweredAt = null,
                NumberOfAttempt = 0,
                UserAnswers = new List<UserAnswer>() 
            };

            foreach (var userAnswer in userAnswers) {
                var userAnswerInOrder = new CorrectSequenceUserAnswer
                {
                    UserAnswerTest = newUserAnswerTest,
                    CorrectSequenceAnswer = test.PossibleAnswers.FirstOrDefault(n => n.Id == userAnswer.Id)
                          ?? throw new NotFoundException("Answer not found"),
                    Order = userAnswer.Order,
                };
                newUserAnswerTest.UserAnswers.Add(userAnswerInOrder);
            }

            await _dbContext.UserAnswerTests.AddAsync(newUserAnswerTest);
            await _dbContext.SaveChangesAsync();
        }
        else {
            //foreach (var existAnswer in existingUserAnswerTest.UserAnswers) //clear
            //{
            //    _dbContext.Remove(existAnswer);
            //}
            existingUserAnswerTest.UserAnswers = new List<UserAnswer>();
            foreach (var userAnswer in userAnswers)
            {
                var userAnswersInOrder = new CorrectSequenceUserAnswer
                {
                    UserAnswerTest = existingUserAnswerTest,
                    CorrectSequenceAnswer = test.PossibleAnswers.FirstOrDefault(n => n.Id == userAnswer.Id)
                          ?? throw new NotFoundException("Answer not found"),
                    Order = userAnswer.Order,
                };
                existingUserAnswerTest.UserAnswers.Add(userAnswersInOrder);
            }
            _dbContext.Update(existingUserAnswerTest);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task AnswerCorrectSequenceTest(Guid testId, Guid userId)
    {
        var student = await _dbContext.Students
            .FirstOrDefaultAsync(n => n.Id == userId);
        if (student == null)
            throw new NotFoundException("Student with this id not found");

        var test = await _dbContext.SimpleAnswerTests
            .Include(n => n.PossibleAnswers)
            .FirstOrDefaultAsync(n => n.Id == testId);
        if (test == null)
            throw new NotFoundException("Test not found");

        if (test.ArchivedAt.HasValue)
            throw new NotFoundException("Test was deleted");

        var existingUserAnswerTest = await _dbContext.UserAnswerTests
            .Include(n => n.UserAnswers)!
            .FirstOrDefaultAsync(n => n.Test == test && n.Student == student);

        if (existingUserAnswerTest == null)
            throw new NotFoundException("User Answer not found");
        existingUserAnswerTest.AnsweredAt = DateTime.UtcNow;

        _dbContext.Update(existingUserAnswerTest);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task AddDetailedTestToChapter(Guid chapterId, TestDetailedCreateDto testModel)
    {
        var chapter = await _dbContext.Chapters
            .Include(m => m.ChapterTests)!
            .FirstOrDefaultAsync(n => n.Id == chapterId && !n.ArchivedAt.HasValue);
        if (chapter == null)
            throw new NotFoundException("Chapter not found");

        var newTest = new Test
        {
            Chapter = chapter,
            Question = testModel.Question ?? throw new BadRequestException("Test has no question"),
            Files = testModel.FileIds,
            TestType = TestType.DetailedAnswer
        };
        chapter.OrderedTests!.Add(newTest.Id);
        await _dbContext.AddAsync(newTest);
        await _dbContext.SaveChangesAsync();
    }

    public async Task SaveAnswerDetailedTest(Guid testId, DetailedAnswerDto userAnswer, Guid userId)
    {
        var student = await _dbContext.Students
            .FirstOrDefaultAsync(n => n.Id == userId);
        if (student == null)
            throw new NotFoundException("Student with this id not found");

        var test = await _dbContext.Tests
            .FirstOrDefaultAsync(n => n.Id == testId);
        if (test == null)
            throw new NotFoundException("Test not found");

        if (test.ArchivedAt.HasValue)
            throw new NotFoundException("Test was deleted");

        var existingUserAnswerTest = await _dbContext.UserAnswerTests
            .Include(n => n.UserAnswers)!
            .FirstOrDefaultAsync(n => n.Test == test && n.Student == student);

        if (existingUserAnswerTest == null)
        {
            var newUserAnswerTest = new UserAnswerTest
            {
                Test = test,
                Student = student,
                AnsweredAt = null,
                NumberOfAttempt = 0,
                UserAnswers = new List<UserAnswer>() // <--- DetailedAnswer
            };

            var newDetailedAnswer = new DetailedAnswer
            {
                AnswerContent = userAnswer.AnswerContent,
                Files = userAnswer.Files,
                Accuracy = 0,
                UserAnswerTest = newUserAnswerTest
            };

            newUserAnswerTest.UserAnswers.Add(newDetailedAnswer);

            await _dbContext.UserAnswerTests.AddAsync(newUserAnswerTest);
            await _dbContext.SaveChangesAsync();
        }
        else
        {

            //foreach (var existAnswer in existingUserAnswerTest.UserAnswers) //clear
            //{
            //    _dbContext.Remove(existAnswer);
            //}

            existingUserAnswerTest.UserAnswers = new List<UserAnswer>();
            var newDetailedAnswer = new DetailedAnswer
            {
                AnswerContent = userAnswer.AnswerContent,
                Files = userAnswer.Files,
                Accuracy = 0,
                UserAnswerTest = existingUserAnswerTest
            };

            existingUserAnswerTest.UserAnswers.Add(newDetailedAnswer);

            _dbContext.Update(existingUserAnswerTest);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task AnswerDetailedTest(Guid testId, Guid userId)
    {
        var student = await _dbContext.Students
            .FirstOrDefaultAsync(n => n.Id == userId);
        if (student == null)
            throw new NotFoundException("Student with this id not found");

        var test = await _dbContext.Tests
            .FirstOrDefaultAsync(n => n.Id == testId);
        if (test == null)
            throw new NotFoundException("Test not found");

        if (test.ArchivedAt.HasValue)
            throw new NotFoundException("Test was deleted");

        var existingUserAnswerTest = await _dbContext.UserAnswerTests
            .Include(n => n.UserAnswers)!
            .FirstOrDefaultAsync(n => n.Test == test && n.Student == student);

        if (existingUserAnswerTest == null)
            throw new NotFoundException("User Answer not found");
        existingUserAnswerTest.AnsweredAt = DateTime.UtcNow;

        _dbContext.Update(existingUserAnswerTest);
        await _dbContext.SaveChangesAsync();
    }

    public async Task ArchiveTest(Guid testId)
    {
        var test = await _dbContext.Tests
            .Include(t=>t.Chapter)
            .FirstOrDefaultAsync(n => n.Id == testId);
        if (test == null)
            throw new NotFoundException("Test not found");
        if (test.ArchivedAt.HasValue)
            throw new ConflictException("Test already archived");
        test.ArchivedAt = DateTime.UtcNow;
        _dbContext.Update(test);
        test.Chapter.OrderedTests!.Remove(test.Id);
        await _dbContext.SaveChangesAsync();
    }

    public async Task EditTest(Guid testId, EditTestDto testModel)
    {
        var test = await _dbContext.Tests
            .FirstOrDefaultAsync(n => n.Id == testId && !n.ArchivedAt.HasValue);
        if (test == null)
            throw new NotFoundException("Test not found");

        test.Question = testModel.Question;
        test.Files = testModel.FileIds;

        _dbContext.Update(test);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddAnswerToSimpleTest(Guid testId, SimpleAnswerCreateDto newAnswerModel)
    {
        var test = await _dbContext.SimpleAnswerTests
            .Include(n => n.PossibleAnswers)!
            .FirstOrDefaultAsync(n => n.Id == testId);

        var newAnswer = new SimpleAnswer
        {
            AnswerContent = newAnswerModel.AnswerContent,
            IsRight = newAnswerModel.isRight,
            SimpleAnswerTest = test ?? throw new NotFoundException("Test not found")
        };
        
        test.PossibleAnswers.Add(newAnswer);
        _dbContext.Update(test);
        await _dbContext.AddAsync(newAnswer);
        await _dbContext.SaveChangesAsync();
    }

    public async Task EditAnswerInSimpleTest(Guid answerId, Guid testId, SimpleAnswerCreateDto answerModel)
    {
        var answer = await _dbContext.SimpleAnswers
            .FirstOrDefaultAsync(n => n.Id == answerId) ?? throw new NotFoundException("Answer not found");
        var test = await _dbContext.SimpleAnswerTests
            .Include(t=>t.PossibleAnswers)
            .FirstOrDefaultAsync(n => n.Id == testId);
        if (test!.TestType == TestType.SingleAnswer)
            foreach (var simpleAnswer in test.PossibleAnswers.Where(pa=>pa.IsRight)) {
                simpleAnswer.IsRight = false;
            }

        answer.AnswerContent = answerModel.AnswerContent;
        answer.IsRight = answerModel.isRight;
        _dbContext.Update(answer); 
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAnswerFromSimpleTest(Guid testId, Guid answerId)
    {
        var test = await _dbContext.SimpleAnswerTests
            .Include(n => n.PossibleAnswers)!
            .FirstOrDefaultAsync(n => n.Id == testId) ?? throw new NotFoundException("Test not found");

        var answer = await _dbContext.SimpleAnswers
            .FirstOrDefaultAsync(n => n.Id == answerId) ?? throw new NotFoundException("Answer not found");

        test.PossibleAnswers.Remove(answer); 
        _dbContext.Update(test);
        _dbContext.Remove(answer);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddAnswerToSequenceTest(Guid testId, CorrectSequenceAnswerCreateDto newAnswerCreateModel)
    {
        var test = await _dbContext.CorrectSequenceTest
            .Include(n => n.PossibleAnswers)!
            .FirstOrDefaultAsync(n => n.Id == testId);

        var newAnswer = new CorrectSequenceAnswer
        {
            AnswerContent = newAnswerCreateModel.AnswerContent,
            RightOrder = newAnswerCreateModel.RightOrder,
            CorrectSequenceTest = test ?? throw new NotFoundException("Test not found")
        };

        test.PossibleAnswers.Add(newAnswer);
        _dbContext.Update(test);
        await _dbContext.AddAsync(newAnswer);
        await _dbContext.SaveChangesAsync();
    }

    public async Task EditAnswerInSequenceTest(Guid answerId, CorrectSequenceAnswerCreateDto answerCreateModel)
    {
        var answer = await _dbContext.CorrectSequenceAnswers
            .FirstOrDefaultAsync(n => n.Id == answerId) ?? throw new NotFoundException("Answer not found");

        answer.AnswerContent = answerCreateModel.AnswerContent;
        answer.RightOrder = answerCreateModel.RightOrder;

        _dbContext.Update(answer);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAnswerFromSequenceTest(Guid testId, Guid answerId)
    {
        var test = await _dbContext.CorrectSequenceTest
            .Include(n => n.PossibleAnswers)!
            .FirstOrDefaultAsync(n => n.Id == testId) ?? throw new NotFoundException("Test not found");

        var answer = await _dbContext.CorrectSequenceAnswers
            .FirstOrDefaultAsync(n => n.Id == answerId) ?? throw new NotFoundException("Answer not found");

        test.PossibleAnswers.Remove(answer);
        _dbContext.Update(test);
        _dbContext.Remove(answer);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task EditChapterTestsOrder(List<Guid> orderedChapterTests, Guid chapterId) {
        var duplicates = orderedChapterTests.GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(y => y.Key)
            .ToList();
        if (duplicates.Count > 0)
            throw new BadRequestException("There are duplicates: " + string.Join(", ", duplicates.Select(x => x)));
        var chapter = await _dbContext.Chapters
            .Include(m=>m.ChapterTests)
            .FirstOrDefaultAsync(m => m.Id == chapterId);
        if (chapter == null)
            throw new NotFoundException("Chapter not found");
        if (chapter.ChapterTests.IsNullOrEmpty() || chapter.ChapterTests!.All(c => c.ArchivedAt.HasValue))
            throw new ConflictException("There are no existing chapter tests");
        var missingChapterTests = chapter.ChapterTests!
            .Where(c=>!c.ArchivedAt.HasValue)
            .Select(c=>c.Id)
            .Except(orderedChapterTests)
            .ToList();
        if (missingChapterTests.Any())
            throw new ConflictException("These chapter tests are missing: " 
                                        + string.Join(", ", missingChapterTests.Select(x => x)));
        var notExistingChapterTests = orderedChapterTests.Except(chapter.ChapterTests!
                .Where(c=>!c.ArchivedAt.HasValue)
                .Select(c=>c.Id)
                .ToList())
            .ToList();
        if (notExistingChapterTests.Any())
            throw new ConflictException("These chapter tests do not exist: " 
                                        + string.Join(", ", notExistingChapterTests.Select(x => x)));
        chapter.OrderedTests = orderedChapterTests;
        _dbContext.Update(chapter);
        await _dbContext.SaveChangesAsync();
        
    }
}
