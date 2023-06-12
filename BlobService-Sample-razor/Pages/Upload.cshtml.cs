using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlobService_Sample_razor.Pages;

public class UploadModel : PageModel
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
    public IFormFile? FileUpload {get; set;}

    [BindProperty]
    public string? FolderPath {get; set;}

    public UploadModel(ILogger<IndexModel> logger, IConfiguration configuration)
    {

        _logger = logger;
        Configuration = configuration;

        //Collect the ConnectionString from the appsettings file.
        var cs = Configuration["Emrick:BlobService:ConnectionString"];
        if (cs is null) {
            throw new Exception("ConnectionString is not found in appsettings.json.");
        } else {
            BlobConnectionString = cs;
        }

        //Collect the AzureStorageUrl from the appsettings file.
        var asu = Configuration["Emrick:BlobService:AzureStorageUrl"];
        if (asu is null) {
            throw new Exception("AzureStorageUrl is not found in appsettings.json.");
        } else {
            AzureStorageUrl = asu;
        }

        //Collect the ContainerName from the appsettings file.
        var cn = Configuration["Emrick:BlobService:ContainerName"];
        if (cn is null) {
            throw new Exception("ContainerName is not found in appsettings.json.");
        } else {
            ContainerName = cn;
        }

        //Collect the TenantId from the appsettings file.
        var ti = Configuration["Emrick:BlobService:TenantId"];
        if (ti is null) {
            throw new Exception("TenantId is not found in appsettings.json.");
        } else {
            TenantId = ti;
        }

        //Collect the ClientId from the appsettings file.
        var ci = Configuration["Emrick:BlobService:ClientId"];
        if (ci is null) {
            throw new Exception("ClientId is not found in appsettings.json.");
        } else {
            ClientId = ci;
        }

        //Collect the ClientSecret from the appsettings file.
        var csec = Configuration["Emrick:BlobService:ClientSecret"];
        if (csec is null) {
            throw new Exception("ClientSecret is not found in appsettings.json.");
        } else {
            ClientSecret = csec;
        }

    }

    public void OnGet()
    {
        
    }

    public IActionResult OnPostUpload()
    {
        using (var memStream = new MemoryStream())

        {
            FileUpload!.CopyTo(memStream);

            Emrick.CStringBlobService blobService = new Emrick.CStringBlobService(BlobConnectionString, ContainerName);
            memStream.Position = 0;
            blobService.UploadFile(FolderPath + "/" + FileUpload.FileName, memStream);
        }

        return Page();
    }

}