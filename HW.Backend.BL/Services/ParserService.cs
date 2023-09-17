using System.Xml;
using HW.Backend.DAL.Data;
using HW.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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

    public async Task ParseFile(IFormFile file) {
        List<string> textWithHeadings = new List<string>();
        List<string> text = new List<string>();
        List<Paragraph> parags = new List<Paragraph>();
        if (file != null && file.Length > 0) {
            using var doc = DocX.Create("output.docx");
            await using var stream = file.OpenReadStream();
            var sourceDoc = DocX.Load(stream);

            foreach (var paragraph in sourceDoc.Paragraphs) {
                if (paragraph.StyleName.StartsWith("1"))
                    textWithHeadings.Add(paragraph.Text);
                else {
                    text.Add(paragraph.Text);
                }
                parags.Add(paragraph);
            }
            var a = 3;
        }
    }
}