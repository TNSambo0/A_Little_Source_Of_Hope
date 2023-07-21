using A_Little_Source_Of_Hope.Models;

namespace A_Little_Source_Of_Hope.Data
{
    public class Producttatus
    {
        public static bool IsProductActive(Product product)
        {
            if (product.Quantity < 1)
            {
                return false;
            }
            return true;
        }
    }
}
