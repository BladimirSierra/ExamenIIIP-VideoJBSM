using Plugin.Media.Abstractions;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ExamenIIIP.Models;
using ExamenIIIP.Controllers;
using Plugin.AudioRecorder;
using System.IO;
using Xamarin.Essentials;

namespace ExamenIIIP.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditarNota : ContentPage
    {
        MediaFile file;
        Notas nota;
        bool ImageEdited;

        private AudioRecorderService audioRecorderService = new AudioRecorderService()
        {
            StopRecordingOnSilence = false,
            StopRecordingAfterTimeout = false
        };

        private AudioPlayer audioPlayer = new AudioPlayer();

        private bool reproducir = false;

        public EditarNota(Notas _nota)
        {
            InitializeComponent();
            nota = _nota;

            FillForm();
        }
        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            bool response = await Application.Current.MainPage.DisplayAlert("Advertencia", "Escoja de donde quiere que provenga su imagen", "Camara", "Galeria");

            if (response)
                GetImageFromCamera();
            else
                TakeProductImage();

        }
        private async void GetImageFromCamera()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Photos>();
            if (status == PermissionStatus.Granted)
            {
                try
                {
                    file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {

                        PhotoSize = PhotoSize.Medium
                    });
                    if (file == null)
                    {
                        return;
                    }
                    imgFoto.Source = ImageSource.FromStream(() =>
                    {
                        return file.GetStream();
                    });
                }
                catch (Exception ex)
                {

                }
            }
        }


        private void FillForm()
        {
            imgFoto.Source = nota.PhotoRecord;
            DescripcionNotaText.Text = nota.Descripcion;
            string iDate = nota.Fecha;
            DateTime oDate = Convert.ToDateTime(iDate);
            FechaNotaText.Date = oDate;
          
        }

        private async void TakeProductImage()
        {
            await CrossMedia.Current.Initialize();
            try
            {
                file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                {
                    PhotoSize = PhotoSize.Medium
                });
                if (file == null)
                {
                    return;
                }
                imgFoto.Source = ImageSource.FromStream(() =>
                {
                    ImageEdited = true;
                    return file.GetStream();
                });
            }
            catch (Exception ex)
            {

            }
        }

        private async void UpdateButton_Clicked(object sender, EventArgs e)
        {
           
            string ImageNota;

            
          
            if (ImageEdited)
            {
                ImageNota = await NotasController.SaveImage(file.GetStream(), Path.GetFileName(file.Path));
            }
            else
            {
                ImageNota = nota.PhotoRecord;
            }
            DateTime date = (FechaNotaText.Date);

            string str = string.Format("dd-MM-yyyy");
            string ab = date.ToString(str);
            Notas dataUpdate = new Notas()
            {
                Id_Nota = nota.Id_Nota,
                Descripcion = DescripcionNotaText.Text,
                
            Fecha = ab,
                PhotoRecord = ImageNota,
                AudioRecord = ConvertAudioToByteArray(),

            };

            bool res = await NotasController.UpdateNota(dataUpdate);

            if (res)
            {
                await Application.Current.MainPage.DisplayAlert("Satisfactorio", "Nota actualizada satisfactoriamente.", "OK");
                Navigation.PopModalAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Fallo de sistema", "No se pudo actualizar el producto.", "OK");
            }
        }

        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void CancelButton_Clicked_1(object sender, EventArgs e)
        {

        }

        private async void btnGrabar_Clicked(object sender, EventArgs e)
        {
            try
            {
                var status = await Permissions.RequestAsync<Permissions.Microphone>();
                var status2 = await Permissions.RequestAsync<Permissions.StorageRead>();
                var status3 = await Permissions.RequestAsync<Permissions.StorageWrite>();
                if (status != PermissionStatus.Granted & status2 != PermissionStatus.Granted & status3 != PermissionStatus.Granted)
                {
                    return; // si no tiene los permisos no avanza
                }

                if (audioRecorderService.IsRecording)
                {
                    await audioRecorderService.StopRecording();


                    audioPlayer.Play(audioRecorderService.GetAudioFilePath());

                    //txtMessage.Text = "No esta grabando";

                    //txtMessage.TextColor = Color.Red;

                    btnGrabar.Text = "Grabar";

                    reproducir = true;
                }
                else
                {
                    await audioRecorderService.StartRecording();


                    // txtMessage.Text = "Esta grabando";

                    //txtMessage.TextColor = Color.Green;

                    btnGrabar.Text = "Parar";

                    //reproducir = false;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alerta", ex.Message, "OK");
            }

        }
        public byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        private Byte[] ConvertAudioToByteArray()
        {
            Stream audioFile = audioRecorderService.GetAudioFileStream();

            //var mStream = new MemoryStream(File.ReadAllBytes(audioRecorderService.GetAudioFilePath()));
            //var mStream = (MemoryStream)audioFile;

            Byte[] bytes = ReadFully(audioFile);
            return bytes;
        }
    }
}