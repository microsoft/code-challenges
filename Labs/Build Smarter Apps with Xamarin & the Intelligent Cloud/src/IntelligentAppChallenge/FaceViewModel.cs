using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace IntelligentAppChallenge
{
	public class FaceViewModel : INotifyPropertyChanged
	{
		// TODO: Insert your API key here:
		const string API_KEY = "";

		Face face;
		public Face Face
		{
			get { return face; }
			set { face = value; OnPropertyChanged(); }
		}

		List<FaceAttributeType> attributes;

		public FaceViewModel()
		{

			attributes = new List<FaceAttributeType>
			{
				FaceAttributeType.Age,
				FaceAttributeType.Smile,
				FaceAttributeType.Gender
			};
		}


		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged([CallerMemberName] string name = null)
		{
			var changed = PropertyChanged;

			if (changed == null)
				return;

			changed.Invoke(this, new PropertyChangedEventArgs(name));
		}
		#endregion
	}
}
