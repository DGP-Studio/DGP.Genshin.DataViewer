using DGP.Genshin.Common.Data.Json;
using DGP.Genshin.Common.Extensions.System;
using DGP.Genshin.DataViewer.Controls.Dialogs;
using DGP.Genshin.DataViewer.Helpers;
using DGP.Genshin.DataViewer.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace DGP.Genshin.DataViewer.Views
{
    /// <summary>
    /// DirectorySelectView.xaml 的交互逻辑
    /// </summary>
    public partial class DirectorySelectView : UserControl, INotifyPropertyChanged
    {
        public DirectorySelectView()
        {
            DataContext = this;
            InitializeComponent();
            Container.GoToElementState("PickingFolder", true);
        }

        private ExcelSplitView? excelSplitView;
        public ExcelSplitView? ExcelSplitView { get => excelSplitView; set => Set(ref excelSplitView, value); }

        private void OpenFolderRequested(object sender, RoutedEventArgs e)
        {
            WorkingFolderService.SelectWorkingFolder();
            string? path = WorkingFolderService.WorkingFolderPath;
            InitializeMapsAsync(path);
        }
        private void OnFolderDrop(object sender, DragEventArgs e)
        {
            string? folder = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0)?.ToString();

            if (Directory.Exists(folder))
            {
                InitializeMapsAsync(folder);
            }
        }
        private async void InitializeMapsAsync(string? path)
        {
            if (path == null)
            {
                return;
            }

            string? mapPath = null;
            string? excelPath = null;
            if (Directory.Exists(path + @"\TextMap\"))
            {
                mapPath = $@"{path}\TextMap\";
            }

            if (Directory.Exists(path + @"\Excel\"))
            {
                excelPath = $@"{path}\Excel\";
            }

            if (Directory.Exists(path + @"\ExcelBinOutput\"))
            {
                excelPath = $@"{path}\ExcelBinOutput\";
            }

            if (mapPath == null || excelPath == null)
            {
                await new SelectionSuggestDialog().ShowAsync();
            }
            else
            {
                if (ExcelSplitView is null)
                {
                    return;
                }
                ExcelSplitView.TextMapCollection = Directory2.GetFileExs(mapPath);
                ExcelSplitView.ExcelConfigDataCollection = Directory2.GetFileExs(excelPath);
                if (!ExcelSplitView.ExcelConfigDataCollection.Any())
                {
                    await new SelectionSuggestDialog().ShowAsync();
                }
                else
                {
                    Container.GoToElementState("SelectingMap", true);
                    MapService.NPCMap = new Lazy<Dictionary<string, string>>(() =>
                    {
                        string jsonData = ExcelSplitView.ExcelConfigDataCollection?.First(f => f.FileName == "Npc").Read() ?? "";
                        JArray? jArray = Json.ToObject<JArray>(jsonData);
                        return jArray is null
                            ? new Dictionary<string, string>()
                            : jArray.ToDictionary(t => (t["Id"] ?? "").ToString(), v => (v["NameTextMapHash"] ?? "").ToString());
                    });
                }
            }
        }

        private void OnMapSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Container.GoToElementState("Confirming", true);
        }

        private void OnConfirmed(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            ExcelSplitView!.IsPaneOpen = true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
