using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using WebApplication1.Model;

namespace WebApplication1.Controllers
{
    
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        // Constructor
        public CustomerController(IConfiguration config)
        {
            _configuration = config;
        }

        // Create Customer
        [HttpPost]
        [Route("addCustomer")]
        public Customer AddNewCustomer(Customer customer)
        {
            try
            {
                SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                SqlCommand cmd = new SqlCommand("INSERT INTO Customer (UserId, Username, Email, FirstName, LastName, CreatedOn, IsActive) VALUES ('"+customer.UserId+"', '"+customer.Username+"', '"+customer.Email+"', '"+customer.FirstName+"', '"+customer.LastName+"', '"+customer.CreatedOn+"', "+customer.IsActive+");", connection);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
                return customer;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }

        // Get All Customers
        [HttpGet]
        [Route("getAllCustomers")]
        public List<Customer> GetCustomerList()
        {
            List<Customer> result = new List<Customer>();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            SqlCommand cmd = new SqlCommand("select * from Customer", connection);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                Customer customer = new Customer();
                customer.UserId = dataTable.Rows[i]["UserId"].ToString();
                customer.Username = dataTable.Rows[i]["Username"].ToString();
                customer.Email = dataTable.Rows[i]["Email"].ToString();
                customer.FirstName = dataTable.Rows[i]["FirstName"].ToString();
                customer.LastName = dataTable.Rows[i]["LastName"].ToString();
                customer.CreatedOn = DateTime.Parse(dataTable.Rows[i]["CreatedOn"].ToString());
                customer.IsActive = int.Parse(dataTable.Rows[i]["IsActive"].ToString());

                result.Add(customer);
            }
            return result;
        }

        // Delete Customer
        [HttpDelete]
        [Route("deleteCustomer/{customerId}")]
        public async Task<ActionResult> DeleteCustomer(string customerId)
        {
            try
            {
                SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                SqlCommand cmd = new SqlCommand("DELETE FROM Customer WHERE UserId='"+customerId.ToString()+"'", connection);
                connection.Open();
                int result = cmd.ExecuteNonQuery();
                connection.Close();
                if (result > 0) {
                    return Ok();
                }
                return BadRequest("Not Deleted !!!");
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Update Customer
        [HttpPost]
        [Route("updateCustomer/{customerId}")]
        public async Task<ActionResult> UpdateCustomer(string customerId, [FromBody] Customer customer)
        {
            try
            {
                //Find the Customer
                SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                SqlCommand cmd = new SqlCommand("UPDATE Customer SET Username='"+customer.Username+ "',Email='"+customer.Email+ "',FirstName='"+customer.FirstName+ "',LastName='"+customer.LastName+ "', CreatedOn='"+customer.CreatedOn+ "',IsActive='"+customer.IsActive+"'     WHERE UserId='" + customerId.ToString()+"'", connection);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
                return Ok();
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get Active Orders by Customer
        [HttpGet]
        [Route("getActiveOrders/{customerId}")]
        public async Task<ActionResult> GetOrdersByCustomer(string customerId)
        {
            List<Order> orders = new List<Order>();
            try
            {
                SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "get_orders_by_customer";
                cmd.Parameters.AddWithValue("@customerId", customerId.ToString());
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Order order = new Order();
                    order.OrderId = reader["OrderId"].ToString();
                    order.ProductId = reader["ProductId"].ToString();
                    order.OrderStatus = int.Parse(reader["OrderStatus"].ToString());
                    order.OrderType = int.Parse(reader["OrderType"].ToString());
                    order.OrderBy = reader["OrderBy"].ToString();
                    order.OrderedOn = DateTime.Parse(reader["OrderedOn"].ToString());
                    order.ShippedOn = DateTime.Parse(reader["ShippedOn"].ToString());
                    order.IsActive = bool.Parse(reader["IsActive"].ToString());

                    orders.Add(order);
                }

                connection.Close();
                return Ok(orders);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
                Console.WriteLine(ex.Message);
            }
        }
    }
}
