using System;
using System.Collections.Generic;
using System.Text;
using ExamenIIIP.Models;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ExamenIIIP.Controllers
{
    class NotasController
    {
        private string WebApiKey = "AIzaSyAgx5NQXm2SnC0rzQ19Xx4x_ZEH2dEhX0k";
        public static FirebaseClient firebaseClient = new FirebaseClient("https://exameniiip-6d25c-default-rtdb.firebaseio.com/");
        public static FirebaseStorage firebaseStorage = new FirebaseStorage("exameniiip-6d25c.appspot.com");
        //==============================================================================

        //==============================================================================
        public static async Task<List<Notas>> GetNotas()
        {
            try
            {
                var NotasList = (await firebaseClient
                .Child("Notas")
                .OnceAsync<Notas>()).Select(item =>
                new Notas
                {
                    Id_Nota = item.Key,
                    Descripcion = item.Object.Descripcion,
                    Fecha = item.Object.Fecha,
                    PhotoRecord = item.Object.PhotoRecord,
                    AudioRecord = item.Object.AudioRecord,
                }).ToList();
                return NotasList;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error:{e}");
                return null;
            }
        }
        //==============================================================================

        //==============================================================================
        public static async Task<bool> AddNota(string descripcion, string fecha, string photorecord, Byte[] audiorecord)
        {
            try
            {
                await firebaseClient
                .Child("Notas")
                .PostAsync(new Notas() { Descripcion = descripcion, Fecha = fecha, PhotoRecord = photorecord, AudioRecord = audiorecord });
                return true;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Alerta", e.Message, "OK");
                return false;
            }
        }

        //==============================================================================

        //==============================================================================
        public async void CreateNewNota(string descripcion, string fecha, string photorecord, Byte[] audiorecord)
        {
            if (string.IsNullOrEmpty(descripcion) || string.IsNullOrEmpty(fecha) || photorecord==null || audiorecord==null)
            {
                await Application.Current.MainPage.DisplayAlert("Alerta", "Se requiere llenar todos los campos del formulario.", "OK");
            }
            else
            {
                try
                {
                    await AddNota(descripcion, fecha, photorecord, audiorecord);
                }
                catch (Exception ex)
                {
                }
            }
        }

        //==============================================================================

        //==============================================================================
        public static async Task<bool> DeleteNota(string id)
        {
            await firebaseClient.Child("Notas" + "/" + id).DeleteAsync();
            return true;
        }

        //==============================================================================

        //==============================================================================
        public static async Task<bool> UpdateNota(Notas nota)
        {
            await firebaseClient.Child("Notas" + "/" + nota.Id_Nota).PutAsync(JsonConvert.SerializeObject(nota));
            return true;
        }


        //==============================================================================

        //==============================================================================

        public static async Task<string> SaveImage(Stream image, string filename)
        {
            var img = await firebaseStorage.Child("NotaImage").Child(filename).PutAsync(image);
            return img;
        }


        public static async Task<string> SaveAudio(Stream audio, string filename)
        {
            var img = await firebaseStorage.Child("NotaAudio").Child(filename).PutAsync(audio);
            return img;
        }
        //==============================================================================

        //==============================================================================

        public static async Task<bool> DeleteImage(string filename)
        {
            await firebaseStorage.Child("NotaImage").Child(filename).DeleteAsync();
            return true;
        }
    }
}