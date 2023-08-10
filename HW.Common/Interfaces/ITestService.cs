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
        public Task AddSimpleTestToChapter(Guid chapterId, TestSimpleCreateDto testModel); //done
        public Task EditSimpleTest(Guid testId, TestSimpleCreateDto testModel); //done
        public Task SaveAnswerSimpleTest(Guid testId, UserAnswerSimpleDto userAnswer, Guid userId); //done
        public Task AnswerSimpleTest(Guid testId, Guid userId); //done (saveAnswer and IsAnswered to true)
        public Task AddCorrectSequenceTestToChapter(Guid chapterId, TestCorrectSequenceCreateDto model); //done
        public Task EditCorrectSequenceTest(Guid testId, TestCorrectSequenceCreateDto model); //done
        public Task AnswerCorrectSequenceTest(Guid testId, Guid userId); //done (saveAnswer and IsAnswered to true)
        public Task SaveAnswerCorrectSequenceTest(Guid testId, List<UserAnswerCorrectSequenceDto> userAnswers, Guid userId); //done
        public Task AddDetailedTestToChapter(Guid chapterId, TestDetailedCreateDto testModel);
        public Task EditDetailedTest(Guid testId, TestDetailedCreateDto testModel);
        public Task SaveAnswerDetailedTest (Guid testId);
        public Task AnswerDetailedTest(Guid testId);
        public Task ArchiveTest(Guid testId); //done


    }
}
