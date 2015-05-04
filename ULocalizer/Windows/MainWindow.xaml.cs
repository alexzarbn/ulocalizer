using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ULocalizer.Binding;
using ULocalizer.Classes;

namespace ULocalizer.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window,INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _Title = "ULocalizer";
        public string WTitle
        {
            get 
            {
                if (string.IsNullOrWhiteSpace(Projects.CurrentProject.Name))
                {
                    return _Title;
                }
                else
                {
                    return _Title + " - "+Projects.CurrentProject.Name + ".ulp";
                }
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;        
        }

        

        private void NewCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Projects.NewProject = new Classes.CProject();
            NewProjectWindow NewProjectWnd = new NewProjectWindow();
            NewProjectWnd.Owner = this;
            NewProjectWnd.Closed += NewProjectWnd_Closed;
            NewProjectWnd.ShowDialog();
        }

        private async Task Build()
        {
            await CBuilder.Build();
            OutputField.ScrollToEnd();
        }

        private async void NewProjectWnd_Closed(object sender, EventArgs e)
        {
            if (Projects.isNewProjectCreated)
            {
                this.GetBindingExpression(Window.TitleProperty).UpdateTarget();
                await Build();
                Projects.isNewProjectCreated = false;
            }
        }
        
        private async void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            await Projects.Open();
            this.GetBindingExpression(Window.TitleProperty).UpdateTarget();
        }
        private async void SaveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            await Projects.CurrentProject.SaveTranslations();
        }

        private void CloseCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void LocDocs_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://docs.unrealengine.com/latest/INT/Gameplay/Localization/index.html");
        }

        private async void BuildBtn_Click(object sender, RoutedEventArgs e)
        {
            await Build();
        }


        private void DefaultLocConfigBtn_Click(object sender, RoutedEventArgs e)
        {
            if (System.IO.File.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\Localization.ini")))
            {
                System.Diagnostics.Process.Start(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\Localization.ini"));
            }
            else
            {
                Common.WriteToConsole("[ERROR] Cannot locate default localization config file.");
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if ((e.NewValue != null) && (e.NewValue.GetType().ToString() == "ULocalizer.Classes.CTranslationNode"))
            {
                    Common.SelectedNode = (CTranslationNode)e.NewValue;
            }
        }

        public ItemsControl GetSelectedTreeViewItemParent(TreeView item)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItem || parent is TreeView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as ItemsControl;
        }

        private void LanguagesBtn_Click(object sender, RoutedEventArgs e)
        {
            LanguagesWindow LanguagesWnd = new LanguagesWindow();
            LanguagesWnd.Closed += LanguagesWnd_Closed;
            LanguagesWnd.Owner = this;
            LanguagesWnd.ShowDialog();
        }

        private async void LanguagesWnd_Closed(object sender, EventArgs e)
        {
            if (Projects.isLanguagesListChanged)
            {
                await CBuilder.Build();
                await CBuilder.BuildTranslations();
                Common.ToggleOverlay();
                Projects.isLanguagesListChanged = false;
            }
        }
    }
}
