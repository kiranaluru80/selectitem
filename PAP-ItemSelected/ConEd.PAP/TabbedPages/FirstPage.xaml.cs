using System;
using System.Collections.Generic;
using ConEd.PAP.Models;
using ConEd.PAP.ViewModels;
using ConEd.PAP.Views;
using PCLStorage;
using Xamarin.Forms;

namespace ConEd.PAP.TabbedPages
{
    public partial class FirstPage : ContentPage
    {
        public FirstPage(TabPolicyModel policyModel)
        {
            InitializeComponent();

            lstDDT.ItemsSource = policyModel.PolicyItems;
        }

        public string selectedDocName = string.Empty;
        public bool isFavorite;
        public bool _favstate;
        async void TapGestureRecognizer_Tapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                selectedDocName = e.Group.ToString();
                var imageSender = (Image)sender;
                Image _imgstate = (Image)sender;
                var stk = (StackLayout)imageSender.Parent;
                var img = (Image)stk.Children[0];
                // if(imageSender.FindByName<ImageSource>("fav"))
                //overlay
                ContentView ai = this.FindByName<ContentView>("overlay");
                // var answer = await DisplayAlert("", "Would like to mark as favourite", "Yes", "No");
                //if (_imgstate)

                var answer = await DisplayActionSheet("Are you sure to save this pdf to your favorite list? ", "Yes", "No");
                if (answer == "Yes")
                {
                    Device.BeginInvokeOnMainThread(()=>{
                        activityIndicatorLayout.IsVisible = true;
                    });
                    img.Source = "fav_selected.png";
                   
                    //call the save function
                    App.PoliciesRepo.UpdateFavorites(selectedDocName, "1");
                    ViewDocumentViewModel vdv = new ViewDocumentViewModel();
                   bool isDoneFavorite = await vdv.viewDocument(selectedDocName);
                    _favstate = true;
                    activityIndicatorLayout.IsVisible = false;
                }
                else
                {
                    img.Source = "fav-unselected.png";
                    //if exists delete file and update in sqlite
                    App.PoliciesRepo.UpdateFavorites(selectedDocName, "0");
                    var check = await FileSystem.Current.LocalStorage.CheckExistsAsync(selectedDocName);
                    if (ExistenceCheckResult.FileExists == check)
                    {
                        IFile file = await FileSystem.Current.LocalStorage.GetFileAsync(selectedDocName);
                        await file.DeleteAsync();
                    }
                    _favstate = false;
                }
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message);

            }

        }

		private void lstDDT_ItemTapped_1(object sender, ItemTappedEventArgs e)
		{
			//DocumentItem di = new DocumentItem();
			try
			{
				Policies di = new Policies();
				di = (Policies)e.Item;

				string docName = di.DocName;//di.DocumentItemTitle;
				if (_favstate)
				{
					isFavorite = true;
				}
				else
				{
					isFavorite = di.IsFavourite;// di.IsFavorite;

				}

				di = null;

				ViewDocument dd = new ViewDocument(docName, isFavorite);
				Label lblTitle = dd.FindByName<Label>("lblVDTitle");
				lblTitle.Text = "Policies And Procedures";

				Navigation.PushAsync(dd);
				dd = null;
			}
			catch (Exception ex)
			{
				//logger.Error(ex.Message);

			}

		}

	}
}
