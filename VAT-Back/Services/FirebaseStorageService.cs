using Firebase.Storage;
using System.IO;
using System.Threading.Tasks;

namespace VAT_Back.Services
{
    public class FirebaseStorageService
    {
        // Use your actual bucket name found in Firebase Console > Storage
        private readonly string Bucket = "vat-back-db.appspot.com";

        public async Task<string> UploadReceiptImage(string localFilePath, string fileName)
        {
            try
            {
                using var stream = File.OpenRead(localFilePath);

                // This starts the cloud upload process
                var task = new FirebaseStorage(Bucket)
                    .Child("ReceiptPhotos")
                    .Child(fileName)
                    .PutAsync(stream);

                // Wait for the upload to complete and return the public link
                string downloadUrl = await task;
                return downloadUrl;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}