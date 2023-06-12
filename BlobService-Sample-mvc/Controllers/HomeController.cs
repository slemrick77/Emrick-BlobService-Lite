using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BlobService_mvc.Models;
using EmrickExtensions;

namespace BlobService_mvc.Controllers;

public class HomeController : Controller
{

    private string BlobConnectionString;
    private string AzureStorageUrl;
    private string ContainerName;
    private string TenantId;
    private string ClientId;
    private string ClientSecret;

    private readonly ILogger<HomeController> _logger;

    private readonly IConfiguration Configuration;

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
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

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Files()
    {
        FilesModel viewModel = new FilesModel();

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

        viewModel.Files = blobService.GetContentsOfFolder(viewModel.CurrentFolder);

        return View(viewModel);
    }

    public IActionResult ViewFile(string filePath)
    {
        Emrick.CStringBlobService blobService = new Emrick.CStringBlobService(BlobConnectionString, ContainerName);

        FilesModel viewModel = new FilesModel();

        if (filePath.IsFolder())
        {
            viewModel.CurrentFolder = filePath;
            viewModel.Files = blobService.GetContentsOfFolder(filePath);
            return View("Files", viewModel);
        }   
        else
        {
            string mime = viewModel.GetMime(Path.GetExtension(filePath));
            if (mime != string.Empty)
                return File(blobService.GetFile(filePath), mime);
            else
                throw new Exception("This file extension is not supported for viewing in the sample app.");
        }         
    }

    public IActionResult DownloadFile(string filePath)
    {
        Emrick.CStringBlobService blobService = new Emrick.CStringBlobService(BlobConnectionString, ContainerName);
        return File(blobService.GetFile(filePath), "application/octet-stream", Path.GetFileName(filePath));
    }

    public IActionResult DeleteFolder(string filePath)
    {
        Emrick.CStringBlobService blobService = new Emrick.CStringBlobService(BlobConnectionString, ContainerName);
        blobService.DeleteFolderAndContents(filePath);

        return RedirectToAction("Files");
    }

    public IActionResult DeleteFile(string currentFolder, string filePath)
    {
        Emrick.CStringBlobService blobService = new Emrick.CStringBlobService(BlobConnectionString, ContainerName);

        FilesModel viewModel = new FilesModel();

        if (filePath.IsFolder())
        {
            blobService.DeleteFolderAndContents(filePath);
        }
        else
        {
            blobService.DeleteFile(filePath);
        }

        viewModel.CurrentFolder = currentFolder;
        viewModel.Files = blobService.GetContentsOfFolder(currentFolder);
        return View("Files", viewModel);
    }


    [HttpGet]
    public IActionResult UploadMultiple()
    {
        return View();
    }

    [HttpPost]
    public IActionResult UploadMultiple(List<IFormFile> files, string folderPath)
    {
        foreach (var thisFile in files)
        {

            using (var memStream = new MemoryStream())
            {
                thisFile.CopyTo(memStream);

                Emrick.CStringBlobService blobService = new Emrick.CStringBlobService(BlobConnectionString, ContainerName);
                memStream.Position = 0;
                blobService.UploadFile(folderPath + "/" + thisFile.FileName, memStream);
            }

        }

        return View();
    }

    [HttpGet]
    public IActionResult Upload()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Upload(IFormFile file, string folderPath)
    {
        using (var memStream = new MemoryStream())
        {
            file.CopyTo(memStream);

            Emrick.CStringBlobService blobService = new Emrick.CStringBlobService(BlobConnectionString, ContainerName);
            memStream.Position = 0;
            blobService.UploadFile(folderPath + "/" + file.FileName, memStream);

        }

        return View();
    }

}
