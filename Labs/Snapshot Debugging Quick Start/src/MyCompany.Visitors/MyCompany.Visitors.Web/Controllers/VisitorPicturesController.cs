namespace MyCompany.Visitors.Web.Controllers
{
    using MyCompany.Visitors.Data.Repositories;
    using MyCompany.Visitors.Model;
    using MyCompany.Visitors.Web.Hubs;
    using MyCompany.Visitors.Web.Infraestructure.Security;
    using MyCompany.Visitors.Web.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Http;

    /// <summary>
    /// Travel Attachment Controller
    /// </summary>
    [RoutePrefix("api/visitorpictures")]
    [MyCompanyAuthorization]
    public class VisitorPicturesController : ApiController
    {
        private readonly IVisitorPictureRepository _visitorPictureRepository = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="visitorPictureRepository">IVisitorPictureRepository dependency</param>
        public VisitorPicturesController(IVisitorPictureRepository visitorPictureRepository)
        {
            if (visitorPictureRepository == null)
                throw new ArgumentNullException("visitorPictureRepository");

            _visitorPictureRepository = visitorPictureRepository;
        }

        /// <summary>
        /// Gets the picture for the specified visitor id.
        /// </summary>
        /// <param name="visitorId">The visitor id.</param>
        /// <param name="pictureType">Type of the picture.</param>
        /// <returns></returns>
        [Route("{visitorId:int:min(1)}/{pictureType:int:range(1,2)}")]
        [Route("~/noauth/api/visitorpictures/{visitorId:int:min(1)}/{pictureType:int:range(1,2)}")]
        public async Task<byte[]> Get(int visitorId, PictureType pictureType)
        {
            var visitorPicture = await _visitorPictureRepository.GetAsync(visitorId, pictureType);

            if (visitorPicture == null)
                return null;

            return visitorPicture.Content;
        }

        /// <summary>
        /// Add new  travel attachment.
        /// </summary>
        /// <param name="visitorPicture"> travel attachment information</param>
        /// <returns>AtachmentId</returns>
        [WebApiOutputCacheAttribute(false, true)]
        public async Task<int> Add(VisitorPicture visitorPicture)
        {
            if (visitorPicture == null)
                throw new ArgumentNullException("visitorPicture");

            return await _visitorPictureRepository.AddAsync(visitorPicture);
        }

        /// <summary>
        /// Update travel attachment.
        /// </summary>
        /// <param name="visitorPicture"> travel attachment information</param>
        [HttpPut]
        [WebApiOutputCacheAttribute(false, true)]
        public async Task Update(VisitorPicture visitorPicture)
        {
            if (visitorPicture == null)
                throw new ArgumentNullException("visitorPicture");

            await _visitorPictureRepository.UpdateAsync(visitorPicture);
        }

        /// <summary>
        /// Add or update a picture.
        /// </summary>
        /// <returns></returns>
        [Route("addOrUpdatePictures")]
        [Route("~/noauth/api/visitorpictures/addOrUpdatePictures")]
        [HttpPost]
        [WebApiOutputCacheAttribute(false, true)]
        public async Task AddOrUpdatePictures(ICollection<VisitorPicture> visitorPictures)
        {
            if (null == visitorPictures || visitorPictures.Count == 0)
                throw new ArgumentNullException("visitorPictures");

            foreach (var visitorPicture in visitorPictures)
            {
                await _visitorPictureRepository.AddOrUpdateAsync(visitorPicture.VisitorId, visitorPicture.PictureType, visitorPicture.Content);                
            }

            VisitorsNotificationHub.NotifyVisitorPicturesChanged(visitorPictures);
        }

        /// <summary>
        /// Upload Picture
        /// </summary>
        /// <exception cref="System.Web.Http.HttpResponseException"></exception>
        /// <exception cref="System.ArgumentNullException">visitor</exception>
        [Route("pictures")]
        [Route("~/noauth/api/visitorpictures/pictures")]
        [HttpPost]
        [ActionName("UploadPicture")]
        public async Task UploadPicture()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            int visitorId = 0;
            ImageCrop imageCrop = null;
            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var directory = new DirectoryInfo(root);
            if (!directory.Exists)
                directory.Create();

            var provider = new MultipartFormDataStreamProvider(root);

            await Request.Content.ReadAsMultipartAsync(provider);

            var serializer = new Newtonsoft.Json.JsonSerializer();

            foreach (var key in provider.FormData.AllKeys)
            {
                var val = provider.FormData.GetValues(key)[0];

                if (key == "visitorId")
                {
                    visitorId = (int)serializer.Deserialize(new StringReader(val), typeof(int));
                }
                else if (key == "image-crop")
                {
                    imageCrop = (ImageCrop)serializer.Deserialize(new StringReader(val), typeof(ImageCrop));
                }
            }

            if (visitorId <= 0)
                throw new ArgumentException("visitorId");

            if (imageCrop == null)
                throw new ArgumentNullException("imageCrop");

            foreach (var fileData in provider.FileData)
            {
                byte[] file = File.ReadAllBytes(fileData.LocalFileName);

                await CropAndSave(file, visitorId, imageCrop);

                File.Delete(fileData.LocalFileName);
            }
        }

        /// <summary>
        /// Add new request.
        /// </summary>
        /// <param name="visitorPictureId">travel AttachmentId</param>
        [WebApiOutputCacheAttribute(false, true)]
        [Route("{visitorPictureId:int:min(1)}")]
        [Route("~/noauth/api/visitorpictures/{visitorPictureId:int:min(1)}")]
        [HttpDelete]
        public async Task Delete(int visitorPictureId)
        {
            await _visitorPictureRepository.DeleteAsync(visitorPictureId);
        }

        /// <summary>
        /// Dispose all controllers resources
        /// </summary>
        /// <param name="disposing">Managed resources check</param>
        protected override void Dispose(bool disposing)
        {
            _visitorPictureRepository.Dispose();

            base.Dispose(true);
        }
        
        private async Task CropAndSave(byte[] content, int visitorId, ImageCrop crop)
        {
            var webImage = new WebImage(content);
            var visitorPictures = new List<VisitorPicture>();

            double rx = webImage.Width / crop.w;
            double ry = webImage.Height / crop.h;

            var smallImage = CropImage(webImage, rx, ry, crop.smallSelection);
             await _visitorPictureRepository.AddOrUpdateAsync(visitorId, PictureType.Small, smallImage.GetBytes());

             visitorPictures.Add(new VisitorPicture
             {
                 Content = smallImage.GetBytes(),
                 PictureType = PictureType.Small,
                 VisitorId = visitorId
             });

            var bigImage = CropImage(webImage, rx, ry, crop.bigSelection);
            await _visitorPictureRepository.AddOrUpdateAsync(visitorId, PictureType.Big, bigImage.GetBytes());

            visitorPictures.Add(new VisitorPicture
            {
                Content = bigImage.GetBytes(),
                PictureType = PictureType.Big,
                VisitorId = visitorId
            });

            VisitorsNotificationHub.NotifyVisitorPicturesChanged(visitorPictures);
        }

        private WebImage CropImage(WebImage webImage, double rx, double ry, Selection selection)
        {
            var top = (int)(selection.y1 * ry);
            var left = (int)(selection.x1 * rx);
            var bottom = webImage.Height - (int)(selection.y2 * ry);
            var right = webImage.Width - (int)(selection.x2 * rx);

            if (top < 0) top = 0;
            if (left < 0) left = 0;
            if (bottom < 0) bottom = 0;
            if (right < 0) right = 0;

            return webImage.Clone().Crop(top, left, bottom, right);
        }
    }
}