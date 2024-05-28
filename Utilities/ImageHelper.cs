using Microsoft.AspNetCore.Mvc;

namespace MVCJuegos.Utilities
{
    public class ImageHelper
    {
        public static string GetImagePath(IWebHostEnvironment webHostEnvironment, IUrlHelper urlHelper, string nombreFormateado)
        {
            string imagePath = "~/imgjuegos/" + nombreFormateado + ".jpg";
            bool imageExists = File.Exists(Path.Combine(webHostEnvironment.WebRootPath, "imgjuegos", nombreFormateado + ".jpg"));

            return urlHelper.Content(imageExists ? imagePath : "~/images/prueba.png");
        }
    }
}
