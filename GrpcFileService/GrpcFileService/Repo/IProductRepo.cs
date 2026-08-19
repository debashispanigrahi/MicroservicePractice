using GrpcFileService.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace GrpcFileService.Repo
{
    public interface IProductRepo
    {
        Task InsertProductsAsync(List<Product> products);
    }

    public class ProductRepo(IConfiguration configuration) : IProductRepo
    {
        public async Task InsertProductsAsync(List<Product> products)
        {
            var connectionString =
                configuration.GetConnectionString("DefaultConnection");

            await using var connection =
                new SqlConnection(connectionString);

            await connection.OpenAsync();

            var table = CreateProductDataTable(products);

            var parameters = new DynamicParameters();

            parameters.Add(
                "@Products",
                table.AsTableValuedParameter("dbo.ProductTableType"));

            await connection.ExecuteAsync(
                "dbo.InsertProducts",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        private static DataTable CreateProductDataTable(
            List<Product> products)
        {
            var table = new DataTable();

            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Price", typeof(decimal));

            foreach (var product in products)
            {
                table.Rows.Add(
                    product.Name,
                    product.Price);
            }

            return table;
        }
    }
}
