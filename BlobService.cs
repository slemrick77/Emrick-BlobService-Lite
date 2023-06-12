using Azure.Storage.Blobs;
using Azure.Identity;
using Azure.Storage.Blobs.Models;
using Azure;
using Azure.Core;

//shift alt f - format code
//ctrl / - comment multiple lines

namespace EmrickExtensions
{

    public static class StringExtension
    {
        ///<summary>
        ///Does this string end in a forward slash?  This indicates the item is a folder.
        ///</summary>
        ///<param name="input">Azure blob item (file name or folder name).</param>
        public static bool IsFolder(this string input)
        {
            if (input.Substring(input.Length-1, 1) == "/")
                return true;
            else
                return false;
        }
    }

}

namespace Emrick
{
    using EmrickExtensions;

    ///<summary>
    ///The DefaultBlobService is instantiated using the DefaulAzureCredential.
    ///</summary>
    public class DefaultBlobService:BlobService
    {

        ///<summary>
        ///Instantiate the DefaultBlobService.
        ///</summary>
        ///<param name="azureAccountUrl">The URL of the Azure storage account.  Typically "https://NAME_OF_STORAGE_ACCOUNT.blob.core.windows.net"</param>
        ///<param name="blobContainerName">Name of the Azure storage container.  Can be found on the Azure portal.</param>
        public DefaultBlobService(string azureAccountUrl, string blobContainerName)
            :base(new DefaultAzureCredential(), azureAccountUrl, blobContainerName)
        {}

    }

    ///<summary>
    ///The CStringBlobService is instantiated using a connection string obtained from the Azure portal.
    ///</summary>
    public class CStringBlobService:BlobService
    {

        ///<summary>
        ///Instantiate the CStringBlobService.
        ///</summary>
        ///<param name="connectionString">The connection string which can be found on the Azure Portal, contains both the endpoint and an access key.</param>
        ///<param name="blobContainerName">Name of the Azure storage container.  Can be found on the Azure portal.</param>
        public CStringBlobService(string connectionString, string blobContainerName)
            :base(connectionString, blobContainerName)
        {}

    }

    ///<summary>
    ///The ClientSecretBlobService is instantiated using a client id and secret obtained from the Azure portal.
    ///</summary>
    public class ClientSecretBlobService:BlobService
    {

        ///<summary>
        ///Instantiate the ClientSecretBlobService.
        ///</summary>
        ///<param name="azureAccountUrl">The URL of the Azure storage account.  Typically "https://NAME_OF_STORAGE_ACCOUNT.blob.core.windows.net"</param>
        ///<param name="blobContainerName">Name of the Azure storage container.  Can be found on the Azure portal.</param>
        ///<param name="tenantId">Uniquely identifies the Azure tenant.  Can be found on the Azure portal.</param>
        ///<param name="clientId">Uniquely identifies the .NET application.  Can be generated on the Azure portal.</param>
        ///<param name="clientSecret">Authenticates and authorizes the .NET application.  Analagous to a password.  Can be generated on the Azure portal.</param>
        public ClientSecretBlobService(string azureAccountUrl, string blobContainerName, string tenantId, string clientId, string clientSecret)
            :base(new ClientSecretCredential(tenantId,clientId,clientSecret), azureAccountUrl, blobContainerName)
        {}

    }

    ///<summary>
    ///The BlobService encapsulates Azure Blob Storage functionality in a simple to use class.
    ///</summary>
    public class BlobService
    {

        //Represents the number of blobs to retrieve at a time when retrieving mulitple blobs.
        private const int PageSize = 10;

        //Created by the contructor and used by most methods.
        private BlobServiceClient _blobServiceClient;
        //Created by the contructor and used by most methods.
        private BlobContainerClient _blobContainerClient;

        ///<summary>
        ///Constructor is used when authenticating using a connection string.
        ///</summary>
        public BlobService(string? connectionString, string blobContainerName)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
        }

        ///<summary>
        ///Constructor is used when authenticating using either the DefaultAzureCredential or the ClientSecretCredential.
        ///</summary>
        public BlobService(TokenCredential credential, string azureAccountUrl, string blobContainerName)
        {
            _blobServiceClient = new BlobServiceClient(new Uri(azureAccountUrl), credential);
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
        }

        ///<summary>
        ///Retrieves a single file as a stream.
        ///</summary>
        ///<param name="filePath">Name of the file including the folder path.</param>
        ///<returns>A stream representing the file.</returns>
        public System.IO.Stream GetFile(string filePath)
        {
            BlobClient blobClient = _blobContainerClient.GetBlobClient(filePath);
            return blobClient.OpenRead();
        }

        ///<summary>
        ///Deletes a single file.
        ///</summary>
        ///<param name="filePath">Name of the file including the folder path.</param>
        public void DeleteFile(string filePath)
        {
            throw new Exception("This feature is not available in the Lite version.");
        }

        ///<summary>
        ///Deletes a single empty folder.
        ///</summary>
        ///<param name="folderPath">Name of the folder including the folder path if it is nested.</param>
        private void DeleteEmptyFolder(string folderPath)
        {
            throw new Exception("This feature is not available in the Lite version.");
        }

        ///<summary>
        ///Delete the folder and all contents contained within.
        ///</summary>
        /// <param name="folderPath">Relative path of the folder to be deleted.  Must end with the "/" character.</param>
        public void DeleteFolderAndContents(string folderPath)
        {
            throw new Exception("This feature is not available in the Lite version.");
        }

        ///<summary>
        ///Uploads a single file.
        ///</summary>
        ///<param name="destinationFilePath">Name of the file including the folder path.</param>
        ///<param name="fileData">Stream representing the file to be uploaded.</param>
        ///<param name="overwrite">Specify whether to overwrite the file if it already exists.</param>
        public void UploadFile(string destinationFilePath, Stream fileData, bool overwrite = false)
        {
            throw new Exception("This feature is not available in the Lite version.");
        }

        ///<summary>
        ///Retrieve a list of filenames and subfolders in a specified folder.
        ///</summary>
        ///<param name="folderPath">Name of the folder including the folder path if it is nested.</param>
        public List<string> GetContentsOfFolder(string folderPath)
        {
            List<string> files = new List<string>();
            var blobPages = _blobContainerClient.GetBlobsByHierarchy(prefix: folderPath, delimiter: "/").AsPages(default, PageSize);

            foreach (Page<BlobHierarchyItem> blobPage in blobPages)
            {
                foreach (BlobHierarchyItem thisBlob in blobPage.Values)
                {

                    if (thisBlob.IsPrefix)
                    {
                        files.Add(thisBlob.Prefix);
                    }
                    else
                    {
                        files.Add(thisBlob.Blob.Name);
                    }

                }
            }

            return files;
        }

    }

}
