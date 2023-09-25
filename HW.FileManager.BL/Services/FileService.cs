using System.Text.RegularExpressions;
using HW.Common.DataTransferObjects;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.Exceptions;

namespace HW.FileManager.BL.Services; 

public class FileService : IFileService {
    private readonly MinioClient _minioClient;
    private readonly ILogger<FileService> _logger;
    private readonly IConfiguration _configuration;


    public FileService(IConfiguration configuration, ILogger<FileService> logger) {
        _configuration = configuration;
        _logger = logger;

        _minioClient = new MinioClient()
            .WithEndpoint(_configuration.GetSection("MinioCredentials")["URL"])
                .WithCredentials(
                _configuration.GetSection("MinioCredentials")["Access"],
                _configuration.GetSection("MinioCredentials")["Secret"])
                .WithSSL(_configuration.GetSection("MinioCredentials")["SSL"] == "True")
                .Build();
    }

    public async Task<List<FileKeyDto>> UploadFiles(List<IFormFile> files) {
        List<FileKeyDto> fileNames = new();
        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                await using var stream = file.OpenReadStream(); // await warning
                try {
                    var bucketName = GetBucketName(file.ContentType);
                    var id = Guid.NewGuid();
                    var fileKey = new FileKeyDto {
                        NewFileName = bucketName + id + Path.GetExtension(file.FileName),
                        PreviousFileName = file.FileName
                    };
                    fileNames.Add(fileKey);
                    var putObjectArgs = new PutObjectArgs()
                        .WithBucket(bucketName)
                        .WithStreamData(stream)
                        .WithObjectSize(stream.Length)
                        .WithObject(fileKey.NewFileName)
                        .WithContentType(file.ContentType);
                    await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
                    _logger.LogInformation("Successfully uploaded " + fileKey.NewFileName + " " + file.FileName);
                        
                }
                catch (Exception e)
                {
                    _logger.LogError("Minio not responding" + e.Message);
                    throw new ServiceUnavailableException("Error uploading file");
                }
            }
        }
        return fileNames;
    }

    public async Task RemoveFiles(List<string> fileIds) {
        
        var bucketNameWithFileIds = new Dictionary<string, List<string>>();
        foreach (var fileId in fileIds) {
           if (bucketNameWithFileIds.ContainsKey(GetBucketName(fileId))) 
               bucketNameWithFileIds[GetBucketName(fileId)].Add(fileId);
           else {
               bucketNameWithFileIds[GetBucketName(fileId)] = new List<string>() { fileId };
           }
        }
        
        foreach (var keyValuePair in bucketNameWithFileIds) {

            try {
                var objArgs = new RemoveObjectsArgs()
                    .WithBucket(keyValuePair.Key)
                    .WithObjects(keyValuePair.Value);
              await _minioClient.RemoveObjectsAsync(objArgs).ConfigureAwait(false);
            }
            catch (Exception e) {
                _logger.LogError("Minio not responding" + e.Message);
                throw new ServiceUnavailableException("Error removing file");
            }
           
        }

    }

    public async Task<string?> GetAvatarLink(string avatarId) {
        var args = new PresignedGetObjectArgs()
            .WithBucket(_configuration.GetSection("MinioCredentials")["ImageBucketName"])
            .WithObject(avatarId)
            .WithExpiry(1000);
        try {
            var presignedUrl = await _minioClient.PresignedGetObjectAsync(args).ConfigureAwait(false);
            return presignedUrl;
        }
        catch (Exception e) {
            return null;
        }
    }

    public async Task<string> GetFileLink(string fileId) {
        var bucketName = GetBucketName(fileId);
        var args = new PresignedGetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(fileId)
            .WithExpiry(1000);
        try {
            var presignedUrl = await _minioClient.PresignedGetObjectAsync(args).ConfigureAwait(false);
            return presignedUrl;
        }
        catch (Exception e) {
            return null;
        }
    }

    public async Task<List<FileDownloadDto>> GetFiles(List<string> fileNames) {
        var files = new List<FileDownloadDto>();
        foreach (var fileName in fileNames) {
            try {
                var stream = new MemoryStream();
                var bucketName = GetBucketName(fileName);
                StatObjectArgs statObjectArgs = new StatObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(fileName);
                await _minioClient.StatObjectAsync(statObjectArgs);
                
                var args = new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(fileName)
                    .WithFile(fileName)
                    .WithCallbackStream(data => {
                        data.CopyTo(stream);
                    });
                var objectStat =  await _minioClient.GetObjectAsync(args).ConfigureAwait(false);
              var file = new FileDownloadDto {
                  Name = fileName,
                  ContentType = objectStat.ContentType,
                  Content = stream.ToArray()
              };
              files.Add(file);
            }
            catch (Exception e) {
                throw new BadRequestException($"Error retrieving file: {e.Message}");
            }
        }

        return files;
    }

    public async Task CreateBuckets() {
        var bucketNames = new List<string?> { 
            _configuration.GetSection("MinioCredentials")["ImageBucketName"],
            _configuration.GetSection("MinioCredentials")["AudioBucketName"],
            _configuration.GetSection("MinioCredentials")["VideoBucketName"],
            _configuration.GetSection("MinioCredentials")["TextBucketName"], 
            _configuration.GetSection("MinioCredentials")["ApplicationBucketName"], 
            _configuration.GetSection("MinioCredentials")["OtherBucketName"],
        };
        
        foreach (var bucketName in bucketNames) {
            var beArgs = new BucketExistsArgs()
                .WithBucket(bucketName);
            if (!await _minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false))
            {
                var mbArgs = new MakeBucketArgs()
                    .WithBucket(bucketName);
                await _minioClient.MakeBucketAsync(mbArgs).ConfigureAwait(false);
            }
        }
    }
    
    private string GetBucketName(string contentType)
    {
        if (!string.IsNullOrEmpty(contentType))
        {
            var regex = new Regex(@"image|video|audio|application|text", RegexOptions.IgnoreCase);
            var match = regex.Match(contentType);

            if (match.Success)
            {
                return match.Value.ToLower();
            }
        }
        return "other";
    }
}