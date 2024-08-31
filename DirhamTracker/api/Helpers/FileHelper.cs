namespace api.Helpers;

public static class FileHelper
{
    public static IFormFile ConvertBase64ToIFormFile(string base64String, string fileName, string contentType)
    {
        byte[] byteArray = Convert.FromBase64String(base64String);
        var stream = new MemoryStream(byteArray);
        var formFile = new FormFile(stream, 0, byteArray.Length, null, fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
        return formFile;
    }
}