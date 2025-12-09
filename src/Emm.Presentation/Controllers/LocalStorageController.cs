using Emm.Application.Abstractions;
using Emm.Infrastructure.Services;
using Emm.Presentation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Emm.Presentation.Controllers;

[ApiController]
[Route("api/local-storage")]
public class LocalStorageController : ControllerBase
{
    private readonly IFileStorage _fileStorage;

    public LocalStorageController(IFileStorage fileStorage)
    {
        _fileStorage = fileStorage;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file, [FromQuery] string? subfolder = null)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new ApiErrorResponse("FILE_REQUIRED", "No file provided"));
        }

        using var stream = file.OpenReadStream();
        var result = await _fileStorage.SaveFileAsync(stream, file.FileName, subfolder);

        if (result.Success)
        {
            var fileUrl = _fileStorage.GetFileUrl(result.FilePath!);
            return Ok(new
            {
                fileId = result.FileId,
                success = true,
                filePath = result.FilePath,
                fileName = result.FileName,
                originalFileName = result.OriginalFileName,
                fileSize = result.FileSize,
                url = fileUrl
            });
        }

        return BadRequest(new ApiErrorResponse("FILE_UPLOAD_FAILED", result.ErrorMessage ?? "Failed to upload file"));
    }

    [HttpPost("upload/image")]
    public async Task<IActionResult> UploadImage(IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            return BadRequest(new ApiErrorResponse("FILE_REQUIRED", "No file provided"));
        }

        using var stream = image.OpenReadStream();
        var result = await _fileStorage.SaveFileAsync(stream, image.FileName, FileType.Image, "images");

        if (result.Success)
        {
            var fileUrl = _fileStorage.GetFileUrl(result.FilePath!);
            return Ok(new
            {
                fileId = result.FileId,
                success = true,
                filePath = result.FilePath,
                fileName = result.FileName,
                originalFileName = result.OriginalFileName,
                fileSize = result.FileSize,
                url = fileUrl
            });
        }

        return BadRequest(new ApiErrorResponse("IMAGE_UPLOAD_FAILED", result.ErrorMessage ?? "Failed to upload image"));
    }

    [HttpPost("upload-typed")]
    public async Task<IActionResult> UploadFileWithType(IFormFile file, [FromQuery] FileType fileType = FileType.Any, [FromQuery] string? subfolder = null)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new ApiErrorResponse("FILE_REQUIRED", "No file provided"));
        }

        using var stream = file.OpenReadStream();
        var result = await _fileStorage.SaveFileAsync(stream, file.FileName, fileType, subfolder);

        if (result.Success)
        {
            var fileUrl = _fileStorage.GetFileUrl(result.FilePath!);
            return Ok(new
            {
                fileId = result.FileId,
                success = true,
                filePath = result.FilePath,
                fileName = result.FileName,
                originalFileName = result.OriginalFileName,
                fileSize = result.FileSize,
                url = fileUrl,
                expectedFileType = fileType.ToString()
            });
        }

        return BadRequest(new ApiErrorResponse("FILE_UPLOAD_FAILED", result.ErrorMessage ?? "Failed to upload file"));
    }

    [HttpPost("upload-multiple")]
    public async Task<IActionResult> UploadMultipleFiles(List<IFormFile> files, [FromQuery] string? subfolder = null)
    {
        if (files == null || files.Count == 0)
        {
            return BadRequest(new ApiErrorResponse("FILES_REQUIRED", "No files provided"));
        }

        var results = new List<object>();
        var hasErrors = false;

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var result = await _fileStorage.SaveFileAsync(stream, file.FileName, subfolder);

                if (result.Success)
                {
                    var fileUrl = _fileStorage.GetFileUrl(result.FilePath!);
                    results.Add(new
                    {
                        fileId = result.FileId,
                        success = true,
                        filePath = result.FilePath,
                        fileName = result.FileName,
                        originalFileName = result.OriginalFileName,
                        fileSize = result.FileSize,
                        url = fileUrl
                    });
                }
                else
                {
                    hasErrors = true;
                    results.Add(new
                    {
                        success = false,
                        fileName = file.FileName,
                        error = new { code = "FILE_UPLOAD_FAILED", message = result.ErrorMessage }
                    });
                }
            }
        }

        if (hasErrors)
        {
            return BadRequest(new
            {
                error = new { code = "PARTIAL_UPLOAD_FAILURE", message = "One or more files failed to upload" },
                results
            });
        }

        return Ok(results);
    }

    [HttpPost("upload-multiple/images")]
    public async Task<IActionResult> UploadMultipleImages(List<IFormFile> images)
    {
        if (images == null || images.Count == 0)
        {
            return BadRequest(new ApiErrorResponse("FILES_REQUIRED", "No images provided"));
        }

        var results = new List<object>();
        var hasErrors = false;

        foreach (var image in images)
        {
            if (image.Length > 0)
            {
                using var stream = image.OpenReadStream();
                var result = await _fileStorage.SaveFileAsync(stream, image.FileName, FileType.Image, "images");

                if (result.Success)
                {
                    var fileUrl = _fileStorage.GetFileUrl(result.FilePath!);
                    results.Add(new
                    {
                        fileId = result.FileId,
                        success = true,
                        filePath = result.FilePath,
                        fileName = result.FileName,
                        originalFileName = result.OriginalFileName,
                        fileSize = result.FileSize,
                        url = fileUrl
                    });
                }
                else
                {
                    hasErrors = true;
                    results.Add(new
                    {
                        success = false,
                        fileName = image.FileName,
                        error = new { code = "IMAGE_UPLOAD_FAILED", message = result.ErrorMessage }
                    });
                }
            }
        }

        if (hasErrors)
        {
            return BadRequest(new
            {
                error = new { code = "PARTIAL_UPLOAD_FAILURE", message = "One or more images failed to upload" },
                results
            });
        }

        return Ok(results);
    }
}
