using System.Net;
using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using ContentDispositionHeaderValue = System.Net.Http.Headers.ContentDispositionHeaderValue;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;

namespace HW.FileManager.API.Controllers;

/// <summary>
/// Controller for file management
/// </summary>
[ApiController]
[Route("api/file")]
public class FileController : ControllerBase {
   

    private readonly ILogger<FileController> _logger;
    private readonly IFileService _fileService;
    /// <summary>
    /// Controller constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="fileService"></param>
    public FileController(ILogger<FileController> logger, IFileService fileService) {
        _logger = logger;
        _fileService = fileService;
    }

    /// <summary>
    /// Upload file
    /// </summary>
    /// <param name="files"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("upload")]
    public async Task<ActionResult<List<FileKeyDto>>> UploadFiles(List<IFormFile> files) {
        return Ok(await _fileService.UploadFiles(files));
    }

    /// <summary>
    /// Remove list of file
    /// </summary>
    /// <param name="fileIds"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("remove/list")]
    public async Task<ActionResult> RemoveFile(List<string> fileIds) {
        await _fileService.RemoveFiles(fileIds);
        return Ok();
    }
    
    /// <summary>
    /// Download file
    /// </summary>
    /// <param name="fileNames"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Administrator)]
    [Route("download")]
    public async Task<HttpResponseMessage> DownloadFiles([FromQuery] List<string> fileNames) {
        var byteFiles = await _fileService.GetFiles(fileNames);
        var files = new List<FileContentResult>();
        byteFiles.ForEach(f=>
            files.Add(File(f.Content,f.ContentType,true)));
        var fileContentList = new List<ByteArrayContent>();

        foreach (var byteContent in byteFiles)
        {
            var fileContent = new ByteArrayContent(byteContent.Content);

            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = byteContent.Name
            };

            fileContent.Headers.ContentType = new MediaTypeHeaderValue(byteContent.ContentType);

            fileContentList.Add(fileContent);
        }
        
        var multipartContent = new MultipartContent();

        foreach (var fileContent in fileContentList)
        {
            multipartContent.Add(fileContent);
        }

        // Возвращение HttpResponseMessage с MultipartContent в качестве содержимого
        var response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Content = multipartContent;

        return response;
        //return files;
    }
    
    /// <summary>
    /// Download 2
    /// </summary>
    /// <param name="fileNames"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Administrator)]
    [Route("download1")]
    public async Task<FileContentResult> DownloadFiles1([FromQuery] List<string> fileNames) {
        var byteFiles = await _fileService.GetFiles(fileNames);
        var files = new List<FileContentResult>();
       
        
        return File(byteFiles[0].Content,byteFiles[0].ContentType,true);
    }
}