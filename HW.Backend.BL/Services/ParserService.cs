using System.Drawing;
using System.Net.Mime;
using System.Xml;
using HW.Backend.DAL.Data;
using HW.Backend.DAL.Data.Entities;
using HW.Common.Enums;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using HW.Common.Other;
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
                default:
                    if (subModules.IsNullOrEmpty())
                        throw new InvalidOperationException("Not found heading 1 in paragraph: " +
                                                            sourceDoc.Paragraphs.IndexOf(paragraph));
                    if (subModules.Last().Chapters.IsNullOrEmpty())
                        throw new InvalidOperationException("Not found heading 2: " +
                                                            sourceDoc.Paragraphs.IndexOf(paragraph));
                    var chapterBlocks = subModules.Last().Chapters!.Last().ChapterBlocks;
                    
                    var content = await GetTextWithHtmlTags(paragraph);
                    if (paragraph is { IsListItem: true, PreviousParagraph.IsListItem: true }) {
                        chapterBlocks!.Last().Content += content.Content;
                        chapterBlocks!.Last().Files!.AddRange(content.fileIds);
                    }
                    else {
                        chapterBlocks!.Add(new ChapterBlock {
                            Content = content.Content,
                            Files = content.fileIds,
                            Chapter = subModules.Last().Chapters!.Last()
                        });
                        subModules.Last().Chapters!.Last().OrderedBlocks!.Add(chapterBlocks.Last().Id);
                    }
                    break;
            }
        }

        await _dbContext.AddRangeAsync(subModules);
        await _dbContext.SaveChangesAsync();
        var a = 3;
    }

    private async Task<HtmlContentAndFiles> GetTextWithHtmlTags(Paragraph paragraph) {
        var content = new HtmlContentAndFiles();
        foreach (var formattedText in paragraph.MagicText) {
            var text = formattedText.text;
            if (formattedText.formatting != null) {
                var url = paragraph.Hyperlinks
                    .FirstOrDefault(h => h.Text == formattedText.text);
                paragraph.Hyperlinks.Remove(url);
                text = formattedText.formatting.Bold == true ? "<strong>" + text + "</strong>" : text;
                text = formattedText.formatting.Italic == true ? "<em>" + text + "</em>" : text;
                text = formattedText.formatting.UnderlineStyle.HasValue
                       && formattedText.formatting.UnderlineColor is not { Name: "ff0000ff" }
                    ? "<u>" + text + "</u>"
                    : text;
                text = formattedText.formatting.UnderlineColor != null
                       && formattedText.formatting.UnderlineStyle.HasValue
                       && formattedText.formatting.UnderlineColor.Value.Name == "ff0000ff"
                    ? url?.Uri.ToString() != null
                        ? "<a href=\"" + url.Uri + "\" rel=\"noopener noreferrer\" target=\"_blank\">" + text + "</a>"
                        : text
                    : text;
            }
            content.Content+= text;
        }

        if (paragraph.IsListItem) {
            content.Content= "<li>" + content.Content+ "</li>";

            if (paragraph.PreviousParagraph is not { IsListItem: true }) {
                content.Content= paragraph.ListItemType == ListItemType.Numbered
                    ? "<ol>" + content.Content
                    : "<ul>" + content.Content;
            }

            if (paragraph.NextParagraph is not { IsListItem: true }) {
                content.Content= paragraph.ListItemType == ListItemType.Numbered
                    ? content.Content+ "</ol>"
                    : content.Content+ "</ul>";
            }
        }

        if (!paragraph.Pictures.IsNullOrEmpty()) {
            var files = new List<IFormFile>();
            foreach (var paragraphPicture in paragraph.Pictures) {
                var memoryStream = new MemoryStream();
                await paragraphPicture.Stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
               
                files.Add(new FormFile(memoryStream, 0, memoryStream.Length,
                    "image", paragraphPicture.FileName) {
                    Headers = new HeaderDictionary(),
                    ContentType = GetContentTypeFromExtension(Path.GetExtension(paragraphPicture.FileName))
                });
            }
            var fileIds = await _fileService.UploadFiles(files);
            content.fileIds = fileIds
                .Select(f => f.NewFileName)
                .ToList();
        }
        if (content.Content.IsNullOrEmpty())
            content.Content = "<br>";
        if(!paragraph.IsListItem && !content.Content.IsNullOrEmpty()) 
            content.Content = "<p>" + content.Content + "</p>";
        foreach (var contentFileId in content.fileIds) {
            content.Content = content.Content + "<img src=\"" + contentFileId + "\" alt = \"\">";
        }
        return content;
    }
    private string GetContentTypeFromExtension(string fileExtension)
    {
        switch (fileExtension.ToLower())
        {
            case ".jpg":
            case ".jpeg":
                return "image/jpeg";
            case ".png":
                return "image/png";
            case ".gif":
                return "image/gif";
            case ".bmp":
                return "image/bmp";
            case ".tiff":
                return "image/tiff";
            case ".pdf":
                return "application/pdf";
            // Другие расширения и их соответствующие contentType
            default:
                throw new ForbiddenException("Wrong picture format");
        }
    }
}