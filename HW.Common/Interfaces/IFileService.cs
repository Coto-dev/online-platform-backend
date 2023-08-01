using System.Net;
using HW.Common.DataTransferObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HW.Common.Interfaces; 

public interface IFileService {
    public Task<List<FileKeyDto>> UploadFiles(List<IFormFile> files);
    public Task<List<FileDownloadDto>> GetFiles(List<string> fileNames);
    public Task CreateBuckets();
}