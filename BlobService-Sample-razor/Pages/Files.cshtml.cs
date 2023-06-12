using Emrick;
using EmrickExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BlobService_Sample_razor.Pages;

public class FilesModel : PageModel
{

    private readonly ILogger<IndexModel> _logger;
    private readonly IConfiguration Configuration;

    private string BlobConnectionString;
    private string AzureStorageUrl;
    private string ContainerName;
    private string TenantId;
    private string ClientId;
    private string ClientSecret;

    [BindProperty]
    public List<string> Files { get; set; }

    [BindProperty]
    public string? CurrentFolder { get; set; }

    public FilesModel(ILogger<IndexModel> logger, IConfiguration configuration)
    {
        _logger = logger;
        Configuration = configuration;

        Files = new List<string>();

        //Collect the ConnectionString from the appsettings file.
        var cs = Configuration["Emrick:BlobService:ConnectionString"];
        if (cs is null)
        {
            throw new Exception("ConnectionString is not found in appsettings.json.");
        }
        else
        {
            BlobConnectionString = cs;
        }

        //Collect the AzureStorageUrl from the appsettings file.
        var asu = Configuration["Emrick:BlobService:AzureStorageUrl"];
        if (asu is null)
        {
            throw new Exception("AzureStorageUrl is not found in appsettings.json.");
        }
        else
        {
            AzureStorageUrl = asu;
        }

        //Collect the ContainerName from the appsettings file.
        var cn = Configuration["Emrick:BlobService:ContainerName"];
        if (cn is null)
        {
            throw new Exception("ContainerName is not found in appsettings.json.");
        }
        else
        {
            ContainerName = cn;
        }

        //Collect the TenantId from the appsettings file.
        var ti = Configuration["Emrick:BlobService:TenantId"];
        if (ti is null)
        {
            throw new Exception("TenantId is not found in appsettings.json.");
        }
        else
        {
            TenantId = ti;
        }

        //Collect the ClientId from the appsettings file.
        var ci = Configuration["Emrick:BlobService:ClientId"];
        if (ci is null)
        {
            throw new Exception("ClientId is not found in appsettings.json.");
        }
        else
        {
            ClientId = ci;
        }

        //Collect the ClientSecret from the appsettings file.
        var csec = Configuration["Emrick:BlobService:ClientSecret"];
        if (csec is null)
        {
            throw new Exception("ClientSecret is not found in appsettings.json.");
        }
        else
        {
            ClientSecret = csec;
        }

    }

    public void OnGet(string currentFolder = "/")
    {
        CurrentFolder = currentFolder;

        //Use a connection string to authenticate.
        Emrick.CStringBlobService blobService = new Emrick.CStringBlobService(BlobConnectionString, ContainerName);

        //Use a client secret to authenticate.
        // Emrick.ClientSecretBlobService blobService = new Emrick.ClientSecretBlobService(AzureStorageUrl,
        //                                                                                 ContainerName,
        //                                                                                 TenantId,
        //                                                                                 ClientId,
        //                                                                                 ClientSecret);

        //Use the default credential to authenticate.
        //Emrick.DefaultBlobService blobService = new Emrick.DefaultBlobService(AzureStorageUrl, ContainerName);

        Files = blobService.GetContentsOfFolder(currentFolder);

    }

    public IActionResult OnPostDelete(string currentFolder, string itemPath)
    {
        Emrick.CStringBlobService blobService = new Emrick.CStringBlobService(BlobConnectionString, ContainerName);

        if (itemPath.IsFolder())
        {
            blobService.DeleteFolderAndContents(itemPath);
        }
        else
        {
            blobService.DeleteFile(itemPath);
        }

        return RedirectToPage("./Files", new { currentFolder = currentFolder });
    }

    public bool IsFileViewable(string filePath)
    {
        if (GetMime(Path.GetExtension(filePath)) == string.Empty)
            return false;
        else
            return true;
    }

    private string GetMime(string extension)
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

    public IActionResult OnPostView(string itemPath)
    {
        Emrick.CStringBlobService blobService = new Emrick.CStringBlobService(BlobConnectionString, ContainerName);

        if (itemPath.IsFolder())
        {
            return RedirectToPage(new { CurrentFolder = itemPath });

        }
        else
        {
            string mime = GetMime(Path.GetExtension(itemPath));
            if (mime != string.Empty)
                return File(blobService.GetFile(itemPath), mime);
            else
                throw new Exception("This file extension is not supported for viewing in the sample app.");

        }
    }

    public IActionResult OnPostDownload(string itemPath)
    {
        Emrick.CStringBlobService blobService = new Emrick.CStringBlobService(BlobConnectionString, ContainerName);
        return File(blobService.GetFile(itemPath), "application/octet-stream", Path.GetFileName(itemPath));
    }
}