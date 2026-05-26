using Chat_App.Services.Dtos;
using Chat_App.Services.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Chat_App.Services.Implementations
{
    public class FileUploadService : IFileUploadService
    {
        private readonly Cloudinary _cloudinary;

        public FileUploadService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }
        private double? _lastVideoDuration;
        public async Task<UploadResultDto> UploadFile(IFormFile file, string fileType)
        {
            using var stream = file.OpenReadStream();
            var publicId = $"uploads/{Guid.NewGuid()}";
            var cloudUrl = await UploadToCloudinary(file, stream, fileType, publicId);
            var dto = new UploadResultDto { FilePath = cloudUrl, FileName = file.FileName };
            if (fileType == "video") dto.Duration = _lastVideoDuration;
            return dto;
        }

        public async Task<UploadResultDto> UploadGroupFile(IFormFile file, string fileType)
        {
            using var stream = file.OpenReadStream();
            var publicId = $"uploads/{Guid.NewGuid()}";
            var cloudUrl = await UploadToCloudinary(file, stream, fileType, publicId);
            var dto = new UploadResultDto { FilePath = cloudUrl, FileName = file.FileName };
            if (fileType == "video") dto.Duration = _lastVideoDuration;
            return dto;
        }
        private async Task<string> UploadToCloudinary(IFormFile file, Stream stream, string fileType, string publicId)
        {
            switch (fileType)
            {
                case "image":
                    {
                        var uploadParams = new ImageUploadParams
                        {
                            File = new FileDescription(file.FileName, stream),
                            PublicId = publicId
                        };
                        var result = await _cloudinary.UploadAsync(uploadParams);
                        if (result.Error != null)
                            throw new Exception(result.Error.Message);
                        return result.SecureUrl.ToString();
                    }
                case "video":
                    {
                        var uploadParams = new VideoUploadParams
                        {
                            File = new FileDescription(file.FileName, stream),
                            PublicId = publicId
                        };
                        var result = await _cloudinary.UploadAsync(uploadParams);
                        if (result.Error != null)
                            throw new Exception(result.Error.Message);
                        _lastVideoDuration = result.Duration;  // store duration
                        return result.SecureUrl.ToString();
                    }
                case "audio":
                    {
                        var audioFileName = $"audio_{Guid.NewGuid()}.webm";
                        var uploadParams = new RawUploadParams
                        {
                            File = new FileDescription(audioFileName, stream),
                            PublicId = $"uploads/{Guid.NewGuid()}"
                        };
                        var result = await _cloudinary.UploadAsync(uploadParams);
                        if (result.Error != null)
                            throw new Exception(result.Error.Message);
                        return result.SecureUrl.ToString();
                    }
                default:
                    {
                        var uploadParams = new RawUploadParams
                        {
                            File = new FileDescription(file.FileName, stream),
                            PublicId = publicId
                        };
                        var result = await _cloudinary.UploadAsync(uploadParams);
                        if (result.Error != null)
                            throw new Exception(result.Error.Message);
                        return result.SecureUrl.ToString();
                    }
            }
        }
    }
}
