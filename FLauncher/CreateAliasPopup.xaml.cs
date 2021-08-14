using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AdonisUI.Extensions;
using Microsoft.Win32;

namespace FLauncher
{
	/// <summary>
	/// Interaction logic for CreateAliasPopup.xaml
	/// </summary>
	public partial class CreateAliasPopup : AdonisUI.Controls.AdonisWindow
	{
		public Alias newAlias;
		public bool ok = false;

		public CreateAliasPopup()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			newAlias = new Alias();
			newAlias.alias = Alias_Box.Text;
			newAlias.full_path = Filename_Box.Text;
			newAlias.parameters = Param_Box.Text;

			ok = true;

			Close();
		}

		private void Textbox_Text_Changed(object sender, TextChangedEventArgs e)
		{
			if(Ok_Button != null)
			{
				Ok_Button.IsEnabled = true;
			}
			else
			{
				return;
			}
			

			if (String.IsNullOrWhiteSpace(Alias_Box.Text))
			{
				Alias_Box.BorderBrush = Brushes.Red;
				Ok_Button.IsEnabled = false;
			}
			else
			{
				Alias_Box.ClearValue(TextBox.BorderBrushProperty);
			}
			if (String.IsNullOrWhiteSpace(Filename_Box.Text))
			{
				Filename_Box.BorderBrush = Brushes.Red;
				Ok_Button.IsEnabled = false;
			}
			else
			{
				Filename_Box.ClearValue(TextBox.BorderBrushProperty);
			}
		}

		private void Ok_Button_Loaded(object sender, RoutedEventArgs e)
		{
			Alias_Box.Text = "";
		}

		private void Cancel_Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void Open_File_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();

			Nullable<bool> result = openFileDialog.ShowDialog();

			if (result == true)
			{
				Filename_Box.Text = openFileDialog.FileName;
			}
		}
	}
}
