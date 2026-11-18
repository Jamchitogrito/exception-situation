using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        CalcLog.Info($"Попытка добавить в корзину: {product?.name}");
        Console.WriteLine("------------------------------------------------");
        var sw = new Stopwatch();
        sw.Start();
        Debug.WriteLine("разбор операции добавления в корзину");
        Debug.Indent();
        try
        {
            if (product == null)
            {
                throw new InvalidOperationException("Товар не выбран");
            }
            Debug.WriteLine($"Товар: {product.name}, цена: {product.price}, кол-во: {product.count}");

            if (product.count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(product.count),
                    $"Количество товара '{product.name}' должно быть > 0, получено: {product.count}");
            }

            var productFromList = products.FirstOrDefault(w => w.name == product.name && w.price == product.price);
            Debug.Assert(productFromList != null, $"Товар '{product.name}' отсутствует в каталоге");
            if (productFromList == null)
            {
                throw new InvalidOperationException($"Товар '{product.name}' не найден в каталоге");
            }

            if (productFromList.count <= 0)
            {
                throw new OutOfStockException(
                    $"Товар '{product.name}' закончился на складе", productFromList.count);
            }

            Debug.Indent();
            var productFromCart = cartProducts.FirstOrDefault(w => w.name == product.name && w.price == product.price);
            if (productFromCart == null)
            {
                cartProducts.Add(new Product(product.name, product.price, 1));
                productFromList.count -= 1;
                Debug.WriteLine($"Товар добавлен в корзину впервые");
            }
            else
            {
                productFromCart.count += 1;
                productFromList.count -= 1;
                Debug.WriteLine($"Количество в корзине увеличено до {productFromCart.count}");
            }
            Debug.Unindent();

            CalcLog.Info($"Товар '{product.name}' добавлен в корзину");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Debug.WriteLine($"!!! ArgumentOutOfRangeException: {ex.Message}");
            Debugger.Break();
            CalcLog.Error($"ArgumentOutOfRangeException: {ex.Message}");
            Console.WriteLine($"Ошибка количества: {ex.Message}");
        }
        catch (OutOfStockException ex) when (ex is OutOfStockException o && o.AvailableCount == 0)
        {
            Debug.WriteLine($"!!! OutOfStockException (нет в наличии вообще): {ex.Message}");
            CalcLog.Error($"OutOfStockException (AvailableCount=0): {ex.Message}");
            Console.WriteLine($"Товар закончился: {ex.Message}");
        }
        catch (OutOfStockException ex)
        {
            Debug.WriteLine($"!!! OutOfStockException: {ex.Message}, доступно: {ex.AvailableCount}");
            CalcLog.Warning($"OutOfStockException: {ex.Message}");
            Console.WriteLine($"Недостаточно товара: {ex.Message} (доступно {ex.AvailableCount})");
        }
        catch (InvalidOperationException ex)
        {
            Debug.WriteLine($"!!! InvalidOperationException: {ex.Message}");
            CalcLog.Error($"InvalidOperationException: {ex.Message}");
            Console.WriteLine($"Ошибка операции: {ex.Message}");
        }
        finally
        {
            Debug.WriteLine("finally: очистка временных данных и пересчёт итога");
            Debug.Unindent();
            Debug.Unindent();
            Debug.WriteLine("Готово");

            CalculateAllPrice();
            UpdateListBox();
            sw.Stop();
            Console.WriteLine($"Операция заняла {sw.ElapsedMilliseconds} мс");
        }
    }

    public void CalculateAllPrice()
    {
        int Summ = 0;
        foreach (var product in cartProducts)
        {
            Summ += (product.count * product.price);
        }
        ShowAllPrice_TextBlock.Text = $"Итоговая сумма: {Summ.ToString()} руб";
        CalcLog.Info($"Итоговая сумма: {Summ}");
    }

    private void AddToBasket(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Debug.WriteLine("Нажата кнопка «Добавить в корзину»");
        if (selectedProduct == null)
        {
            CalcLog.Warning("Товар не выбран — добавление отменено");
            Console.WriteLine("Товар не выбран");
            return;
        }
        addProductToCartProducts(selectedProduct);
    }

    private void MakeOrder(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        CalcLog.Info("Попытка оформить заказ");
        Console.WriteLine("------------------------------------------------");
        var sw = new Stopwatch();
        sw.Start();

        Debug.WriteLine("Разбор операции оформления заказа");
        Debug.Indent();
        try
        {
            if (cartProducts.Count == 0)
            {
                throw new InvalidOperationException("Нельзя оформить пустую корзину");
            }

            int total = cartProducts.Sum(p => p.count * p.price);
            CalcLog.Info($"Заказ оформлен. Позиций: {cartProducts.Count}, сумма: {total}");
            Console.WriteLine($"Заказ оформлен на сумму {total} руб. Позиций: {cartProducts.Count}");
        }
        catch (InvalidOperationException ex)
        {
            Debug.WriteLine($"!!! InvalidOperationException: {ex.Message}");
            CalcLog.Error($"InvalidOperationException: {ex.Message}");
            Console.WriteLine($"Ошибка заказа: {ex.Message}");
        }
        finally
        {
            Debug.WriteLine("очистка временных данных корзины и обновление итога");
            Debug.Unindent();
            Debug.WriteLine("Готово");

            cartProducts.Clear();
            CalculateAllPrice();
            UpdateListBox();
            sw.Stop();
            Console.WriteLine($"Оформление заняло {sw.ElapsedMilliseconds} мс");
        }
    }

    public void FillUpListOfProducts()
    {
        products.Add(new Product("Кириешки", 50, 20));
        products.Add(new Product("Влажные салфетки", 120, 10));
        products.Add(new Product("Грабли", 450, 5));
        products.Add(new Product("Пакет", 5, 0));
        Debug.WriteLine("Заполнен каталог товаров");
    }

    public void UpdateListBox()
    {
        tv_DataGrid.ItemsSource = null;
        tv_DataGrid.ItemsSource = products.ToList();
    }

    private void tv_DataGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var selected = tv_DataGrid.SelectedItem as Product;
        if (selected == null)
        {
            return;
        }
        selectedProduct = selected;
        CalcLog.Info($"Выбран товар: {selected.name} (цена {selected.price}, остаток {selected.count})");
    }
}

public class OutOfStockException : Exception
{
    public int AvailableCount { get; }
    public OutOfStockException(string message, int availableCount) : base(message)
    {
        AvailableCount = availableCount;
    }
}

[DebuggerDisplay("{name}: Price={price}, Count={count}")]
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

public static class CalcLog
{
    public static TraceSource source;
    static CalcLog()
    {
        source = new TraceSource("ShopCart", SourceLevels.All);
        source.Listeners.Clear();
        source.Listeners.Add(new TextWriterTraceListener("calc.log")
        {
            TraceOutputOptions = TraceOptions.DateTime
        });
    }
    public static void Info(string message)
    {
        source.TraceEvent(TraceEventType.Information, 1, message);
        source.Flush();
    }
    public static void Warning(string message)
    {
        source.TraceEvent(TraceEventType.Warning, 2, message);
        source.Flush();
    }
    public static void Error(string message)
    {
        source.TraceEvent(TraceEventType.Error, 3, message);
        source.Flush();
    }
}