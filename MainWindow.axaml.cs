using System.Collections.Generic;
using Avalonia.Controls;

namespace exception_situation;

public partial class MainWindow : Window
{
    public List<Product> products;
    public MainWindow()
    {
        InitializeComponent();
        FillUpListOfProducts();
        UpdateListBox();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        
    }
    private void Button_Click2(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        
    }
    public void FillUpListOfProducts()
    {
        products.Add(new Product("Кириешки", 50, 20));
        products.Add(new Product("Влажные салфетки", 120, 10));
        products.Add(new Product("Грабли", 450, 5));
    }
    public void UpdateListBox()
    {
        tv_ListBox.ItemsSource = products;
    }
}
public class Product
{
    public string name {get; set;}
    public int price {get; set;}
    public int count {get; set;}
    public Product(){}
    public Product (string name1, int price1, int count1)
    {
        name = name1;
        price = price1;
        count = count1;
    }
}