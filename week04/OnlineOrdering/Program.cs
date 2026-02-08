using System;

class Program
{
    static void Main(string[] args)
    {
        // Create first order - Customer in USA
        Address address1 = new Address("123 Main St", "New York", "NY", "USA");
        Customer customer1 = new Customer("John Smith", address1);
        
        Product product1 = new Product("Laptop", "P001", 999.99, 1);
        Product product2 = new Product("Mouse", "P002", 29.99, 2);
        Product product3 = new Product("Keyboard", "P003", 79.99, 1);
        
        Order order1 = new Order(customer1);
        order1.AddProduct(product1);
        order1.AddProduct(product2);
        order1.AddProduct(product3);
        
        // Display Order 1
        Console.WriteLine("=== ORDER 1 ===");
        Console.WriteLine(order1.GetPackingLabel());
        Console.WriteLine(order1.GetShippingLabel());
        Console.WriteLine($"Total Price: ${order1.GetTotalPrice():F2}");
        Console.WriteLine();
        
        // Create second order - Customer outside USA
        Address address2 = new Address("456 Oak Ave", "Toronto", "ON", "Canada");
        Customer customer2 = new Customer("Jane Doe", address2);
        
        Product product4 = new Product("Monitor", "P004", 299.99, 1);
        Product product5 = new Product("Headphones", "P005", 149.99, 1);
        
        Order order2 = new Order(customer2);
        order2.AddProduct(product4);
        order2.AddProduct(product5);
        
        // Display Order 2
        Console.WriteLine("=== ORDER 2 ===");
        Console.WriteLine(order2.GetPackingLabel());
        Console.WriteLine(order2.GetShippingLabel());
        Console.WriteLine($"Total Price: ${order2.GetTotalPrice():F2}");
    }
}