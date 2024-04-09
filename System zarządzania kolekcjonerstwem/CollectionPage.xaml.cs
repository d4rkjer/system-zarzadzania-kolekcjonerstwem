using Microsoft.Maui.Controls;

namespace System_zarządzania_kolekcjonerstwem;

public partial class CollectionPage : ContentPage
{
    private Collection collection;
    private MainPage mainPage;

    public CollectionPage(Collection collection, MainPage mainPage)
    {
        InitializeComponent();
        this.collection = collection;
        this.mainPage = mainPage;

        Title = collection.Name;

        foreach (var item in collection.Items)
        {
            if (!string.IsNullOrWhiteSpace(item))
            {
                AddItemToLayout(item);
            }
        }
    }



    private void AddItemToLayout(string itemName)
    {
        var stackLayout = new StackLayout { Orientation = StackOrientation.Horizontal };

        var label = new Label
        {
            Text = itemName,
            HorizontalOptions = LayoutOptions.StartAndExpand
        };
        label.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() => ToggleItemSelection(label)),
            NumberOfTapsRequired = 1
        });

        var modifyButton = new Button
        {
            Text = "Modify",
            CommandParameter = itemName,
            HorizontalOptions = LayoutOptions.End
        };
        modifyButton.Clicked += ModifyItemButton_Clicked;

        var deleteButton = new Button
        {
            Text = "Delete",
            CommandParameter = itemName,
            HorizontalOptions = LayoutOptions.End
        };
        deleteButton.Clicked += RemoveItemButton_Clicked;

        stackLayout.Children.Add(label);
        stackLayout.Children.Add(modifyButton);
        stackLayout.Children.Add(deleteButton);

        ItemStackLayout.Children.Add(stackLayout);
    }

    private async void AddItemButton_Clicked(object sender, EventArgs e)
    {
        string itemName = ItemNameEntry.Text;
        if (!string.IsNullOrWhiteSpace(itemName))
        {
            if (collection.Items.Contains(itemName))
            {
                bool addDuplicate = await DisplayAlert("Duplicate Item", "This item already exists in the collection. Do you want to add it again?", "Yes", "No");
                if (!addDuplicate)
                    return;
            }

            collection.Items.Add(itemName);
            AddItemToLayout(itemName);

            mainPage.SaveCollections();
        }
    }

    private async void ModifyItemButton_Clicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        string oldItemName = button.CommandParameter.ToString();

        string newItemName = await DisplayPromptAsync("Modify Item", "Enter new name:", initialValue: oldItemName);
        if (!string.IsNullOrWhiteSpace(newItemName))
        {
            int index = collection.Items.FindIndex(item => item == oldItemName);
            if (index != -1)
            {
                collection.Items[index] = newItemName;

                foreach (var child in ItemStackLayout.Children)
                {
                    if (child is StackLayout stackLayout)
                    {
                        var label = stackLayout.Children.FirstOrDefault(child => child is Label) as Label;
                        if (label != null && label.Text == oldItemName)
                        {
                            label.Text = newItemName;
                            break;
                        }
                    }
                }

                mainPage.SaveCollections();
            }
        }
    }



    private void RemoveItemButton_Clicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        string itemName = button.CommandParameter.ToString();

        foreach (var child in ItemStackLayout.Children)
        {
            if (child is StackLayout stackLayout)
            {
                var label = stackLayout.Children.FirstOrDefault(child => child is Label) as Label;
                if (label != null && label.Text == itemName)
                {
                    ItemStackLayout.Children.Remove(stackLayout);
                    collection.Items.Remove(itemName);
                    mainPage.SaveCollections();
                    break;
                }
            }
        }
    }

    private void ToggleItemSelection(Label label)
    {
        label.FontAttributes = label.FontAttributes == FontAttributes.Bold ? FontAttributes.None : FontAttributes.Bold;
    }
}