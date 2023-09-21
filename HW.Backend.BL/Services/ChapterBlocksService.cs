using HW.Backend.DAL.Data;
using HW.Backend.DAL.Data.Entities;
using HW.Common.DataTransferObjects;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace HW.Backend.BL.Services; 

public class ChapterBlocksService : IChapterBlocksService {
    private readonly ILogger<ChapterService> _logger;
    private readonly BackendDbContext _dbContext;
    private readonly IFileService _fileService;

    public ChapterBlocksService(ILogger<ChapterService> logger, BackendDbContext dbContext, IFileService fileService) {
        _logger = logger;
        _dbContext = dbContext;
        _fileService = fileService;
    }

    public async Task EditChapterBlocksOrder(List<Guid> orderedChapterBlocks, Guid chapterId) {
        var duplicates = orderedChapterBlocks.GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(y => y.Key)
            .ToList();
        if (duplicates.Count > 0)
            throw new BadRequestException("There are duplicates: " + string.Join(", ", duplicates.Select(x => x)));
        var chapter = await _dbContext.Chapters
            .Include(m=>m.ChapterBlocks)
            .FirstOrDefaultAsync(m => m.Id == chapterId);
        if (chapter == null)
            throw new NotFoundException("Chapter not found");
        if (chapter.ChapterBlocks.IsNullOrEmpty() || chapter.ChapterBlocks!.All(c => c.ArchivedAt.HasValue))
            throw new ConflictException("There are no existing chapter blocks");
        var missingChapterBlocks = chapter.ChapterBlocks!
            .Where(c=>!c.ArchivedAt.HasValue)
            .Select(c=>c.Id)
            .Except(orderedChapterBlocks)
            .ToList();
        if (missingChapterBlocks.Any())
            throw new ConflictException("These chapter blocks are missing: " 
                                        + string.Join(", ", missingChapterBlocks.Select(x => x)));
        var notExistingChapterBlocks = orderedChapterBlocks.Except(chapter.ChapterBlocks!
                .Where(c=>!c.ArchivedAt.HasValue)
                .Select(c=>c.Id)
                .ToList())
            .ToList();
        if (notExistingChapterBlocks.Any())
            throw new ConflictException("These chapter blocks do not exist: " 
                                        + string.Join(", ", notExistingChapterBlocks.Select(x => x)));
        chapter.OrderedBlocks = orderedChapterBlocks;
        _dbContext.Update(chapter);
        await _dbContext.SaveChangesAsync();    
    }
    
    public async Task CreateChapterBlock(Guid chapterId, ChapterBlockCreateDto model) {
        var chapter = await _dbContext.Chapters
            .FirstOrDefaultAsync(m => m.Id == chapterId && !m.ArchivedAt.HasValue);
        if (chapter == null) 
            throw new NotFoundException("Chapter not found");
        var chapterBlock = new ChapterBlock {
            Content = model.Content,
            Files = model.FileIds,
            Chapter = chapter
        };
        chapter.OrderedBlocks!.Add(chapterBlock.Id);
        await _dbContext.AddAsync(chapterBlock);
        await _dbContext.SaveChangesAsync();
    }

    public async Task EditChapterBlock(Guid chapterBlockId, ChapterBlockEditDto model) {
        var chapterBlock = await _dbContext.ChapterBlocks
            .FirstOrDefaultAsync(m => m.Id == chapterBlockId && !m.ArchivedAt.HasValue);
        if (chapterBlock == null) 
            throw new NotFoundException("Chapter block not found");
        chapterBlock.Content = model.Content;
        chapterBlock.Files = model.FileIds;
        _dbContext.Update(chapterBlock);
        await _dbContext.SaveChangesAsync();
    }

    public async Task ArchiveChapterBlock(Guid chapterBlockId) {
        var chapterBlock = await _dbContext.ChapterBlocks
            .Include(c=>c.Chapter)
            .FirstOrDefaultAsync(m => m.Id == chapterBlockId);
        if (chapterBlock == null) 
            throw new NotFoundException("Chapter block not found");
        if (chapterBlock.ArchivedAt.HasValue) 
            throw new ConflictException("Already archived");
        chapterBlock.ArchivedAt = DateTime.UtcNow;
        chapterBlock.Chapter.OrderedBlocks!.Remove(chapterBlockId);
        _dbContext.Update(chapterBlock);
        await _dbContext.SaveChangesAsync();
    }
}