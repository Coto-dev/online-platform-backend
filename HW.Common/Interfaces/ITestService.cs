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
        public Task AddSimpleTestToChapter(Guid chapterId, TestSimpleCreateDto testModel); 
        public Task EditTest(Guid testId, EditTestDto testModel); 
        public Task AddAnswerToSimpleTest(Guid testId, SimpleAnswerCreateDto newAnswer); 
        public Task EditAnswerInSimpleTest(Guid answerId, Guid testId, SimpleAnswerCreateDto answer); 
        public Task DeleteAnswerFromSimpleTest(Guid testId, Guid answerId); 
        public Task SaveAnswerSimpleTest(Guid testId, List<Guid> userAnswers, Guid userId); 
        public Task AnswerSimpleTest(Guid testId, Guid userId); 
        public Task AddCorrectSequenceTestToChapter(Guid chapterId, TestCorrectSequenceCreateDto model); 
        public Task AnswerCorrectSequenceTest(Guid testId, Guid userId); 
        public Task SaveAnswerCorrectSequenceTest(Guid testId, List<Guid> userAnswerIds, Guid userId); 
        public Task OrderAnswersInCorrectSequenceTest(Guid testId, List<Guid> answerIds);
        public Task AddAnswerToSequenceTest(Guid testId, CorrectSequenceAnswerCreateDto newAnswerCreateModel); 
        public Task EditAnswerInSequenceTest(Guid answerId, CorrectSequenceAnswerCreateDto answerCreateModel); 
        public Task DeleteAnswerFromSequenceTest(Guid testId, Guid answerId); 
        public Task AddDetailedTestToChapter(Guid chapterId, TestDetailedCreateDto testModel); 
        public Task SaveAnswerDetailedTest (Guid testId, DetailedAnswerDto userAnswer, Guid userId);
        public Task AnswerDetailedTest(Guid testId, Guid userId); 
        public Task ArchiveTest(Guid testId); 
        public Task EditChapterTestsOrder(List<Guid> orderedChapterTests, Guid chapterId);
    }
}
