using System.Net;
using HW.Common.DataTransferObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HW.Common.Interfaces; 

public interface IFileService {
    public Task<List<FileKeyDto>> UploadFiles(List<IFormFile> files);
    public Task<string?> GetAvatarLink(string avatarId);
    public Task<string> GetFileLink(string fileId);
    public Task<List<FileDownloadDto>> GetFiles(List<string> fileNames);
    public Task CreateBuckets();
}