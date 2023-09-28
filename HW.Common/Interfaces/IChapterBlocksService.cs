using HW.Common.DataTransferObjects;

namespace HW.Common.Interfaces; 

public interface IChapterBlocksService {
    Task CreateChapterBlock(Guid chapterId, ChapterBlockCreateDto model);
    Task EditChapterBlock(Guid chapterBlockId, ChapterBlockEditDto model);
    Task ArchiveChapterBlock(Guid chapterBlockId);
    Task EditChapterBlocksOrder(List<Guid> orderedChapterBlocks, Guid chapterId);

}