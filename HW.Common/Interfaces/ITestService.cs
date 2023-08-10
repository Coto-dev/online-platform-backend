using HW.Common.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW.Common.Interfaces
{
    public interface ITestService
    {
        public Task AddSimpleTestToChapter(Guid chapterId, TestSimpleCreateDto testModel); //
        public Task EditSimpleTest(Guid testId, TestSimpleCreateDto testModel); //
        public Task AddAnswerToSimpleTest(Guid testId, SimpleAnswerDto newAnswer); //
        public Task EditAnswerInSimpleTest(Guid answerId, SimpleAnswerDto answer); //
        public Task DeleteAnswerFromSimpleTest(Guid testId, Guid answerId); //
        public Task SaveAnswerSimpleTest(Guid testId, UserAnswerSimpleDto userAnswer, Guid userId); //
        public Task AnswerSimpleTest(Guid testId, Guid userId); //
        public Task AddCorrectSequenceTestToChapter(Guid chapterId, TestCorrectSequenceCreateDto model); //
        public Task EditCorrectSequenceTest(Guid testId, TestCorrectSequenceCreateDto model); //
        public Task AnswerCorrectSequenceTest(Guid testId, Guid userId); //
        public Task SaveAnswerCorrectSequenceTest(Guid testId, List<UserAnswerCorrectSequenceDto> userAnswers, Guid userId); //
        public Task AddAnswerToSequenceTest(Guid testId, CorrectSequenceAnswerDto newAnswerModel); //
        public Task EditAnswerInSequenceTest(Guid answerId, CorrectSequenceAnswerDto answerModel); //
        public Task DeleteAnswerFromSequenceTest(Guid testId, Guid answerId); //
        public Task AddDetailedTestToChapter(Guid chapterId, TestDetailedCreateDto testModel); //
        public Task EditDetailedTest(Guid testId, TestDetailedCreateDto testModel); //
        public Task SaveAnswerDetailedTest (Guid testId, DetailedAnswerDto userAnswer, Guid userId); //
        public Task AnswerDetailedTest(Guid testId, Guid userId); //
        public Task ArchiveTest(Guid testId); //


    }
}
