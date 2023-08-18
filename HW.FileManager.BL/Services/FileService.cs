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
                catch (MinioException ex)
                {
                    throw new BadRequestException($"Error uploading file: {ex.Message}");
                }
            }
        }
        return fileNames;
    }

    public async Task<string?> GetAvatarLink(string avatarId) {
        var args = new PresignedGetObjectArgs()
            .WithBucket(_configuration.GetSection("MinioCredentials")["ImageBucketName"])
            .WithObject(avatarId)
            .WithExpiry(1000);
        var presignedUrl = await _minioClient.PresignedGetObjectAsync(args).ConfigureAwait(false);
        return presignedUrl;
    }

    public async Task<string> GetFileLink(string fileId) {
        var bucketName = GetBucketName(fileId);
        var args = new PresignedGetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(fileId)
            .WithExpiry(1000);
        var presignedUrl = await _minioClient.PresignedGetObjectAsync(args).ConfigureAwait(false);
        return presignedUrl;
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
            catch (MinioException ex) {
                throw new BadRequestException($"Error retrieving file: {ex.Message}");
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