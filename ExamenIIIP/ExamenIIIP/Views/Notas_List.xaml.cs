using ExamenIIIP.Controllers;
using ExamenIIIP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace ExamenIIIP.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Notas_List : ContentPage
    {
        Notas selectedNota;
        [Obsolete]
        public Notas_List()
        {
            InitializeComponent();

            NotasListView.RefreshCommand = new Command(() =>
            {
                OnAppearing();
            });
            ClosePopUpModal.GestureRecognizers.Add(new TapGestureRecognizer((view) => CloseModal()));

            PopUpModal.IsVisible = false;
        }

        protected override async void OnAppearing()
        {
            var NotasList = await NotasController.GetNotas();
            NotasListView.ItemsSource = null;
            NotasListView.ItemsSource = NotasList;
            NotasListView.IsRefreshing = false;

        }
      
        private void NotasListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var nota = e.Item as Notas;
            DescripcionNota.Text = nota.Descripcion;
            FechaNota.Text = nota.Fecha;
            IdNota.Text = nota.Id_Nota;
            ImageNota.Source = nota.PhotoRecord;
           
 

            selectedNota = nota;

            PopUpModal.IsVisible = true;
        }
        private void CloseModal()
        {
            PopUpModal.IsVisible = false;
        }

        private async void BorrarNota()
        {
            String id = IdNota.Text;
            bool res = await NotasController.DeleteNota(id);

            if (res)
            {
                await DisplayAlert("Satisfactorio", "Se elimino la nota.", "OK");
                CloseModal();
            }
            else
            {
                await DisplayAlert("Fallo del sistema", "No se pudo eliminar la nota.", "OK");
            }
        }

        private async void UpdateButton_Clicked(object sender, EventArgs e)
        {
            CloseModal();
            Navigation.PushModalAsync(new EditarNota(selectedNota));
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            BorrarNota();
        }

    }
}