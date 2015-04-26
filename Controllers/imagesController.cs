using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace testJsonDynamic.Controllers
{
    public class imagesController : ApiController
    {

        testJsonDynamic.storage.blobStorage azure;

        public imagesController(testJsonDynamic.storage.blobStorage _azure)
        {
            azure = _azure;
        }

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public async Task<dynamic> Post()
        {
            try
            {
                //var value = @"{""tipo"":1,""ImagenString"":""/9j/4AAQSkZJRgABAQAAAQABAAD/2wBDABALDA4MChAODQ4SERATGCgaGBYWGDEjJR0oOjM9PDkzODdASFxOQERXRTc4UG1RV19iZ2hnPk1xeXBkeFxlZ2P/2wBDARESEhgVGC8aGi9jQjhCY2NjY2NjY2NjY2NjY2NjY2NjY2NjY2NjY2NjY2NjY2NjY2NjY2NjY2NjY2NjY2NjY2P/wAARCABLAGQDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwDXFTI2aj24oFZXNbFgCnAVCrkU8PSEPxS4poelDCmhC4pQKAacCKpCEApcUopcUxDcUopcUYoEHFFZt1qTQzmMlFx2JyaKCtSUiqjysJGAPANXSKz5P9Y31rJmg8TOPSlFw/tUI6Ud6kROLl/RacLp/wC6Kr0A4qkSy2Lojqo/Onrdjuv61SzmlBqkI0Vul/umnC6T0NUAaUGncRofaY/U/lS/aYv736VnE0E07iHtZWTu7yMGZmLEkUVHuophzeZcIrObkk+taTcKaziKzZqMpKU9aSpsJi009aOlLjiqSIYo604U0DpTwKoQ4U6kxSgcU7AJilxTgKXbRYRFiipdtFMRak4RvoazjWjMcRP9KzDUs2EpKCeOaYzHOBSSExw5NP4pq0p5pkMdxmnDOKaBxUqj1qhCA1Kq5FCxg9qlVecUANC07ZUoj54qZY80xFXy/Wiroi46UUCKdzxC1ZprQu/9Q34VmsTmoZswPajAFNzzS55FBNxw/lTwOlMFOB6UxEv1przxQIHlcKPU1R1WaSKyZo3KnnkVyRnllfMjlqtK5LZ39ve2033JUJ9M1oRKCBg5HrXnsDEEc1u6bcTRsNkjDn8KdibnWrHU6xcUlvyik9SBVodKkojEQxRU1FBVj//Z"",""folder"":""imagenes"",""name"":""test""}";
                string value = await Request.Content.ReadAsStringAsync();
                var entidad = System.Web.Helpers.Json.Decode(value);                

                // En el caso que la imagen sea en base64 string (Cordova)
                if (Convert.ToInt32(entidad.tipo) == 1)
                {
                    Image Imagen = new testJsonDynamic.util.Images.Converter().Base64ToImage(entidad.ImagenString);
                    var imagenArray = new testJsonDynamic.util.Images.Converter().imageToByteArray(Imagen);
                    var result = azure.insertImagen(entidad, imagenArray);
                    return result;
                }
                
                
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}