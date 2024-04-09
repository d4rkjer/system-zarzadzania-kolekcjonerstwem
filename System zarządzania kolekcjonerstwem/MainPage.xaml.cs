using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace System_zarządzania_kolekcjonerstwem
{
    public partial class MainPage : ContentPage
    {
        private const string DataFileName = "collections.txt";
        string dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DataFileName);
        private List<Collection> collections;

        public MainPage()
        {
            InitializeComponent();

            System.Diagnostics.Debug.WriteLine("Sciezka: " + dataPath);

            collections = LoadCollections();

            foreach (var collection in collections)
            {
                var label = new Label { Text = collection.Name };
                label.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => CollectionTapped(collection.Name)),
                });
                CollectionStackLayout.Children.Add(label);
            }
        }

        private List<Collection> LoadCollections()
        {
            var collections = new List<Collection>();

            try
            {
                if (File.Exists(dataPath))
                {
                    string[] lines = File.ReadAllLines(dataPath);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length >= 2)
                        {
                            string name = parts[0];
                            List<string> items = parts.Skip(1).ToList();
                            collections.Add(new Collection { Name = name, Items = items });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd podczas wczytywania kolekcji: {ex.Message}");
            }

            return collections;
        }

        private void AddCollectionButton_Clicked(object sender, EventArgs e)
        {
            string collectionName = CollectionNameEntry.Text;
            if (!string.IsNullOrWhiteSpace(collectionName))
            {
                Collection newCollection = new Collection { Name = collectionName, Items = new List<string>() };
                collections.Add(newCollection);

                var label = new Label { Text = newCollection.Name };
                label.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => CollectionTapped(newCollection.Name)),
                });

                CollectionStackLayout.Children.Add(label);

                SaveCollections();
            }
        }


        private void CollectionTapped(string collectionName)
        {
            Collection selectedCollection = collections.FirstOrDefault(c => c.Name == collectionName);
            if (selectedCollection != null)
            {
                Navigation.PushAsync(new CollectionPage(selectedCollection, this));
            }
        }

        public void SaveCollections()
        {
            string dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DataFileName);

            using (StreamWriter writer = new StreamWriter(dataPath))
            {
                foreach (var collection in collections)
                {
                    writer.WriteLine($"{collection.Name},{string.Join(",", collection.Items)}");
                }
            }
        }

    }

    public class Collection
    {
        public string Name { get; set; }
        public List<string> Items { get; set; }
    }
}