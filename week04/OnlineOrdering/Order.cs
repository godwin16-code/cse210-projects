using System;
using System.Collections.Generic;

public class Order
{
    private List<Product> _products;
    private Customer _customer;

    public Order(Customer customer)
    {
        _customer = customer;
        _products = new List<Product>();
    }

    public void AddProduct(Product product)
    {
        _products.Add(product);
    }

    public double GetTotalPrice()
    {
        double productsTotal = 0;
        foreach (Product product in _products)
        {
            productsTotal += product.GetTotalCost();
        }

        double shippingCost = _customer.IsInUSA() ? 5 : 35;
        return productsTotal + shippingCost;
    }

    public string GetPackingLabel()
    {
        string label = "PACKING LABEL\n";
        foreach (Product product in _products)
        {
            label += $"{product.GetName()} (ID: {product.GetProductId()})\n";
        }
        return label;
    }

    public string GetShippingLabel()
    {
        string label = "SHIPPING LABEL\n";
        label += $"{_customer.GetName()}\n";
        label += _customer.GetAddress().GetFullAddress();
        return label;
    }
}
