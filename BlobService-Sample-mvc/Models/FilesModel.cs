namespace BlobService_mvc.Models;

public class FilesModel
{
    public FilesModel(){
        Files = new List<string>();
        CurrentFolder = "/";
    }

    public List<string> Files;

    public string CurrentFolder;

    public bool IsFileViewable(string filePath)
    {
        if (GetMime(Path.GetExtension(filePath)) == string.Empty)
            return false;
        else   
            return true;
    }

    public string GetMime(string extension)
    {
        string mime;

        switch (extension.ToUpper())
        {
            case ".JPG" or ".JPEG":
                mime = "image/jpg";
                break;
            case ".PNG":
                mime = "image/png";
                break;
            case ".TXT":
                mime = "text/plain";
                break;
            case ".PDF":
                mime = "application/pdf";
                break;
            default:
                mime = string.Empty;
                break;
        }

        return mime;
    }

}
