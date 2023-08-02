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
        public Task EditSimpleTest(Guid testId, TestSimpleCreateDto testModel);
        public Task SaveAnswerSimpleTest(Guid testId, List<UserAnswerSimpleDto> userAnswers);
        public Task AnswerSimpleTest(Guid testId, List<UserAnswerSimpleDto> userAnswers);
        public Task AddCorrectSequenceTestToChapter(Guid chapterId, TestCorrectSequenceCreateDto model);
        public Task EditCorrectSequenceTest(Guid testId, TestCorrectSequenceCreateDto model);
        public Task AnswerCorrectSequenceTest(Guid testId, List<UserAnswerCorrectSequenceDto> userAnswers);
        public Task SaveAnswerCorrectSequenceTest(Guid testId, List<UserAnswerCorrectSequenceDto> userAnswers);
        public Task ArchiveTest(Guid testId);


    }
}
