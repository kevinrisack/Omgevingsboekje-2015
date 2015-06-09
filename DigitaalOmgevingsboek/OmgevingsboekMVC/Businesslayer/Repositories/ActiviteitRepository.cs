﻿using DigitaalOmgevingsboek;
using DigitaalOmgevingsboek.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage.Blob;

namespace OmgevingsboekMVC.Businesslayer.Repositories
{
    public class ActiviteitRepository : GenericRepository<Activiteit>
    {
        OmgevingsboekContext context;
        public ActiviteitRepository(OmgevingsboekContext context)
            : base(context)
        {
            this.context = context;
        }

        public override Activiteit GetByID(object id)
        {
            var query = (from a in context.Activiteit.Include(a => a.POI)
                                                     .Include(a => a.Foto_Activiteit)
                                                     .Include(a => a.Link)
                                                     .Include(a => a.Leerdoel)
                                                     .Include(a => a.Doelgroep)
                         where a.Id == (int)id
                         select a);
            return query.Single<Activiteit>();
        }

        public void UploadPicture(Foto_Activiteit fotoActiviteit, HttpPostedFileBase picture)
        {
            //retrieve storage account from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            //create the blob client
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            //retrieve reference to a previously created container
            CloudBlobContainer container = blobClient.GetContainerReference("images");

            //retrieve reference to a blob named "pictureName"
            string pictureName = fotoActiviteit.URL;
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(pictureName);

            //create or overwrite the 'picture.FileName" blob with contents from a local file
            blockBlob.UploadFromStream(picture.InputStream);
        }
    }
}