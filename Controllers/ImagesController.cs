using Alody.Helpers;
using Alody.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Azure.Storage.Blobs;

namespace Alody.Controllers
{
    [Route("api/[controller]")]
    public class ImagesController : Controller //This is the controller than handles the file upload 
    {
        public string url = "";
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        
        // make sure that appsettings.json is filled with the necessary details of the azure storage
        private readonly AzureStorageConfig storageConfig = null;

        public ImagesController(IOptions<AzureStorageConfig> config)
        {
            storageConfig = config.Value;
        }

        // POST /api/images/upload
        [HttpPost("[action]")]
        public async Task<IActionResult> Upload(ICollection<IFormFile> files)
        {
            bool isUploaded = false;
            bool isImage = false;
            string ImageName = "";
            string email = "";
            string id = "";
            string pictureToDelete = "";


            try
            {
                if (files.Count == 0)
                    return BadRequest("No files received from the upload"); 

                if (storageConfig.AccountKey == string.Empty || storageConfig.AccountName == string.Empty)
                    return BadRequest("sorry, can't retrieve your azure storage details from appsettings.js, make sure that you add azure storage details there");

                if (storageConfig.Container == string.Empty)
                    return BadRequest("Please provide a name for your image container in the azure blob storage");

                foreach (var formFile in files)
                {
                    if (ViewBag.Email != "")
                    {
                        if (StorageHelper.IsImage(formFile)) //Checks if the file is an image 
                        {
                            //Check if the image's name has invalid characters
                            bool InvalidFileName = CleanString.IsValidFilename(formFile.FileName);

                            //Check if the song's name contains unicode characters that have an AnsiCode > 127
                            bool unicodeCharacters = CleanString.ContainsUnicodeCharacter(formFile.FileName);

                            if (InvalidFileName || !unicodeCharacters)
                            {
                                ImageName = CleanString.UseStringBuilderWithHashSet(formFile.FileName);
                                ImageName = CleanString.EscapeForeignCharacters(ImageName);

                                //If clean up got rid of file name because it only included invalid characters, replace empty string
                                if (ImageName == string.Empty || ImageName == "" || string.IsNullOrEmpty(ImageName))
                                {
                                    ViewBag.Message = "Invalid Image Name!";
                                    return new UnsupportedMediaTypeResult();
                                }
                                else if (ImageName == ".jpg")
                                {
                                    ImageName = "EmptyFileName.jpg";
                                }
                                else if (ImageName == ".png")
                                {
                                    ImageName = "EmptyFileName.png";
                                }
                                else if (ImageName == ".gif")
                                {
                                    ImageName = "EmptyFileName.gif";
                                }
                                else if (ImageName == ".jpeg")
                                {
                                    ImageName = "EmptyFileName.jpeg";
                                }
                            }
                            else
                            {
                                ImageName = formFile.FileName;
                            }
                            url = "https://devstorageale.blob.core.windows.net/muszilla/" + ImageName;

                            if (formFile.Length > 0)
                            {
                                using (Stream stream = formFile.OpenReadStream())
                                {
                                    isUploaded = await StorageHelper.UploadFileToStorage(stream, ImageName, storageConfig);
                                    isImage = true;

                                }
                            }
                        }
                        else if (StorageHelper.IsAudio(formFile)) //Checks if the file is an audio file
                        {
                            ViewBag.Message = "You can only upload an image!";
                            return new UnsupportedMediaTypeResult();
                        }
                        else
                        {
                            return new UnsupportedMediaTypeResult();
                        }
                    }                    
                }
     
                if (isUploaded&&isImage) //Upload procedure for an image
                {
                   
                    string connection = Alody.Properties.Resources.ConnectionString;
                    if (HttpContext.Session.GetString("Email") != null)
                    {
                        using (SqlConnection con = new SqlConnection(connection))
                        {
                            email = HttpContext.Session.GetString("Email");
                            id = HttpContext.Session.GetString("User_ID");
                            pictureToDelete= HttpContext.Session.GetString("Picture");
                       
                            string query = "update Consumer set Picture = '" + url + "'  where Email ='" + email + "'";
                            using (SqlCommand com = new SqlCommand(query, con))
                            {
                                con.Open();
                                com.ExecuteNonQuery();
                                HttpContext.Session.SetString("Picture",url);
                                con.Close();
                            }
                        }
                        DeleteBlob(pictureToDelete);
                    }
                    else
                    {
                        ViewBag.Message = "Error";
                    }
                    return new AcceptedResult();
                }  
                else
                    return BadRequest("Looks like the image couldnt upload to the storage");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
              
        }
        public void DeleteBlob(string deleteFile)
        {
            string[] fileToDel=deleteFile.Split('/');
            BlobClient blobClient = new BlobClient(Alody.Properties.Resources.AzureContainerString, "muszilla", fileToDel[fileToDel.Length-1]);
           

            blobClient.DeleteIfExistsAsync();

            


        }

        // GET /api/images/thumbnails

    }
}