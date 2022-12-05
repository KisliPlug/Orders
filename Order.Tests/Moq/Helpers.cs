using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Orders.Service.Entities;

namespace Order.Tests.Moq;

public static class Helpers
{
    public static void UpdateOrder(Orders.Service.Entities.Order existing, Orders.Service.Entities.Order updates)
    {
        existing.Description = updates.Description;
        existing.Price = updates.Price;
        existing.ClientId = updates.ClientId;
        existing.DueDate = updates.DueDate;
    }

    public static void UpdateClient(Orders.Service.Entities.Client existing, Orders.Service.Entities.Client updates)
    {
        existing.Description = updates.Description;
        existing.Orders = updates.Orders;
        existing.Name = updates.Name;
    }

    public static IEnumerable<T>? GetElements<T>(this ActionResult<IEnumerable<T>> elements)
    {
        if (elements.Result is OkObjectResult { Value: IEnumerable data })
        {
            var enumerator = data.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is T elem)
                {
                    yield return elem;
                }
            }
        }
    }

    public static T? GetElement<T>(this ActionResult<T> elements)
    {
        if (elements.Result is OkObjectResult okRes)
        {
            if (okRes.Value is T data)
            {
                return data;
            }
        }

        return default(T);
    }
}