﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AlbaVulpes.API.Base;
using AlbaVulpes.API.Constants;
using AlbaVulpes.API.Models.Responses;
using AlbaVulpes.API.Modules.Images;
using AlbaVulpes.API.Services;
using AlbaVulpes.API.Services.AWS;
using AlbaVulpes.API.Services.Content;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AlbaVulpes.API.Controllers
{
    [Route("images")]
    [Produces("application/json")]
    public class ImageController : ApiController
    {
        private readonly IFilesService _filesService;

        public ImageController(IUnitOfWork unitOfWork, IValidatorService validator, IFilesService filesService) : base(unitOfWork, validator)
        {
            _filesService = filesService;
        }

        [Authorize(Roles = "Creator")]
        [HttpPost]
        public async Task<IActionResult> Create(IFormFile file)
        {
            if (file == null)
            {
                return BadRequest("File not provided or unreadable");
            }

            var contentType = file.ContentType;
            var supportedFileTypes = new[] { "image/bmp", "image/jpg", "image/jpeg", "image/png", "image/gif" };

            if (!supportedFileTypes.Contains(contentType))
            {
                return BadRequest($"Content Type {contentType} not supported");
            }

            using (var imageFileStream = new MemoryStream())
            {
                await file.CopyToAsync(imageFileStream);

                using (var processedImageStream = ImageProcessor.ProcessImage(imageFileStream, contentType))
                {
                    var fileKey = $"{S3StorageOptions.ImageUploadsKeyPrefix}/image-{Guid.NewGuid()}.jpg";

                    var originalImageUrl = await _filesService.UploadFileAsync(fileKey, processedImageStream);

                    return Ok(new ImageResponse
                    {
                        ImagePath = originalImageUrl
                    });
                }
            }
        }
    }
}