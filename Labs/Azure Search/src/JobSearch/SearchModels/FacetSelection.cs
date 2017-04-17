using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JobSearch.Annotations;

namespace JobSearch.SearchModels
{
    public class FacetSelection : INotifyPropertyChanged
    {
        public string FacetValue { get; set; }

        public long? FacetCount { get; set; }

        public string FacetDisplay => ToString();

        public override string ToString()
        {
            return FacetValue + (FacetCount.HasValue ? $" ({FacetCount.Value})" : String.Empty);
        }


        private bool? _isSelected;

        public bool? IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
