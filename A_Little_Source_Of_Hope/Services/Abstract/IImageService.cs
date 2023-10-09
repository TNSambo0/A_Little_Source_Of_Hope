namespace A_Little_Source_Of_Hope.Services.Abstract
{
    public interface IImageService
    {
        string uploadImageToAzure(IFormFile file);
        void deleteImageFromAzure(string fileName);
    }
}
