using Chat_App.Services.Dtos;

namespace Chat_App.Services.Interfaces
{
    public interface IFileUploadService
    {
        Task<UploadResultDto> UploadFile(IFormFile file, string fileType);
        Task<UploadResultDto> UploadGroupFile(IFormFile file, string fileType);
    }
}
