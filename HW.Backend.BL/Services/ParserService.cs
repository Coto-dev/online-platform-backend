using System.Drawing;
using System.Xml;
using HW.Backend.DAL.Data;
using HW.Backend.DAL.Data.Entities;
using HW.Common.Enums;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace HW.Backend.BL.Services;

public class ParserService : IParserService {
    private readonly ILogger<ModuleManagerService> _logger;
    private readonly BackendDbContext _dbContext;
    private readonly IFileService _fileService;

    public ParserService(ILogger<ModuleManagerService> logger, BackendDbContext dbContext, IFileService fileService) {
        _logger = logger;
        _dbContext = dbContext;
        _fileService = fileService;
    }

    public async Task ParseFile(IFormFile file, Guid moduleId) {
        var module = await _dbContext.Modules
            .FirstOrDefaultAsync(m => m.Id == moduleId && !m.ArchivedAt.HasValue);
        if (module == null)
            throw new NotFoundException("Module not found");

        List<Paragraph> parags = new List<Paragraph>();
        var subModules = new List<SubModule>();

        await using var stream = file.OpenReadStream();
        var sourceDoc = DocX.Load(stream);
        foreach (var paragraph in sourceDoc.Paragraphs) {
            parags.Add(paragraph);
        }
        foreach (var paragraph in sourceDoc.Paragraphs) {
            switch (paragraph.StyleId) {
                case "1":
                    subModules.Add(new SubModule {
                        Name = paragraph.Text,
                        Module = module
                    });
                    module.OrderedSubModules!.Add(subModules.Last().Id);
                    break;
                case "2":
                    if (subModules.IsNullOrEmpty())
                        throw new InvalidOperationException("Not found heading 1: " +
                                                            sourceDoc.Paragraphs.IndexOf(paragraph));
                    subModules.Last()!.Chapters!.Add(new Chapter {
                        Name = paragraph.Text,
                        SubModule = subModules.Last(),
                        ChapterType = ChapterType.DefaultChapter
                    });
                    break;
                case "a":
                    if (subModules.IsNullOrEmpty())
                        throw new InvalidOperationException("Not found heading 1 in paragraph: " +
                                                            sourceDoc.Paragraphs.IndexOf(paragraph));
                    if (subModules.Last().Chapters.IsNullOrEmpty())
                        throw new InvalidOperationException("Not found heading 2: " +
                                                            sourceDoc.Paragraphs.IndexOf(paragraph));
                    var chapterBlocks = subModules.Last().Chapters!.Last().ChapterBlocks;
                    var content = await GetTextWithHtmlTags(paragraph);
                    chapterBlocks!.Add(new ChapterBlock {
                        Content = content,
                        //Files = null,
                        Chapter = subModules.Last().Chapters!.Last()
                    });
                    subModules.Last().Chapters!.Last().OrderedBlocks!.Add(chapterBlocks.Last().Id);
                    parags.Add(paragraph);
                    break;
            }
        }

        await _dbContext.AddRangeAsync(subModules);
        await _dbContext.SaveChangesAsync();
        var a = 3;
    }

    private async Task<string> GetTextWithHtmlTags(Paragraph paragraph) {
        string content = "";    
        foreach (var formattedText in paragraph.MagicText) {
            var text = formattedText.text;
            text = formattedText.formatting.Bold == true?   "<strong>" + text + "</strong>" : text;
            text = formattedText.formatting.Italic == true?  "<em>" + text + "</em>" : text;
            text = formattedText.formatting.UnderlineStyle.HasValue && formattedText.formatting.UnderlineColor != Color.Blue ? "<u>" + text + "</u>" : text;
            content += text;
        }
        return content;
    }
}