using Muszilla.Helpers;
using Muszilla.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Muszilla.Controllers
{
    [Route("api/[controller]")]
    public class ImagesController : Controller
    {
        public string url = "";
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;
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
            string email = "";


            try
            {
                if (files.Count == 0)
                    return BadRequest("No files received from the upload");

                if (storageConfig.AccountKey == string.Empty || storageConfig.AccountName == string.Empty)
                    return BadRequest("sorry, can't retrieve your azure storage details from appsettings.js, make sure that you add azure storage details there");

                if (storageConfig.ImageContainer == string.Empty)
                    return BadRequest("Please provide a name for your image container in the azure blob storage");

                foreach (var formFile in files)
                {
                    if (StorageHelper.IsImage(formFile))
                    {
                        url = "https://muzilla.blob.core.windows.net/muzilla/"+formFile.FileName;
                        if (formFile.Length > 0)
                        {
                            using (Stream stream = formFile.OpenReadStream())
                            {
                                isUploaded = await StorageHelper.UploadFileToStorage(stream, formFile.FileName, storageConfig);
                                
                            }
                        }
                    }
                    else
                    {
                        return new UnsupportedMediaTypeResult();
                    }
                }
     
                if (isUploaded)
                {
                    ConsumerModel add = new ConsumerModel();
                    string connection = Muszilla.Properties.Resources.ConnectionString;
                    if (HttpContext.Session.GetString("Email") != null)
                    {
                        using (SqlConnection con = new SqlConnection(connection))
                        {
                            email = HttpContext.Session.GetString("Email");
                            string query = "update Consumer set Picture = '"+url+"'  where Email ='" + email + "'";
                            using (SqlCommand com = new SqlCommand(query, con))
                            {
                                con.Open();
                                com.ExecuteNonQuery();
                                con.Close();
                            }  
                        }
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

        // GET /api/images/thumbnails

    }
}