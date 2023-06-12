using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlobService_Sample_razor.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    [BindProperty]
    public string? MyStuff {get; set;}

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
        //MyStuff = "Steve";
    }

    public void OnGet(string folderPath)
    {
        MyStuff = folderPath;
    }
}
