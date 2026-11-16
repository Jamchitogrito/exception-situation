using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;

namespace exception_situation;

public partial class MainWindow : Window
{
    public ObservableCollection<Product> products = new ObservableCollection<Product>();
    public ObservableCollection<Product> cartProducts = new ObservableCollection<Product>();
    public Product selectedProduct = new Product();
    public MainWindow()
    {
        InitializeComponent();
        FillUpListOfProducts();
        UpdateListBox();
    }
    public void addProductToCartProducts(Product product)
    {
        var productFromList = products.FirstOrDefault(w => w.name == product.name && w.price == product.price);
        var productFromCart = cartProducts.FirstOrDefault(w => w.name == product.name && w.price == product.price);
        if (productFromCart == null)
        {
            cartProducts.Add(new Product(product.name, product.price, 1));
            productFromList.count -= 1;
        }
        else
        {
            productFromCart.count += 1;
            productFromList.count -= 1;
        }
        CalculateAllPrice();
        UpdateListBox();
    }
    public void CalculateAllPrice()
    {
        int Summ = 0;
        foreach (var product in cartProducts)
        {
            Summ += (product.count * product.price);
        }
        ShowAllPrice_TextBlock.Text = $"Итоговая сумма: {Summ.ToString()} руб";
    }

    private void AddToBasket(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (selectedProduct == null)
        {
            return;
        }
        addProductToCartProducts(selectedProduct);
    }
    private void MakeOrder(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
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
        tv_ListBox.ItemsSource = null;
        tv_ListBox.ItemsSource = products.ToList();
    }

    private void tv_ListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var selected = tv_ListBox.SelectedItem as Product;
        if (selected == null)
        {
            return;
        }
        selectedProduct = selected;
    }
}
public class Product
{
    public string name { get; set; }
    public int price { get; set; }
    public int count { get; set; }
    public Product() { }
    public Product(string name1, int price1, int count1)
    {
        name = name1;
        price = price1;
        count = count1;
    }
    public override string ToString()
    {
        return $"{name}|Цена: {price}|Количество: {count}";
    }
}